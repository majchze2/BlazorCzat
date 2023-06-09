using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Microsoft.JSInterop;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization();
//var app = builder.Build();
var host = builder.Build();
var jsRuntime = host.Services.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
var culture = await jsRuntime.InvokeAsync<string>("Radzen.getCulture");
if (!string.IsNullOrEmpty(culture))
{
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);
}

await host.RunAsync();