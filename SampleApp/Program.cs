using DopplerSDK.ConfigurationProvider;
using SampleApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// SecretOps DopplerClient
// The goal is fetch secrets from Doppler before app initialization to inject values into the configuration store
// just like any of the built-in configuration providers.
// 
// We are working on a custom Configuration Provider (and Service) that will utilize the DopplerClient but we're actively
// seeking feedback on the DopplerClient itself, hence this example repository.


// Step 1. Doppler Client Configuration //

// An instance of DopplerClientConfiguration must be passed to the DopplerClient constructor and provides the
// Doppler Service Token required for authentication

// Requires "DopplerToken" to be set by an existing configuration provider, e.g. environment variable

// If needing to debug your application, an environment variable may not be the easiest thing to setup.
//
// But to avoid hard-coding the "DopplerToken" value in your launchSettings.json or other config file that could accidentally get
// committed, you can opt to store the Service Token in a config file that is only designed to be used locally.

builder.Host.ConfigureAppConfiguration((_, config) =>
{
    // Ensure this file is in your .gitignore
    config.AddJsonFile("dopplerClientConfig.Development.json", true);
});

var dopplerClientConfig = builder.Configuration.Get<DopplerClientConfiguration>();

// Which is equivalent to:
// var dopplerClientConfig = new DopplerClientConfiguration() { DopplerToken = builder.Configuration["DopplerToken"] };
//
// Which might be required if the Service Token is set to a non-standard configuration key
// var dopplerClientConfig = new DopplerClientConfiguration() { DopplerToken = builder.Configuration["App1DopplerToken"] };
//
// Or if the Service Token is provided from a non-configuration provider source
// var dopplerClientConfig = new DopplerClientConfiguration() { DopplerToken = yourCustomClass.DopplerToken };


// Step 2. Doppler Secrets Fetch

// Now let's create an instance of the DopplerClient and perform a secrets fetch which returns a DopplerClientResponse instance 

// The Doppler Client is intentionally designed to never throw or risk Null reference exceptions, even on failure
// because it presumes to be consumed by a configuration provider in which case it may be perfectly reasonable for a
// request to fail config/secrets are injected by another source.
var dopplerClient = new DopplerClient(dopplerClientConfig);

// The DopplerClient will always be return a valid DopplerClientResponse which is a simple data structure to house
// the returned secrets as well as a StatusMessage for inspecting when a request unintentionally fails
var dopplerClientResponse = await dopplerClient.FetchSecretsAsync();

// If failure was unintentional, you can handle it in a way appropriate for your application
// if (!dopplerClientResponse.IsSuccess) Console.WriteLine(dopplerClientResponse.StatusMessage);

// If secrets were fetched successfully, you will now have a Dictionary<string, string> of secrets
Console.WriteLine($"Secrets fetched: ${dopplerClientResponse.Secrets.Count}");


// Step 3. Working with Secrets

// 3.1: Configuration Provider

// A great approach is adding Doppler's secrets to the configuration values so it acts like a regular configuration provider
foreach (var secret in dopplerClientResponse.Secrets) builder.Configuration[secret.Key] = secret.Value;

// This allows you to seamlessly bind complex nested objects from Doppler, simply by using ASP.NET Core's standard
// convention of two underscores in an environment variable when naming the secret in Doppler.

// For example, SMTP__USER_NAME in Doppler is transformed to Smtp:UserName

// Check out the `doppler-template.yaml` file in the root of this repository for additional formatting examples.

// You can bind an instance during initialization
var appSettings = builder.Configuration.Get<AppSettings>();

// Provide easy access to AppSettings without a global singleton thanks to ASP.NET Core Dependency injection
builder.Services.Configure<AppSettings>(builder.Configuration);

// Which can then be accessed with something like the following, e.g. in a Model

// public class MyModel : PageModel
// {
//     public MyModel(IOptions<AppSettings> appSettings)
//     {
//         AppSettings appSettings = appSettings.Value;
//     }
// }


// 3.2 JSON binding

// If you wanted class binding but didn't want to inject configuration values, you can deserialize the Secrets to JSON
// to bind to a class but this is not nearly as robust as configuration value binding and you'll likely need to write
// a custom serializer to get things working, e.g. the below fails parsing the "true" or"True" to a boolean type
// JsonSerializer.Deserialize<AppSettings>(JsonSerializer.Serialize(dopplerClientResponse.Secrets));

// 3.3 Dictionary value access

// And of course, you can also pull a value directly from the `Secrets` dictionary
var debug = bool.Parse(dopplerClientResponse.Secrets["Debug"]);

// Output Doppler secrets in terminal for testing purposes  
new DopplerClientResponseConsoleDebugger(dopplerClientResponse, revealSecretValues: true).Write();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    new DopplerClientResponseConsoleDebugger(dopplerClientResponse).Write();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();