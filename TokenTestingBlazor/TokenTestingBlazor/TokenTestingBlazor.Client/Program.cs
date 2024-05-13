using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TokenTestingBlazor.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<CookieStorageAccessor>();
builder.Services.AddSingleton<AzureOAuth>();
builder.Services.AddSingleton<DatabaseAPIService>();
builder.Services.AddScoped<HttpClient>();

await builder.Build().RunAsync();
