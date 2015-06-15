using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MemTrace
{
  class TraceSession : IDisposable
  {
    public TraceRecorder Recorder { get; private set; }
    public Socket Socket { get; private set; }

    public TraceSession(ITraceFileHandler handler, Socket socket, string filename)
    {
      Socket = socket;
      Recorder = new TraceRecorder(handler, socket, filename);
    }

    ~TraceSession()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      if (Recorder != null)
      {
        Recorder.Dispose();
        Recorder = null;
      }
      if (Socket != null)
      {
        Socket.Dispose();
        Socket = null;
      }

      if (disposing)
      {
        GC.SuppressFinalize(this);
      }
    }
  }

  // Sets up a TCP/IP end point for collecting traces.
  public class TraceListener : IDisposable
  {
    public string TraceDirectory { get; set; }
    public IPAddress BindAddress { get; set; }
    public int BindPort { get; set; }

    public ITraceFileHandler TraceFileHandler { get; private set; }

    private TcpListener m_Socket;
    private List<TraceSession> m_Sessions = new List<TraceSession>();

    public TraceListener(ITraceFileHandler handler)
    {
      TraceFileHandler = handler;
      BindAddress = IPAddress.Loopback;
      BindPort = 9811;
    }

    ~TraceListener()
    {
      Dispose(false);
    }

    public void Start()
    {
      if (String.IsNullOrEmpty(TraceDirectory))
        throw new ApplicationException("TraceDirectory is not valid");

      if (BindAddress == null)
        throw new ApplicationException("BindAddress is not valid");

      if (!Directory.Exists(TraceDirectory))
      {
        Directory.CreateDirectory(TraceDirectory);
      }

      m_Socket = new TcpListener(BindAddress, BindPort);

      m_Socket.Start();

      m_Socket.BeginAcceptSocket(OnIncomingConnection, null);
    }

    private void OnIncomingConnection(IAsyncResult ar)
    {
      if (m_Socket == null)
        return;

      Socket client = m_Socket.EndAcceptSocket(ar);

      // Pick a filename for this trace.
      string fn = String.Format("trace_{0}.mtrace", DateTime.Now.ToFileTimeUtc());
      string path = Path.Combine(TraceDirectory, fn);

      m_Sessions.Add(new TraceSession(TraceFileHandler, client, path));

      // Immediately accept a new client.
      m_Socket.BeginAcceptSocket(OnIncomingConnection, null);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      if (m_Socket != null)
      {
        m_Socket.Stop();
        m_Socket = null;
      }

      if (disposing)
      {
        GC.SuppressFinalize(this);
      }
    }
  }
}
