using Microsoft.JSInterop;
using System.Text.Json;

namespace Bardcoded
{
    public class LocalCacheStorage : LocalStorageAccessor
    {
        public LocalCacheStorage(IJSRuntime jsRuntime) : base(jsRuntime)
        {
        }
        public string CacheKey { get; set; } = "bardcoded-cache";
        //Dictionary<String, dynamic> Cache { get; set; } = new Dictionary<String, dynamic>();

        public async Task push<T>(string key, T value)
        {
            var current = await GetValueAsync<Dictionary<string, T>>(CacheKey);
            current[key] = value;
            await SetValueAsync(CacheKey, current);
        }

        public async Task<T> pop<T>()
        {
            var current = await GetValueAsync<Dictionary<string, T>>(CacheKey);
            var item = current.First();
            return item.Value;
        }

    }


    public class LocalStorageAccessor : IAsyncDisposable
    {
        private Lazy<IJSObjectReference> _accessorJsRef = new();
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageAccessor(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task WaitForReference()
        {
            if (_accessorJsRef.IsValueCreated is false)
            {
                _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/localstorage.js"));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_accessorJsRef.IsValueCreated)
            {
                await _accessorJsRef.Value.DisposeAsync();
            }
        }
        public async Task<T> GetValueAsync<T>(string key)
        {
            await WaitForReference();
            var json = await _accessorJsRef.Value.InvokeAsync<string>("get", key);
            if (json == null) return default;
            var result = (T)JsonSerializer.Deserialize(json, typeof(T));
            return result;
        }

        public async Task SetValueAsync<T>(string key, T value)
        {
            await WaitForReference();
            var val = JsonSerializer.Serialize(value);
            await _accessorJsRef.Value.InvokeVoidAsync("set", key, val);
        }

        public async Task Clear()
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeVoidAsync("clear");
        }

        public async Task RemoveAsync(string key)
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeVoidAsync("remove", key);
        }
    }
}
