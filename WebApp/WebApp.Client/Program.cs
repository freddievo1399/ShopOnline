using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShopOnline.Abstractions.License;
using ShopOnline.BaseBlazor.Interface;
using Syncfusion.Blazor;
using WebApp.Client.Services;
SyncfusionLicense.RegisterLicense();

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped(sp => new HttpClient()
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});
Console.WriteLine(builder.HostEnvironment.BaseAddress);
builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped(typeof(IAppBlazorService<>), typeof(HttpService<>));

await builder.Build().RunAsync();
