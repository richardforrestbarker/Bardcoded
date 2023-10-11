using Bardcoded.API.Data;
using Bardcoded.API.Providers;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace Bardcoded.API
{
    public class ApiProgram
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddFeatureManagement(builder.Configuration.GetRequiredSection("Application:Features"));
            
            AddApiIntegrationConfigurations(builder);

            builder.Services.AddSingleton<MemoryCache>();
            builder.Services.AddTransient<BarcodeFetcher>();

            builder.Services.AddRateLimiter(opts =>
            {
                opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Request.Headers.Host.ToString(), partition =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            QueueLimit = 10,
                            QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
                            AutoReplenishment = true,
                            Window = TimeSpan.FromSeconds(1)
                        });
                });
                opts.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails()
                    {
                        Status = 429,
                        Detail = "You have exceeded the rate limit.",
                        Title = "Rate Limited"
                    });
                };
            });

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<IBarcodeDataContext, FakeBarcodeDataContext>(ServiceLifetime.Singleton);
            }
            else
            {
                builder.Services.AddDbContext<IBarcodeDataContext, BarcodeDataContext>();
            }

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opts =>
                {
                    var apiInfo = builder.Configuration.GetRequiredSection("OpenApiInfo").Get<OpenApiInfo>();
                    Console.WriteLine($"using info: {JsonSerializer.Serialize(apiInfo)}");
                    opts.SwaggerDoc("v1", apiInfo);

                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });


            var corsconfig = builder.Configuration.GetRequiredSection("Cors").Get<Dictionary<string, CorsPolicy>>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseCors(c =>
            {
                var policy = new CorsPolicy();
                foreach (var kvp in corsconfig)
                {
                    c.WithOrigins(kvp.Value.Origins.ToArray());
                    c.WithMethods("*");
                    c.WithHeaders("*");
                    c.WithExposedHeaders("*");

                    var x = c.Build();
                }
            });

            app.UseRateLimiter();

            app.MapControllers();
            app.MapGet("/health", healthProducer())
                .AllowAnonymous()
                .WithName("Health check")
                .WithSummary("Health check")
                .WithDisplayName("Healthcheck")
                .WithTags("Health")
                .WithOpenApi();

            app.Run();
        }

        private static void AddApiIntegrationConfigurations(WebApplicationBuilder builder)
        {
            var integrations = builder.Configuration.GetRequiredSection("Application:Integrations").Get<List<ApiProviderConfiguration>>();
            if (integrations == null || integrations.Count == 0)
            {
                Console.WriteLine("No API provider configs found. If the feature is on, it still won't work.");
            }
            builder.Services.AddSingleton(sc => integrations ?? new List<ApiProviderConfiguration>());
        }

        private static Func<IConfiguration, IHostEnvironment, Task<IResult>> healthProducer()
        {
            return async (IConfiguration config, IHostEnvironment env) =>
            {
                return Results.Ok(Health.Up);
            };
        }

    }
}