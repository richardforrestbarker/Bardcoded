using Bardcoded.API.Data;
using Bardcoded.API.Data.Responses;
using Bardcoded.API.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xunit;

namespace Bardcoded.API.Tests
{
    public class UpcDatabaseApiProviderTest
    {
        private const string key = "aKey";
        private const string url = "https://homstead.local";
        private const string templatePath = "apath/{barcode}";

        public UpcDatabaseApiProvider Subject { get; set; }

        public UpcDatabaseApiProviderTest()
        {
            Subject = new UpcDatabaseApiProvider()
            {
                Key = key,
                Url = url,
                Path = templatePath
            };
        }

        [Fact]
       async void creates_httpClient_WithUrlAndAuth()
        {
            var client = await Subject.GetHttpClient();
            Assert.NotNull(client);
            Assert.Equal(new Uri(url), client.BaseAddress);
            Assert.NotNull(client.DefaultRequestHeaders.Authorization);
            Assert.Equal(key, client.DefaultRequestHeaders.Authorization.Parameter);
        }

        [Fact]
        async void translates_ToAnIBarcodeData_WhenDataSaysSuccesss()
        {
            var upcItem = new UpcItemDataResponse() {  };
            var goodResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            goodResponse.Content = JsonContent.Create(upcItem, mediaType: MediaTypeHeaderValue.Parse("application/json"), null);
            var translated = await Subject.Translate(goodResponse);
            Assert.NotNull(translated);
            Assert.Equal(translated.Name, upcItem.Title);
            Assert.Equal(translated.Code, upcItem.Barcode);
            Assert.Equal(translated.Description, upcItem.Description);
        }

        [Fact]
        async void translates_ToAnIBarcodeData_withBase64EncodedImages_WhenDataSaysSuccesss()
        {
            var upcItem = new UpcItemDataResponse()
            {
                
                Images = new[] { "http://url.local","http://url1.local","http://url2.local", },
            };
            var goodResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            goodResponse.Content = JsonContent.Create(upcItem, mediaType: MediaTypeHeaderValue.Parse("application/json"), null);
            var translated = await Subject.Translate(goodResponse);
            Assert.NotNull(translated);
            Assert.NotNull(translated.ImageAsBase64);
            Assert.Equal(upcItem.Images.Select(UrlEncoder.Default.Encode).Aggregate("", (c,n)=>c+","+n) , translated.ImageAsBase64);

        }

        [Fact]
        async void translates_ToAnNull_WhenDataSaysFails()
        {
            var failed = new FailedUpcResponse();
            var badResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            badResponse.Content = JsonContent.Create(failed, mediaType: MediaTypeHeaderValue.Parse("application/json"), null);
            var translated = await Subject.Translate(badResponse);
            Assert.Null(translated);
        }
    }
}
