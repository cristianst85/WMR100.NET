namespace WMR100.NET
{
    public class DataFrameErrorEventArgs
    {
        public byte[] FrameData { get; private set; }

        public DataFrameErrorType ErrorType { get; private set; }

        public DataFrameErrorEventArgs(byte[] data, DataFrameErrorType errorType)
        {
            this.FrameData = data;
            this.ErrorType = errorType;
        }
    }
}
