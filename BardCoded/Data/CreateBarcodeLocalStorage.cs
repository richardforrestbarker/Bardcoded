using Bardcoded.API.Data.Requests;
using Bardcoded.Exceptions;
using Bardcoded.Shaded.Microsoft.FeatureManagement;
using Microsoft.JSInterop;

namespace Bardcoded.Data
{
    public class CreateBarcodeLocalStorage : LocalStorageAccessor
    {
        private const string CreateRequestsLocalStorageKey = "createRequests";
        public CreateBarcodeLocalStorage(IJSRuntime jsRuntime, IFeatureManager features) : base(jsRuntime)
        {
            Features = features;
        }

        public IFeatureManager Features { get; }

        public async Task TryAddToLocalStorage(BardcodeInjestRequest data)
        {
            if (!await Features.IsEnabledAsync("UseLocalStorage"))
            {
                Console.WriteLine("Not using local storage.");
            }
            Dictionary<string, BardcodeInjestRequest> createRequests = await GetValueAsync<Dictionary<string, BardcodeInjestRequest>>(CreateRequestsLocalStorageKey) ?? new Dictionary<string, BardcodeInjestRequest>();
            if (createRequests.TryGetValue(data.Bard, out BardcodeInjestRequest? exists))
            {
                throw new DataConflictException("That barcode is already cached and ready to be stored when the app is back online.", exists);
            }
            createRequests[data.Bard] = data;
            await SetValueAsync("createRequests", createRequests);
        }
    }
}
