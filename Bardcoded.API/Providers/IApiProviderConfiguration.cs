using System.Text.Json;
using System.Text.Json.Serialization;
using Bardcoded.Data.Responses;

namespace Bardcoded.API.Providers
{

    public class ApiProviderConfiguration
    {
        public ApiProviderConfiguration() { }
        public ApiProviderConfiguration(string path, string key, string url)
        {
            Path = path;
            Key = key;
            Url = url;
        }
        [JsonPropertyName("path")] public string Path { get; set; }
        [JsonPropertyName("$type")] public string Type { get; set; }
        [JsonPropertyName("key")] public string Key { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }

        [JsonExtensionData] public Dictionary<string, JsonElement> UnknownFields { get; set; }

        public virtual Task<HttpClient> GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Url);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Key);
            return Task.FromResult(client);
        }
        public virtual Task<bool> IsResponseKosher(HttpResponseMessage response)
        {
            return Task.FromResult(response.IsSuccessStatusCode);
        }

        public virtual Task<bool> IsOverRates()
        {
            throw new NotImplementedException();
        }

        public virtual Task<BarcodeView> Translate(HttpResponseMessage res)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateRates()
        {
            throw new NotImplementedException();
        }
    }
}
