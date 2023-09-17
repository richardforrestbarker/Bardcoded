using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using ZXing;
using ZXing.Common;

namespace BardCoded
{
    public class BarcodeResult
    {
        [JSInvokable(nameof(translate))]
        public static BarcodeResult translate(String imageAsBase64, String type)
        {
            BarcodeResult result = new BarcodeResult();
            var data = Convert.FromBase64String(imageAsBase64);
            using (var image = Image.Load<Rgba32>(data))
            {
                var decoded = result.DecodeFrom(image);
                if (decoded == null) return null;
                return result.mapFrom(decoded);
            }
        }

        public string Text { get; set; }

        public String BarcodeFormat { get; set; }

        public IDictionary<String, object> ResultMetadata { get; set; }
        private BarcodeResult()
        {
            reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>()
            {
                AutoRotate = true,
                Options = Options,
            };
        }

        private DecodingOptions Options = new DecodingOptions
        {
            TryHarder = true,
            TryInverted = true,
            PureBarcode = false,
            ReturnCodabarStartEnd = true,
            //UseCode39ExtendedMode = true,
            //UseCode39RelaxedExtendedMode = true,
        };

        private ZXing.ImageSharp.BarcodeReader<Rgba32> reader { get; set; }

        
        private Result DecodeFrom(Image<Rgba32> image)
        {
            return reader.Decode(image);
        }
        private BarcodeResult mapFrom([NotNull] ZXing.Result data)
        {
            var res = new BarcodeResult();
            res.Text = data.Text;
            res.BarcodeFormat = Enum.GetName(data.BarcodeFormat);
            res.ResultMetadata = data.ResultMetadata.ToDictionary(x => Enum.GetName(x.Key), x => x.Value);
            return res;
        }
    }
}
