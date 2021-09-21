using Blazored.SessionStorage;
using LT.DigitalOffice.GUI.Pages.Auth;
using LT.DigitalOffice.GUI.Helpers.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace LT.DigitalOffice.GUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddServices();
        

            await builder.Build().RunAsync();
        }
    }
}
