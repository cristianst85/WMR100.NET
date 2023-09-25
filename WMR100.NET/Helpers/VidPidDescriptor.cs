namespace WMR100.NET.Helpers
{
    public sealed class VidPidDescriptor
    {
        public Descriptor Vendor { get; private set; }

        public Descriptor Product { get; private set; }

        private VidPidDescriptor(ushort vendorId, ushort productId, string vendorName, string productName)
        {
            this.Vendor = new Descriptor(vendorId, vendorName);
            this.Product = new Descriptor(productId, productName);
        }

        public static VidPidDescriptor Wmr100
        {
            get
            {
                return new VidPidDescriptor(0x0fde, 0xca01, "Oregon Scientific", "WMRS200 Weather Station");
            }
        }

        public string GetName()
        {
            return string.Format("{0} {1}", this.Vendor.Name, this.Product.Name);
        }
    }

    public sealed class Descriptor
    {
        public ushort Id { get; private set; }

        public string Name { get; private set; }

        internal Descriptor(ushort id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
