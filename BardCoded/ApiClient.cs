using Bardcoded.API.Data.Requests;
using Bardcoded.Data.Responses;
using Bardcoded.Shaded.Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Net;

namespace Bardcoded
{
    public class ApiClient : HttpClient, IAsyncDisposable
    {
        private const string CachedBardsKey = "cachedBards";
        private const string CreateRequestsCacheKey = "createRequests";

        public ApiClient(BardcodedApiConfiguration config, LocalStorageAccessor LocalStorageAccessor)
        {
            Config = config;
            this.LocalStorageAccessor = LocalStorageAccessor;
            BaseAddress = new Uri(config.BaseAddress);
        }

        public BardcodedApiConfiguration Config { get; }
        public LocalStorageAccessor LocalStorageAccessor { get; }

        public async Task<BarcodeView?> CreateItem(BardcodeInjestRequest data)
        {
            Dictionary<String, BardcodeInjestRequest> createRequests;
            HttpResponseMessage res;
            try
            {
                res = await PostAsync("/item", JsonContent.Create(data, mediaType: MediaTypeHeaderValue.Parse("application/json")));
            } catch (Exception ex)
            {
                // the server is unreachable for some reason... is it down? is something in the way? does the device have internet?
                Console.WriteLine($"Encountered an error. {ex}");
                createRequests = await LocalStorageAccessor.GetValueAsync<Dictionary<String, BardcodeInjestRequest>>(CreateRequestsCacheKey) ?? new Dictionary<string, BardcodeInjestRequest>();
                if(createRequests.TryGetValue(data.Bard, out BardcodeInjestRequest? exists))
                {
                    throw new DataConflictException("That barcode is already cached and ready to be stored when the app is back online.", exists);
                }
                createRequests[data.Bard] = data;
                await LocalStorageAccessor.SetValueAsync("createRequests", createRequests);
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
                item = await TryGetItemFromCache(bard);
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
                item = await TryGetItemFromCache(bard);
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
            var cachedBards = (await LocalStorageAccessor.GetValueAsync<Dictionary<String, BarcodeView>>(CachedBardsKey)) ?? new Dictionary<string, BarcodeView>();
            cachedBards[data.Code] = data;
            await LocalStorageAccessor.SetValueAsync(CachedBardsKey, cachedBards);
        }

        private async Task<BarcodeView?> TryGetItemFromCache(string bard)
        {
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
                Console.WriteLine($"recieved heathcheck response: {health} ");
                if (res.IsSuccessStatusCode)
                {
                    return Health.Up;
                }
                return health;
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

    public record Health(bool IsUp)
    {
        public static readonly Health Down = new Health(false);
        public static readonly Health Up = new Health(false);
    }
}
