using Refit;
using SimpleAPI.Core.Configuration;
using SimpleAPI.Core.Filters;
using SimpleAPI.Services;
using SimpleAPI.Services.ExternalServices;

namespace SimpleAPI.Core;

public static class WebApplicationBuilderExtensions
{
    public static void AddCoreServices(this WebApplicationBuilder builder, ApiConfiguration serviceConfiguration)
    {
        builder.Services.AddSingleton(serviceConfiguration);
        builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
        builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

        builder.Services.AddRefitClient<IExternalCardsDataProvider>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(serviceConfiguration.RemoteServiceBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
        builder.Services.AddScoped<ICardsService, CardsService>();
        builder.Services.AddOpenApi();
        builder.Services.AddMemoryCache();
        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks();
    }
}
