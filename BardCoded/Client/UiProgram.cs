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
            builder.Services.AddScoped<LocalStorageAccessor>();


            builder.Services.AddSingleton(builder.Configuration.GetRequiredSection("BardcodedApiConfig").Get<BardcodedApiConfiguration>() ?? new BardcodedApiConfiguration());
            var features = builder.Configuration.GetRequiredSection("Application").Get<MyFeatureManager>();
            builder.Services.AddSingleton<IFeatureManager>(features ?? new MyFeatureManager());
            builder.Services.AddTransient<ApiClient>();
            
            builder.RootComponents.Add<NavMenu>("#navmenu");
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            return builder.Build().RunAsync();
        }
    }
}