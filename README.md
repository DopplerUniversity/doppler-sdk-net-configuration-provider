# ASP.NET Core SecretOps DopplerClient

The [DopplerClient](./DopplerClient/DopplerClient.cs) is designed to fetch Doppler secrets before app initialization to inject values into the configuration store, providing similar functionality to a built-in Configuration Provider.
 
> NOTE: We are working on a custom Configuration Provider (and Service) that will utilize this DopplerClient but we're actively seeking feedback on the DopplerClient itself, hence this example repository.

## Setup

Currently, these instructions are designed for macOS and Linux environments but will be updated soon to better support Windows.

Import the sample project to Doppler:

```sh
doppler import
```

Select the config to use:

```sh
doppler setup --project dotnet-core-webapp --config dev
```

Confirm the Doppler CLI can fetch secrets for that config:

```sh
doppler secrets
```

## Run

The [Program.cs](./Program.cs) has inline documentation about the DopplerClient apart from the code itself, is the best way to learn the DopplerClient can be used.

A `DopplerToken` value is required and if when not debugging, you can use the Doppler CLI to dynamically inject an ephemeral Service Token as an environment variable in:

```sh
DopplerToken="$(doppler configs tokens create  dev --max-age 1m --plain)" dotnet run
```

If you are wanting to debug your application, an environment variable may not be the easiest thing to setup.

To avoid hard-coding the "DopplerToken" value in your launchSettings.json or other JSON config file that could accidentally get committed to source control, can instead store the Service Token in a local development only config file

```c#
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    // Ensure this file is in your .gitignore
    config.AddJsonFile("dopplerclientconfig.Development.json", optional: true);
});
```

Then you can run a one-time setup command to create the JSON file using the Doppler CLI:

```sh
echo "{ \"DopplerToken\": \""$(doppler configs tokens create dev --plain)"\" }" > dopplerclientconfig.Development.json
```

What the right approach will be may differ from team to team, but as long as you've done everything to avoid hard-coding the Service Token where it may end up where it's not supposed to, you're on the right track.

## Feedback Welcome!

This is our initial exploration into building an Doppler with ASP.NET core and we are currently building out our own Doppler configuration provider which will make it possible to hydrate classes without the Doppler CLI.