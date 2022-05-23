# ASP.NET Core SecretOps DopplerClient

The [DopplerClient](./SecretOps/Doppler/DopplerClient.cs) is designed to fetch Doppler secrets before app initialization to inject values into the configuration store, providing similar functionality to a built-in Configuration Provider.

We've designed our Doppler client to seamlessly work with ASP.NET core's configuration provider system so you can take advantage of the [Options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0) to provide strongly typed access to application settings classes and individual values.

## Purpose

At this stage, this repository is designed for reviewing and testing the functionality of the Doppler Client before the next step of making it available as a NuGet package.

To test it in one of your projects, simply copy the [DopplerClient.cs](./SecretOps/Doppler/DopplerClient.cs) (and optionally [DopplerClientResponseDebugger.cs](./SecretOps/Doppler/DopplerClientResponseDebugger.cs)) into your solution and use the example code in [Program.cs](./DopplerClientSampleApp/Program.cs) as a starting point.
 
We are also working on a custom Configuration Provider (and Service) that will utilize this DopplerClient.

The remaining steps in this guide will show you how to use this repository for local testing.

> NOTE: Currently only ASP.NET Core is supported.

## Prerequisites

- ASP.NET Core
- [Doppler CLI](https://docs.doppler.com/docs/install-cli) installed

## Usage 

Take a look at the [DopplerClientSampleApp](./DopplerClientSampleApp) for usage examples.

## Feedback Welcome!

This is our initial exploration into building an Doppler with ASP.NET core and we are currently building out our own Doppler configuration provider which will make it possible to hydrate classes without the Doppler CLI.