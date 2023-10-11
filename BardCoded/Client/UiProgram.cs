using Bardcoded.Data;
using Bardcoded.Shaded.Microsoft.FeatureManagement;
using Bardcoded.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bardcoded
{
    public class UiProgram
    {
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped<CachedBarcodeLocalStorage>();
            builder.Services.AddScoped<CreateBarcodeLocalStorage>();


            builder.Services.AddSingleton(builder.Configuration.GetRequiredSection("BardcodedApiConfig").Get<BardcodedApiConfiguration>() ?? new BardcodedApiConfiguration());
            builder.Services.AddSingleton<IFeatureManager>(builder.Configuration.GetRequiredSection("Application").Get<MyFeatureManager>() ?? new MyFeatureManager());
            
            
            
            builder.Services.AddTransient<ApiClient>();
            
            builder.RootComponents.Add<NavMenu>("#navmenu");
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadContent>("head::after");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            return builder.Build().RunAsync();
        }
    }
}