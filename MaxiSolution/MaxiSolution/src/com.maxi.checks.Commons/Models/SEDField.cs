namespace Maxi.Services.SouthSide.SouthsideFile
{
    public class SEDField
    {
        public SEDFieldType Type { get; set; }

        public int Lenght { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public bool IsConstant { get; set; }

        public string ValueString { get; set; }

        public byte[] ValueBytes { get; set; }
    }
}
