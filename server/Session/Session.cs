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

  protected abstract void OnConnect(Socket conn);
  protected abstract void OnDisconnect();
  protected abstract void OnRecv(ArraySegment<byte> recvBuf);
  protected abstract void OnSend(int byteTransferred);

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

  private bool _sendPending;
  private readonly object _sendLock = new();
  private readonly Queue<byte[]> _sendQueue = new();

  #endregion field

  #region behaviour

  public void Start(Socket conn) {
    _conn = conn;

    StartRecv(_recvArgs);
    // this.StartSend(_sendArgs);
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
      // _conn.Send(sendBuf);

      _sendQueue.Enqueue(sendBuf);

      if (!_sendPending) StartSend();
    }
  }

  public void StartSend() {
    _sendPending = true;

    byte[] data = _sendQueue.Dequeue();
    _sendArgs.SetBuffer(data, 0, data.Length);

    bool isPending = _conn.SendAsync(_sendArgs);
    if (!isPending) OnSendComplete(null, _sendArgs);
  }

  public void OnSendComplete(object? sender, SocketAsyncEventArgs args) {
    lock (_sendLock) {
      if (args.BytesTransferred > 0 &&
          args.SocketError == SocketError.Success)
        try {
          if (_sendQueue.Count > 0)
            StartSend();
          else
            _sendPending = false;

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