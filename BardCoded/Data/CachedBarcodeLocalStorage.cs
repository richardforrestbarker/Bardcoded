using Bardcoded.Data.Responses;
using Bardcoded.Shaded.Microsoft.FeatureManagement;
using Microsoft.JSInterop;

namespace Bardcoded.Data
{
    public class CachedBarcodeLocalStorage : LocalStorageAccessor
    {
        private const string CachedBardsKey = "cachedBards";
        public CachedBarcodeLocalStorage(IJSRuntime jsRuntime, IFeatureManager features) : base(jsRuntime)
        {
            Features = features;
        }

        public IFeatureManager Features { get; }

        public async Task PutItemIntoCache(BarcodeView data)
        {
            if (!await Features.IsEnabledAsync("UseLocalStorage"))
            {
                Console.WriteLine("Not using local storage.");
                return;
            }
            var cachedBards = await GetValueAsync<Dictionary<string, BarcodeView>>(CachedBardsKey) ?? new Dictionary<string, BarcodeView>();
            cachedBards[data.Code] = data;
            await SetValueAsync(CachedBardsKey, cachedBards);
        }

        public async Task<BarcodeView?> TryGetItemFromLocalStorage(string bard)
        {
            if (!await Features.IsEnabledAsync("UseLocalStorage"))
            {
                Console.WriteLine("Not using local storage.");
                return null;
            }
            var items = await GetValueAsync<Dictionary<string, BarcodeView>>(CachedBardsKey);
            if (items == null)
            {
                Console.WriteLine("No Bards have ever been cached.");
                return null;
            }
            if (items.TryGetValue(bard, out BarcodeView? item))
            {
                Console.WriteLine("Found a cached bard.");
                return item;
            }
            else return null;
        }

    }
}
