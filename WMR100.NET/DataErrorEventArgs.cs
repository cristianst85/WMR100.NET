namespace WMR100.NET
{
    public class DataErrorEventArgs
    {
        public byte[] FrameData { get; private set; }
        public bool ChecksumValid { get; private set; }
        public bool LengthValid { get; private set; }

        public DataErrorEventArgs(byte[] data, bool checksumValid, bool lengthValid)
        {
            this.FrameData = data;
            this.ChecksumValid = checksumValid;
            this.LengthValid = lengthValid;
        }
    }
}