# Doppler .NET SDK - Configuration Provider

The [DopplerClient](./DopplerSDK.ConfigurationProvider/src/DopplerClient.cs) is designed to fetch Doppler secrets before
app initialization to inject values into the configuration store, providing similar functionality to a built-in
Configuration Provider.

Injecting secrets into the configuration data store enables the use of
the [Options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0)
to provide strongly typed access to application settings classes and individual values.

A custom Doppler configuration provider and service will also be implemented in the coming weeks which will remove the
boilerplate required fetch and inject secrets using the `DopplerClient` directly.

## Purpose

At this stage, this repository is designed for reviewing and testing the Doppler Client before making it available as a
NuGet package.

To test it in one of your projects, simply copy
the [DopplerClient.cs](./DopplerSDK.ConfigurationProvider/src/DopplerClient.cs) (and
optionally [DopplerClientResponseDebugger.cs](./DopplerSDK.ConfigurationProvider/src/DopplerClientResponseDebugger.cs))
into your solution and use the example code in [Program.cs](./SampleApp/Program.cs) as a starting point.

We are also working on a custom Configuration Provider (and Service) that will utilize this DopplerClient.

The remaining steps in this guide will show you how to use this repository for local testing.

## Usage

Take a look at the [SampleApp](./SampleApp) usage examples.
