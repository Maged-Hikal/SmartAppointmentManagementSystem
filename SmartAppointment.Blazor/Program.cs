using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartAppointment.Blazor;
using SmartAppointment.Blazor.Auth;
using SmartAppointment.Blazor.Handlers;
using SmartAppointment.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<ApiAuthenticationStateProvider>());

builder.Services.AddAuthorizationCore();
builder.Services.AddTransient<AuthorizationMessageHandler>();
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7216/");
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient<AppointmentApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7216/");
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(AppointmentApiService)));

builder.Services.AddScoped<AdminApiService>();
builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<INotificationService, NotificationService>();

await builder.Build().RunAsync();
