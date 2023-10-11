using Bardcoded.API.Data.Requests;
using Bardcoded.Data.Responses;
using Bardcoded.Shaded.Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Net;
using Bardcoded.API.Data;
using Bardcoded.Shaded.Microsoft.FeatureManagement;

namespace Bardcoded
{
    public class ApiClient : HttpClient, IAsyncDisposable
    {
        private const string CachedBardsKey = "cachedBards";
        private const string CreateRequestsLocalStorageKey = "createRequests";

        public ApiClient(BardcodedApiConfiguration config, LocalStorageAccessor LocalStorageAccessor, IFeatureManager features)
        {
            Config = config;
            this.LocalStorageAccessor = LocalStorageAccessor;
            Features = features;
            BaseAddress = new Uri(config.BaseAddress);
        }

        public BardcodedApiConfiguration Config { get; }
        public LocalStorageAccessor LocalStorageAccessor { get; }
        public IFeatureManager Features { get; }

        public async Task<BarcodeView?> CreateItem(BardcodeInjestRequest data)
        {
            HttpResponseMessage res;
            try
            {
                res = await PostAsync("/item", JsonContent.Create(data, mediaType: MediaTypeHeaderValue.Parse("application/json")));
            } catch (Exception ex)
            {
                Console.WriteLine($"Encountered an error. {ex}");
                // the server is unreachable for some reason... is it down? is something in the way? does the device have internet?
                await TryAddToLocalStorage(data);
                throw new OfflineException("Cannot create that until the app is back online. The create request has been cached on the device and will be added as soon as the app is back online.");
            }
            try
            {
                // the request actually made it out and back in at this point.
                res.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<BarcodeView>(res.Content.ReadAsStream());
            } catch (HttpRequestException ex)
            {
                if (res.StatusCode.Equals(HttpStatusCode.Conflict))
                {
                    throw new DataConflictException("That barcode already exists in the database.", data);
                }
                var problemJson = await res.Content.ReadAsStringAsync();
                throw new ApiErrorResponseException($"The API indicated a problem: {ex.Message}", data.Bard, res.StatusCode, JsonSerializer.Deserialize<ProblemDetails?>(problemJson), ex);
            }
        }

        private async Task TryAddToLocalStorage(BardcodeInjestRequest data)
        {
            if (!await Features.IsEnabledAsync("UseLocalStorage"))
            {
                Console.WriteLine("Not using local storage.");
            }
            Dictionary<string, BardcodeInjestRequest> createRequests = await LocalStorageAccessor.GetValueAsync<Dictionary<String, BardcodeInjestRequest>>(CreateRequestsLocalStorageKey) ?? new Dictionary<string, BardcodeInjestRequest>();
            if (createRequests.TryGetValue(data.Bard, out BardcodeInjestRequest? exists))
            {
                throw new DataConflictException("That barcode is already cached and ready to be stored when the app is back online.", exists);
            }
            createRequests[data.Bard] = data;
            await LocalStorageAccessor.SetValueAsync("createRequests", createRequests);
        }

        public async Task<BarcodeView?> GetItem(String bard)
        {
            HttpResponseMessage response;
            BarcodeView? item;
            try
            {
                response = await GetAsync($"item?bard={UrlEncoder.Default.Encode(bard)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encountered an error. {ex}");
                item = await TryGetItemFromLocalStorage(bard);
                if (item != null)
                {
                    Console.WriteLine($"Encountered an error calling the api but found that one in the cache.");
                    return item;
                }
                Console.WriteLine($"Encountered an error calling the api and the local cache failed too.");
                return null;
            }
            if (!response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                item = await TryGetItemFromLocalStorage(bard);
                if (item != null)
                {
                    Console.WriteLine($"Received a problem from the API but found the bard in the cache. {res}");
                    return item;
                }
                Console.WriteLine($"Received a problem from the API and the local cache failed too. {res}");
                throw new ApiErrorResponseException(message: "Api Returned a Problem.", bard, response.StatusCode, JsonSerializer.Deserialize<ProblemDetails?>(res));
            };
            item = await response.Content.ReadFromJsonAsync<BarcodeView?>();
            if (item == null) return null;
            await putItemIntoCache(item);
            return item;
        }

        private async Task putItemIntoCache(BarcodeView data)
        {
            if(!await Features.IsEnabledAsync("UseLocalStorage"))
            {
                Console.WriteLine("Not using local storage.");
                return;
            }
            var cachedBards = (await LocalStorageAccessor.GetValueAsync<Dictionary<String, BarcodeView>>(CachedBardsKey)) ?? new Dictionary<string, BarcodeView>();
            cachedBards[data.Code] = data;
            await LocalStorageAccessor.SetValueAsync(CachedBardsKey, cachedBards);
        }

        private async Task<BarcodeView?> TryGetItemFromLocalStorage(string bard)
        {
            if (!await Features.IsEnabledAsync("UseLocalStorage"))
            {
                Console.WriteLine("Not using local storage.");
                return null;
            }
            var items = await LocalStorageAccessor.GetValueAsync<Dictionary<String, BarcodeView>>(CachedBardsKey);
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

        public async Task<List<BarcodeView>> GetItems()
        {
            try
            { 
                var response = await GetAsync($"item/all");
                if (!response.IsSuccessStatusCode)
                {
                    var res = response.Content.ReadFromJsonAsync<ProblemDetails>();
                    Console.WriteLine(res);
                };
                var x = await response.Content.ReadFromJsonAsync<List<BarcodeView>>();
                if (x == null) return null;
                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<Health> Healthcheck()
        {
            try
            {
                var res = await GetAsync("health");
                var health = await res.Content.ReadFromJsonAsync<Health>();
                Console.WriteLine($"received heath-check response: {health} ");
                if (!res.IsSuccessStatusCode) return Health.Down;
                else return health;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Health.Down;
            }
        }

        public async ValueTask DisposeAsync()
        {
            LocalStorageAccessor?.DisposeAsync();
        }
    }
}
