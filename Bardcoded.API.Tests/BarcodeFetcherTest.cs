using Bardcoded.API.Providers;
using Bardcoded.Data.Responses;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace Bardcoded.API.Tests
{
    public class BarcodeFetcherTest
    {

        HttpClient client { get; set; }
        BarcodeFetcher subject { get; set; }

        public BarcodeFetcherTest()
        {
            client = new HttpClient();
            var fakeProviders = new List<ApiProviderConfiguration>() { new FakeApiProvider() };
            subject = new BarcodeFetcher(fakeProviders, null, null, null);
        }


        [Fact]
        public void WillResolveFromNetwork_ifNotCachedOrDbed()
        {
            //await subject.FindItem("");
        }

        private class FakeMemoryCache : MemoryCache
        {
            public FakeMemoryCache() : base(null,null) { }

            public new ICacheEntry CreateEntry(object key)
            {
                return null;
            }
            public new bool TryGetValue(object key, out object item)
            {
                item = null;
                return false;
            }
        }
        private class FakeApiProvider : ApiProviderConfiguration
        {
            public Dictionary<object, object> UnknownFields { get; set; }
            public string Path { get; set; } = "path";
            public string Key { get; set; } = "api key";
            public string Url { get; set; } = "validuri";

            public HttpClient GetHttpClient()
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(Url);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Key);
                return client;
            }
            public bool IsResponseKosher(HttpResponseMessage response)
            {
                return true;
            }

            public bool OverRates()
            {
                return false;
            }

            public BarcodeView Translate(HttpResponseMessage res)
            {
                return null;
            }

            public void UpdateRates()
            {
                
            }
        }
    }


}