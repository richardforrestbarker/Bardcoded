using Bardcoded.API.Data;
using Bardcoded.API.Providers;
using Bardcoded.Data.Responses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.FeatureManagement;
using System.Runtime.InteropServices;

namespace Bardcoded.API
{

    public class BarcodeFetcher
    {
        private readonly ApiProviderConfiguration[] configs;
        private readonly MemoryCache cache;
        private readonly IFeatureManager features;
        private readonly IBarcodeDataContext database;
        private bool useCache;
        private bool useDb;
        private bool useApis;

        public BarcodeFetcher(List<ApiProviderConfiguration> configs, MemoryCache cache, IFeatureManager features, IBarcodeDataContext database)
        {
            this.configs = configs.ToArray();
            this.cache = cache;
            this.features = features;
            this.database = database;
        }

        public async Task<BarcodeView?> FindItem(String barcode)
        {
            BarcodeView result;
            useCache = await GetUseCache();
            useDb = await GetUseDb();
            useApis = await GetUseApis();

            if (useCache && Cached(barcode))
            {
                result = await Cache(barcode);
                return result;
            }
            else if (!useCache)
            {
                Console.WriteLine("Fetching from Cache is turned off.");
            }

            if (useDb && await Databased(barcode))
            {
                result = await Database(barcode);
                CacheIt(barcode, result);
                return result;
            }
            else if (!useDb)
            {
                Console.WriteLine("Fetching from db is turned off.");
            }
            if (useApis)
            {
                result = await NetworkProviders(barcode);
                if (result == null) return null;
                StoreAndCacheIt(barcode, result);
                return result;
            }
            Console.WriteLine("Fetching from Apis is turned off.");
            return null;
        }

        private Task<bool> GetUseApis()
        {
            return features.IsEnabledAsync("FetchFromApis");
        }

        private Task<bool> GetUseDb()
        {
            return features.IsEnabledAsync("UseDatabase");
        }

        private Task<bool> GetUseCache()
        {
            return features.IsEnabledAsync("UseCache");
        }

        private async void CacheIt(string barcode, BarcodeView result)
        {
            if (!useCache) return;
            var entry = cache.CreateEntry(barcode);
            entry.Value = result;
            entry.SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(60))
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }

        private void StoreAndCacheIt(string barcode, BarcodeView result)
        {
            if (useCache) CacheIt(barcode, result);
            if (useDb) StoreIt(barcode, result);
        }

        private void StoreIt(string barcode, BarcodeView result)
        {

        }

        private async Task<BarcodeView> Database(string barcode)
        {
            IOMapper mapper = new IOMapper();
            var entity = await database.GetBarcode(barcode);
            return mapper.Map(entity);
        }

        private Task<BarcodeView> Cache(string barcode)
        {
            return Task.FromResult(result: cache.Get<BarcodeView>(barcode));
        }

        private async Task<BarcodeView?> NetworkProviders(string barcode)
        {
            foreach (var provider in configs)
            {
                try
                {
                    if (provider.Type.Equals(nameof(UpcDatabaseApiProvider)))
                    {
                        var upc = new UpcDatabaseApiProvider(provider.Path, provider.Key, provider.Url);
                        var client = await upc.GetHttpClient();
                        if (await upc.IsOverRates())
                        {
                            continue;
                        }
                        var response = await client.GetAsync(upc.Path.Replace("{barcode}", barcode));
                        var data = await response.Content.ReadAsStringAsync();
                        if (await upc.IsResponseKosher(response))
                        {
                            return await upc.Translate(response);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Caught a {e.GetType()} trying to get {barcode} from {provider.Type}: {e.Message}");
                }
            }
            return null;
        }

        private async Task<bool> Databased(string barcode)
        {
            return (await database.GetBarcode(barcode)) != null;
        }

        private bool Cached(string barcode)
        {
            return false;
            //return cache.TryGetValue(barcode, out _);
        }
    }
}
