using System.Runtime.CompilerServices;

namespace Bardcoded.Data.Responses
{
    public partial class BarcodeView
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageAsBase64 { get; set; }
        public string? ImageType { get; set; }

        public static BarcodeView Create(string Code, string name, string description, string? ImageAsBase64, string? imageType)
        {
            return new BarcodeView() { Name = name, Description = description, ImageAsBase64 = ImageAsBase64, Code = Code, ImageType = imageType };
        }
    }
}
