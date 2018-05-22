![Logo](https://raw.githubusercontent.com/renardguill/KongRegister/master/resources/KongRegisterLogo.png)

# KongRegister

[![License](https://img.shields.io/github/license/renardguill/KongRegister.svg)](https://github.com/renardguill/KongRegister/blob/master/LICENSE) [![NuGet version](https://img.shields.io/nuget/v/KongRegister.svg)](https://www.nuget.org/packages/KongRegister/) [![Travis build (release)](https://img.shields.io/travis/renardguill/KongRegister/master.svg?label=build%20(release)&logo=travis)](https://travis-ci.org/renardguill/KongRegister) [![Travis build (develop)](https://img.shields.io/travis/renardguill/KongRegister/develop.svg?label=build%20(develop)&logo=travis)](https://travis-ci.org/renardguill/KongRegister)

Register your .Net Core web-service in [Kong API Gateway](https://getkong.org) on startup and unregister it when you application is shutting down.

## Getting started

Install KongRegister package :

```PM
Install-Package KongRegister
```

Add the following configuration in your appsettings.json :

```json
"KongRegister": {
    "OnStartup" : true,
    "KongApiUrl": "http://YourKongApiUrl:8081",
    "KongApiKeyHeader": "YourApikeyHeader",
    "KongApiKey": "YourApiKey",
    "UpstreamId": "YourUpstreamId",
    "TargetHostDiscovery": "dynamic",
    "TargetPortDiscovery": "dynamic",
    "TargetWeight": 1000
  }
```

Add the reference in your Startup class :

```csharp
using KongRegister;
```

Add those two services in the ConfigureServices method :

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<KongRegisterConfig>(Configuration.GetSection("KongRegister"));
    services.PostConfigure<KongRegisterConfig>(kongConf =>
    {
        if (bool.TryParse(Configuration.GetValue<string>("KONGREGISTER_DISABLED"), out bool disabled))
        {
            kongConf.Disabled = disabled;
        }

        if (bool.TryParse(Configuration.GetValue<string>("KONGREGISTER_ONSTARTUP"), out bool onStratup))
        {
            kongConf.OnStartup = onStratup;
        }

    });
    services.AddSingleton<IHostedService, KongRegisterService>();
    ...
}
```

Run your application, **OK it's registered in your Kong server !**<br/>
Shutdown it, it's now unregistered from Kong !

## Compatibility

KongRegister is compatible with:

- Kong CE 0.12.x, 0.13.0
- .Net Core 2.0

## Prerequisites

- A running Kong installation
- A .Net Core 2.0 web application

## Discussions & Support

You can get help via the [issue tracker](https://github.com/renardguill/KongRegister/issues) here on GitHub

## Features

- Auto register in Kong server on startup
- Auto unregister from Kong server on shutdown
- Auto discover host IP
- Auto discover host port

## Installation

You must install KongRegister from [NuGet](https://www.nuget.org/packages/KongRegister). 

*Package manager :*

```PM
Install-Package KongRegister
```

*.Net CLI :*

```dotnet
dotnet add package KongRegister
```

## Configuration

The configuration must be define in the appsettings.json under a root section that you can named it as you choice.

*Example :*

```json
"KongRegister": {
    "OnStartup" : true,
    "KongApiUrl": "http://YourKongApiUrl:8081",
    "KongApiKeyHeader": "YourApikeyHeader",
    "KongApiKey": "YourApiKey",
    "UpstreamId": "YourUpstreamId",
    "TargetHostDiscovery": "dynamic",
    "TargetHost": "YourTargetHostNameOrIp",
    "TargetPortDiscovery": "dynamic",
    "TargetPort": 5000,
    "TargetWeight": 1000
  }
```

Or define in environments variables in uppercase and start with `KR_` followed by a parameter name. Like `KR_ONSTARTUP=True`.

See below all detailed parameters :

Parameter  | Type | Description
------------- | ------------- |-------------
`Disabled`|`Boolean`|Disble KongRegister service.
`OnStratup`|`Boolean`|Enable registering on startup.
`KongApiUrl`|`String`|Url of your Kong Admin Api. Don't forget the port if it's not standard (80 or 443).
`KongApiKeyHeader`|`String`|Name of your header that contain your API Key (if your Kong server is secured).
`KongApiKey`|`String`|Your API Key (if your Kong server is secured).
`UpstreamId`|`String`|Kong Upstream Id that you want to register your application into.
`TargetHostDiscovery`|`String`|Discovery method for your application host. Value must be `dynamic` or `static`.
`TargetHost`|`String`|Host name or IP of your application host. (if discovery method is static).
`TargetPortDiscovery`|`String`|Discovery method for your application host port. Value must be `dynamic` or `static`.
`TargetPort`|`Integer`|Port of your application host (if discovery method is static).
`TargetWeight`|`Integer`|Weight for the registered target in Kong server.

## Running

Todo

```csharp
using KongRegister;
```
