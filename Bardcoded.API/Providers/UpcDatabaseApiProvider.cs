using Bardcoded.API.Data.Responses;
using Bardcoded.Data.Responses;
using System.Data;
using System.Text.Encodings.Web;

namespace Bardcoded.API.Providers
{
    public class UpcDatabaseApiProvider : ApiProviderConfiguration
    {

        public UpcDatabaseApiProvider(string path, string key, string url) : base(path, key, url)
        {
        }

        public UpcDatabaseApiProvider()
        {
        }

        public override async Task<BarcodeView> Translate(HttpResponseMessage res)
        {
            var body = res.Content;
            var parsed = await body.ReadFromJsonAsync<UpcDatabaseResponse>();
            if (parsed is UpcItemDataResponse item)
            {
                var images = item.Images?.Select(UrlEncoder.Default.Encode).Aggregate(string.Empty, (c, n) => c + "," + n);
                if (images == string.Empty) images = null;
                return BarcodeView.Create(item.Barcode, item.Title, item.Description, images, "png");
            }
            else if (parsed is FailedUpcResponse error)
            {
                Console.WriteLine($"{nameof(UpcDatabaseApiProvider)}: Request to {Path} failed because {error.Error}");
            }
            else
            {
                Console.WriteLine($"UPCDatabase returned an unexpected response {parsed}");
            }
            return null;

        }

        public override Task UpdateRates()
        {
            return Task.CompletedTask;
        }

        public override Task<bool> IsOverRates()

        {
            return Task.FromResult(false);
        }

        public override async Task<bool> IsResponseKosher(HttpResponseMessage response)
        {
            if (!await base.IsResponseKosher(response)) return false;
            //var body = await response.Content.ReadFromJsonAsync<UpcDatabaseResponse>();
            //return body is UpcItemDataResponse;
            return true;
        }
    }
}
