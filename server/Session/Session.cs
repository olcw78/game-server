using System.Net;
using System.Net.Sockets;

namespace server.Session;

public abstract class Session : ISession {
  protected Session() {
    _recvArgs = new SocketAsyncEventArgs();
    _recvArgs.Completed -= OnRecvComplete;
    _recvArgs.Completed += OnRecvComplete;
    // recv 는 받을 버퍼의 사이즈를 연락 이전까지 어느정도 예측 가능하지만,
    // send 는 알 수 없으므로 미리 버퍼 설정 x
    _recvArgs.SetBuffer(new byte[RECV_BUF_SIZE], 0, RECV_BUF_SIZE);

    _sendArgs = new SocketAsyncEventArgs();
    _sendArgs.Completed -= OnSendComplete;
    _sendArgs.Completed += OnSendComplete;
  }

  public abstract void OnConnect(EndPoint endPoint);
  public abstract void OnDisconnect(EndPoint endPoint);
  public abstract void OnRecv(ArraySegment<byte> data);
  public abstract void OnSend(int byteTransferred);

  #region constant

  private const int SESSION_DISCONNECTED = 1;
  private const int SESSION_CONNECTED = 0;

  private const int RECV_BUF_SIZE = 1024;

  #endregion constant

  #region field

  private int _disconnected;
  public bool Disconnected => _disconnected == SESSION_DISCONNECTED;
  private Socket _conn;

  private readonly SocketAsyncEventArgs _sendArgs;
  private readonly SocketAsyncEventArgs _recvArgs;

  private readonly object _sendLock = new();
  private readonly Queue<byte[]> _sendQueue = new();

  private readonly IList<ArraySegment<byte>> _sendPendingList
    = new List<ArraySegment<byte>>();

  #endregion field

  #region behaviour

  public void Start(Socket conn) {
    _conn = conn;

    StartRecv(_recvArgs);
  }

  public void Disconnect() {
    if (SESSION_DISCONNECTED ==
        Interlocked.Exchange(ref _disconnected, SESSION_DISCONNECTED))
      return;

    _conn.Shutdown(SocketShutdown.Both);
    _conn.Close();
  }

  #endregion behaviour

  #region Recv

  public void StartRecv(SocketAsyncEventArgs args) {
    args.AcceptSocket = null;

    bool isPending = _conn.ReceiveAsync(args);
    if (!isPending) OnRecvComplete(null, args);
  }

  public void OnRecvComplete(object? sender, SocketAsyncEventArgs args) {
    if (args.BytesTransferred > 0 &&
        args.SocketError == SocketError.Success)
      try {
        if (args.Buffer == null) return;

        var recvArgsSlice = new ArraySegment<byte>(
          args.Buffer, args.Offset, args.Buffer.Length);
        OnRecv(recvArgsSlice);

        StartRecv(args);
      }
      catch (Exception e) {
        Console.WriteLine(e);
      }
    else
      Disconnect();
  }

  #endregion Recv

  #region Send

  public void Send(byte[] sendBuf) {
    // 보내는 부분이 재사용됨 -> critical section.
    lock (_sendLock) {
      _sendQueue.Enqueue(sendBuf);

      if (_sendPendingList.Count == 0)
        StartSend();
    }
  }

  public void StartSend() {
    while (_sendQueue.Count > 0) {
      byte[] data = _sendQueue.Dequeue();
      var dataSeg = new ArraySegment<byte>(data, 0, data.Length);
      _sendPendingList.Add(dataSeg);
    }

    _sendArgs.BufferList = _sendPendingList;

    bool isPending = _conn.SendAsync(_sendArgs);
    if (!isPending)
      OnSendComplete(null, _sendArgs);
  }

  public void OnSendComplete(object? sender, SocketAsyncEventArgs args) {
    lock (_sendLock) {
      if (args.BytesTransferred > 0 &&
          args.SocketError == SocketError.Success)
        try {
          _sendArgs.BufferList = null;
          _sendPendingList.Clear();

          Console.WriteLine($"Transferred bytes: {_sendArgs.BytesTransferred}");

          if (_sendQueue.Count > 0)
            StartSend();

          OnSend(args.BytesTransferred);
        }
        catch (Exception e) {
          Console.WriteLine(e);
        }
      else
        Disconnect();
    }
  }

  #endregion Send
}