# Doppler Client Sample App

## Prerequisites

- ASP.NET Core
- [Doppler CLI](https://docs.doppler.com/docs/install-cli) installed

## Setup

Import the sample project to Doppler which has the data in the required structured format to be bound to
the [AppSettings.cs](./AppSettings.cs) class.

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

Then open the Project in the dashboard to take a look at how the secrets would be managed:

```sh
doppler open dashboard
```

## Sample App Overview

The [Program.cs](./Program.cs) in the [DopplerClientSampleApp](./) projects has inline documentation about how to use
the DopplerClient as should be your starting place for experimentation.

A `DopplerToken` value is required to configure the [DopplerClientConfiguration](./SecretOps/Doppler/DopplerClient.cs]
instance passed to the [DopplerClient](./SecretOps/Doppler/DopplerClient.cs) constructor.

If debugging is not required, you can use the Doppler CLI to dynamically inject an ephemeral Service Token as an
environment variable and launch the app from the terminal:

```sh
DopplerToken="$(doppler configs tokens create  dev --max-age 1m --plain)" dotnet run
```

But if wanting to debug, then you'll need to inject the `DopplerToken` value via one of the existing configuration
providers such as environment variables or JSON settings files.

Using an environment variable is your best bet but if that's not an option, avoid locations such
as `launchSettings.json` and `appSettings.Development.json` as they could accidentally get committed to source control.

Instead, our current advice is to use a local development only config file that is unique to your and your development
environment such as `dopplerClientConfig.Development.json` which you would then add to your `.gitignore` file.

Open a terminal window and use the Doppler CLI to generate this local settings file by running:

```sh
echo "{ \"DopplerToken\": \""$(doppler configs tokens create dev --plain)"\" }" > dopplerClientConfig.Development.json
```

We encourage you to chat with your team about the best way of supplying the `DopplerToken` value as this is just one
option. As long as you've done everything to avoid hard-coding the Service Token so it doesn't end up in source control
or somewhere else it's not supposed to, you're on the right track.

Now run/debug the application and you'll be taken to the index page which displays the secrets so you can see the value
binding in action.

![](https://user-images.githubusercontent.com/133014/169822956-d1c0db1e-cd3c-4331-8caf-79e458768416.png)
