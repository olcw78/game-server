namespace server.Buffer;

public class RecvBuffer(int _initBufSize) {
    readonly ArraySegment<byte> _buf = new(new byte[_initBufSize], 0, _initBufSize);
    
    int _readPos;
    int _writePos;
    
    public int DataSize => _writePos - _readPos;
    public int FreeSize => _buf.Count - _writePos;
    
    public ArraySegment<byte> ReadSeg => _buf.Slice(_readPos, DataSize);
    public ArraySegment<byte> WriteSeg => _buf.Slice(_writePos, FreeSize);
    
    public void Clean()
    {
      int dataSize = DataSize;
      // 남은 데이터가 없으면 복사없이 커서 위치만 리셋
      if (dataSize == 0) {
        _readPos = _writePos = 0;       
        return;
      }

      // 남은 데이터가 있으므로 커서 위치 초기화 이전에 첫 위치로 복사필요
      if (_buf.Array != null) {
        Array.Copy(
          sourceArray: _buf.Array,
          sourceIndex: _buf.Offset + _readPos,
          destinationArray: _buf.Array,
          destinationIndex: _buf.Offset,
          length: DataSize
        );
        _readPos = 0;
        _writePos = DataSize;
      }
    }
    
    public bool OnRead(int numOfBytes) {
      if (numOfBytes > DataSize)
        return false;
      
      _readPos += numOfBytes;
      return true;
    }
    
    public bool OnWrite(int numOfBytes) {
      if (numOfBytes > FreeSize)
        return false;
      
      _writePos += numOfBytes;
      return true;
    }
}