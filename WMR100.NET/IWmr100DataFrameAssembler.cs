using System.Collections.Generic;

namespace WMR100.NET
{
    public interface IWmr100DataFrameAssembler
    {
        ICollection<Wmr100DataFrame> Assemble(byte[] usbDataBlock);
    }
}