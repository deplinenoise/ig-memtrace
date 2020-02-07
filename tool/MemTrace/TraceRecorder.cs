using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO.Compression;
using System.Net;

namespace MemTrace
{
  public struct ModuleInfo
  {
    public string Name;
    public ulong BaseAddress;
    public ulong SizeBytes;
  }

  public struct TraceMark
  {
    public string Name;
    public ulong TimeStamp;

    public override string ToString()
    {
      return Name;
    }
  }

  public interface ITraceFileHandler
  {
    object OnRecordingStarted(string filename, TraceMeta meta, TraceRecorder recorder);
    void OnRecordingProgress(object context, TraceMeta meta);
    void OnRecordingEnded(object context);
  }

  public class TraceRecorder : IDisposable
  {
    private readonly string m_FileName;
    private Socket m_Socket;
    private FileStream m_OutputFile;
    private object m_Lock = new object();
    private readonly TraceTranscoder m_Analyzer;
    private readonly ITraceFileHandler m_Handler;
    private object m_Context;
    private byte[] m_Buffer = new byte[65536];

    public TraceRecorder(ITraceFileHandler handler, Socket input, string output_filename)
    {
      m_Handler = handler;
      m_FileName = output_filename;
      m_OutputFile = new FileStream(output_filename, FileMode.Create, FileAccess.Write);

      m_Analyzer = new TraceTranscoder(m_OutputFile);

      m_Analyzer.MetaData.Status = TraceStatus.Recording;
      m_Analyzer.MetaData.SourceMachine = input.RemoteEndPoint.ToString();

      m_Socket = input;
      m_Context = handler.OnRecordingStarted(output_filename, m_Analyzer.MetaData, this);

      m_Socket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, OnDataRead, null);
    }

    ~TraceRecorder()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      lock (m_Lock)
      {
        if (m_Socket != null)
        {
          m_Socket.Close();
          m_Socket.Dispose();
          m_Socket = null;
        }
      }

      if (m_OutputFile != null)
      {
        m_OutputFile.Dispose();
        m_OutputFile = null;
      }

      if (disposing)
      {
        GC.SuppressFinalize(this);
      }
    }

    void OnDataRead(IAsyncResult res)
    {
      int bytes_read = 0;

      try
      {
        bytes_read = m_Socket.EndReceive(res);
      }
      catch (SocketException ex)
      {
        Debug.WriteLine("Exception: {0}", ex.Message);
        // Ignore - treat as EOF.
      }
      catch (EndOfStreamException ex)
      {
        Debug.WriteLine("Exception: {0}", ex.Message);
        // Ignore - owner has forcibly closed the socket because we're shutting down.
      }
      catch (ObjectDisposedException ex)
      {
        Debug.WriteLine("Exception: {0}", ex.Message);
        // Ignore - owner has forcibly closed the socket because we're shutting down.
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Exception: {0}", ex.Message);
      }

      m_Analyzer.Update(m_Buffer, bytes_read);

      m_Handler.OnRecordingProgress(m_Context, m_Analyzer.MetaData);

      if (0 == bytes_read || !m_Socket.Connected)
      {
        OnEndOfInput();
      }
      else
      {
          try
          {
              m_Socket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, OnDataRead, null);
          }
          catch (SocketException ex)
          {
              Debug.WriteLine("Exception: {0}", ex.Message);
              OnEndOfInput();
          }
      
      }
    }

    void OnEndOfInput()
    {
      if (m_Socket != null)
      {
        m_Socket.Close();
      }

      m_Analyzer.Flush();
      m_OutputFile.Close();

      m_Handler.OnRecordingProgress(m_Context, m_Analyzer.MetaData);
      m_Handler.OnRecordingEnded(m_Context);
    }

    public void Cancel()
    {
      m_Socket.Close();
    }

    public void AddTraceMarkFromUI()
    {
      m_Analyzer.MetaData.AddMark(new TraceMark { Name = "UI Mark", TimeStamp = m_Analyzer.CurrentTimeStamp });
    }
  }
}
