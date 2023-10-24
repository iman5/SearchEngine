using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SearchEngine.UI;

// builder is created for the WASM and the RootComponents are added to it.
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Defining the base URI for the backend API service.
Uri BaseURI = new Uri("https://localhost:7120/api/v1");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = BaseURI });

//Running the app
await builder.Build().RunAsync();

