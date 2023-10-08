using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using ZXing;
using ZXing.Common;

namespace Bardcoded
{ 
    public class BarcodeResult
    {
        
        public static BarcodeResult translateFromBase64(String imageAsBase64, String type)
        {
           
            var data = Convert.FromBase64String(imageAsBase64);
            return translate(data, type);
        }

        [JSInvokable(nameof(translate))]
        public static BarcodeResult translate(byte[] data, string type)
        {
            Console.WriteLine($"translating a {data.Length} byte(s) of an image");
            using (var image = Image.Load<Rgba32>(data))
            {
                BarcodeResult result = new BarcodeResult();
                var decoded = result.DecodeFrom(image);
                if (decoded == null) return null;
                Console.WriteLine($"Decoded a bardcode: {decoded.Text}");
                return result.mapFrom(decoded);
            }
        }

        [JSInvokable(nameof(translateFromStream))]
        public static BarcodeResult translateFromStream(Stream data, string type)
        {
            Console.WriteLine($"translating a {data.Length} byte(s) of an image");
            using (var image = Image.Load<Rgba32>(data))
            {
                BarcodeResult result = new BarcodeResult();
                var decoded = result.DecodeFrom(image);
                if (decoded == null) return null;
                Console.WriteLine($"Decoded a bardcode: {decoded.Text}");
                return result.mapFrom(decoded);
            }
        }

        [JSInvokable(nameof(translateAsync))]
        public static Task<BarcodeResult> translateAsync(byte[] data, string type)
        {
            return Task.Run(() => translate(data, type));
        }

        [JSInvokable(nameof(translateFromStreamAsync))]
        public static Task<BarcodeResult> translateFromStreamAsync(Stream data, string type)
        {
            return Task.Run(() => translateFromStream(data, type));
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
