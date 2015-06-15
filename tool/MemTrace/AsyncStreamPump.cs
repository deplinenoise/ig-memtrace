using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemTrace
{
  interface IAsyncStreamHandler
  {
    void DataRead(byte[] data, int length);
    void IOCompleted();
  }

  sealed class AsyncStreamPump
  {
    private readonly object m_Lock = new object();
    private readonly Socket m_InputStream;
    private readonly Stream m_OutputStream;
    private readonly IAsyncStreamHandler m_Handler;

    private enum BufferState
    {
      Invalid,
      Reading,
      Read,
      Writing
    }

    private class Buffer
    {
      public readonly byte[] Bytes;

      public int Index { get; set; }
      public int ValidSize { get; set; }
      public BufferState State { get; set; }

      public Buffer(int size)
      {
        Bytes = new byte[size];
      }
    }

    private int m_ReadIndex = 0;
    private int m_WriteIndex = 0;
    private int m_PendingReadCount = 0;
    private int m_PendingWriteCount = 0;

    private readonly Buffer[] m_Buffers;
    private bool m_EndOfInput;
    private bool m_EndOfOutput;

    public AsyncStreamPump(Socket input, Stream output, int buffer_size, IAsyncStreamHandler handler)
    {
      m_InputStream = input;
      m_OutputStream = output;
      m_Handler = handler;

      m_Buffers = new Buffer[2];
      for (int i = 0; i < 2; ++i)
      {
        m_Buffers[i] = new Buffer(buffer_size);
        m_Buffers[i].Index = i;
      }

      KickRead();
    }

    private Buffer GetNextReadBuffer()
    {
      Buffer result = null;
      if (m_Buffers[m_ReadIndex].State == BufferState.Invalid)
      {
        result = m_Buffers[m_ReadIndex];
        m_ReadIndex = (m_ReadIndex + 1) & (m_Buffers.Length - 1);
      }
      return result;
    }

    private Buffer GetNextWriteBuffer()
    {
      Buffer result = null;
      if (m_Buffers[m_WriteIndex].State == BufferState.Read)
      {
        result = m_Buffers[m_WriteIndex];
        m_WriteIndex = (m_WriteIndex + 1) & (m_Buffers.Length - 1);
      }
      return result;
    }

    private void KickRead()
    {
      if (0 != m_PendingReadCount || m_EndOfInput || !m_InputStream.Connected)
        return;

      var buf = GetNextReadBuffer();
      if (buf != null)
      {
        ++m_PendingReadCount;
        buf.State = BufferState.Reading;
        m_InputStream.BeginReceive(buf.Bytes, 0, buf.Bytes.Length, SocketFlags.None, OnReadCompleted, buf);
      }
    }

    private void KickWrite()
    {
      if (0 != m_PendingWriteCount)
        return;

      var buf = GetNextWriteBuffer();
      if (buf != null)
      {
        ++m_PendingWriteCount;
        buf.State = BufferState.Writing;
        m_OutputStream.BeginWrite(buf.Bytes, 0, buf.ValidSize, OnWriteCompleted, buf);
      }
    }

    private void OnReadCompleted(IAsyncResult ar)
    {
      lock (m_Lock)
      {
        Buffer buf = (Buffer)ar.AsyncState;
        int bytes_read = 0;
        Debug.Assert(buf.State == BufferState.Reading);

        try
        {
          bytes_read = m_InputStream.EndReceive(ar);
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

        --m_PendingReadCount;
        buf.ValidSize = bytes_read;
        buf.State = BufferState.Read;

        m_Handler.DataRead(buf.Bytes, bytes_read);

        if (0 == bytes_read)
        {
          OnEndOfInput();
          if (m_PendingWriteCount == 0)
          {
            OnEndOfOutput();
          }
          return;
        }

        KickWrite();
        KickRead();
      }
    }

    private void OnEndOfInput()
    {
      m_EndOfInput = true;

      if (0 == m_PendingWriteCount)
      {
        m_Handler.IOCompleted();
      }
    }

    private void OnWriteCompleted(IAsyncResult ar)
    {
      lock (m_Lock)
      {
        Buffer buf = (Buffer)ar.AsyncState;
        Debug.Assert(buf.State == BufferState.Writing);
        m_OutputStream.EndWrite(ar);
        --m_PendingWriteCount;
        buf.State = BufferState.Invalid;

        if (!m_EndOfInput && !m_InputStream.Connected && 0 == m_PendingReadCount)
        {
          OnEndOfInput();
        }

        if (m_EndOfInput)
        {
          OnEndOfOutput();
        }
        else
        {
          KickRead();
          KickWrite();
        }
      }
    }

    private void OnEndOfOutput()
    {
      m_EndOfOutput = true;
      Monitor.Pulse(m_Lock);
    }


    internal void Shutdown()
    {
      lock (m_Lock)
      {
        while (!m_EndOfOutput)
        {
          Monitor.Wait(m_Lock);
        };
      }
    }
  }
}
