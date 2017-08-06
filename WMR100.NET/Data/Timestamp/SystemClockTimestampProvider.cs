using System;

namespace WMR100.NET.Data.Timestamp
{
    public class SystemClockTimestampProvider : ITimestampProvider
    {
        public DateTime Timestamp
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
