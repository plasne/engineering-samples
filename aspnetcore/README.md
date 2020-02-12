# Included

This sample incorporates the following features into an aspnetcore application:

-   Configuration using DotEnv
-   Dependency Injection
-   CORS
-   Unit Testing using xUnit
-   Mocking using Moq
-   Logging using a custom console logger
-   Telemetery using App Insights

This should serve as a starting point for aspnetcore applications that are hosted in Azure App Service, Kubernetes, or anywhere else.

## Installation

To install all components into a new project, you can run the following:

```bash
dotnet new webapi
dotnet add package dotenv.net
dotnet add package Microsoft.ApplicationInsights.AspNetCore
dotnet add package Moq
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
dotnet restore
```

You can probably get rid of the unneeded project files (settings will be provided via environment variable and Kestrel should be used to run the solution):

```bash
rm appsettings*.json
rm -r Properties
```

## Configuration

https://github.com/bolorundurowb/dotenv.net

DotEnv is used for configuration management, so you can create a ".env" file in the folder you are running from. It can contain any of the following:

-   LOG_LEVEL: Can be set to Critical, Error, Trace, Debug, Information, Warning, or None and determines the minimum logging level.
-   DISABLE_COLORS: Can be set to "true" if you want suppress the logging messages being in different colors; this is particularly useful when looking at Docker logs.
-   HOST_URL: If set, this will determine what names and ports the solution is hosted on. Despite the name, you can use a semicolon to separate multiple entries. For hosting behind a gateway (which is common), you might use http://*:80 and allow the gateway to terminate SSL.
-   ALLOWED_ORIGINS: In order for CORS to work, you must specify a semicolon-delimited list of origins to allow via CORS. For example, "http://localhost:5000;https://sample.plasne.com" would allow for both testing locally and using a published domain.
-   ASPNETCORE_ENVIRONMENT: You can set this to "Development", "Staging", or "Production".
-   APPINSIGHTS_KEY: If you intend to use AppInsights, you must set this instrumentation key.

## CORS

https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1

The implementation of CORS can be seen in the code, however, if you trying to test using cURL or Postman, you will need to do the following:

-   Origin:http://localhost:5000 (matching whatever you used in ALLOWED_ORIGINS)

For OPTIONS preflight requests, you must also include:

-   Access-Control-Request-Method:GET (matching whatever method you intend to use)

This sample shows enabling CORS on all requests, however, if you want to be more selective, you might use endpoint routing or the [EnableCors] / [DisableCors] attributes:

-   https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1#enable-cors-with-endpoint-routing

-   https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1#enable-cors-with-attributes

Be aware that with with aspnetcore 3.1 and higher you should now specify the middleware in addition to using [EnableCors] / [DisableCors]: https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-3.1&tabs=visual-studio#cors. If you want to restrict CORS except on specific endpoints, you could simply not define a default policy like so...

```c#
services.AddCors(options =>
    {
        options.AddPolicy("AllowedOrigins", builder =>
        {
            builder.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
            if (!allowedOrigins.Contains("*")) builder.AllowCredentials();
        });
    });
```

...and then decorate methods or controllers with [EnableCors("AllowedOrigins")].

## HttpClient

Per https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests, it is important to use an IHttpClientFactory for all HTTP/S requests.

If your network requires a proxy, you can use the ProxyHandler, which will use PROXY, HTTP_PROXY, or HTTPS_PROXY as a URL to send the traffic through.

This sample show configuring a typed HttpClient for AppConfig in the Startup.cs...

```c#
public void ConfigureServices(IServiceCollection services)
{

    // add HttpClient (could be typed or named)
    services
        .AddHttpClient<AppConfig>()
        .ConfigurePrimaryHttpMessageHandler(() => new ProxyHandler());

    // add configuration
    services.AddSingleton<AppConfig, AppConfig>();

}
```

...then in the constructor for AppConfig, you can receive the client like this...

```c#
public class AppConfig
{

    public AppConfig(ILogger<AppConfig> logger = null, HttpClient httpClient = null, IHttpClientFactory httpClientFactory = null)
    {
        this.Logger = logger;
        this.HttpClient = httpClient ?? httpClientFactory?.CreateClient("config");
    }
}
```

Note that the above code would also work for a named HttpClient called "config".

## Unit Testing

https://xunit.net
https://github.com/Moq/moq4/wiki/Quickstart

The Unit Test in this sample makes use of xUnit (testing) and Moq (mocking).

You must include the following in the .csproj file for xUnit to work:

```xml
<PropertyGroup>
    <GenerateProgramFile>false</GenerateProgramFile>
</PropertyGroup>
```

In addition, this plug-in should be installed for VSCode:
https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer

The common library includes some extensions that are helpful for unit testing controllers. Consider the following service which needs to be tested. You need to make sure you return an ActionResult...

```c#
[HttpGet]
public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
{
    var list = await this.WeatherResolver.GetForecast();
    return Ok(list);
}
```

...then in your unit test you can use the StatusCode() and Body() extensions like so...

```c#
using (var provider = services.BuildServiceProvider())
{
    using (var scope = provider.CreateScope())
    {
        var controller = scope.ServiceProvider.GetService<Controllers.WeatherForecastController>();
        var result = await controller.Get();
        Assert.Equal(200, result.StatusCode()); // <-- .StatusCode() extension returns the HTTP Status Code
        var list = result.Body(); // <-- .Body() extension returns the typed body, by contrast .Value is always null
        Assert.Equal(5, list.Count());
    }
}
```

### Running the application

```
> dotnet run

dbug 1/6/2020 4:03:04 PM Hosting starting
info 1/6/2020 4:03:04 PM LOG_LEVEL = 'Debug'
info 1/6/2020 4:03:04 PM DISABLE_COLORS = 'False'
info 1/6/2020 4:03:04 PM HOST_URL = ''
info 1/6/2020 4:03:04 PM ALLOWED_ORIGINS = 'http://localhost:5000;http://bogus.com'
info 1/6/2020 4:03:04 PM ASPNETCORE_ENVIRONMENT = 'Development'
info 1/6/2020 4:13:25 PM APPINSIGHTS_INSTRUMENTATIONKEY = '(set)'
dbug 1/6/2020 4:03:04 PM Developer exception page will be used.
dbug 1/6/2020 4:03:04 PM Failed to locate the development https certificate at '(null)'.
dbug 1/6/2020 4:03:04 PM No listening endpoints were configured. Binding to http://localhost:5000 and https://localhost:5001 by default.
dbug 1/6/2020 4:03:04 PM Hosting started
dbug 1/6/2020 4:03:04 PM Loaded hosting startup assembly aspnetcore
Hosting environment: Development
Content root path: /Users/plasne/Documents/samples/aspnetcore
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

### Testing the application

```
> dotnet test

Microsoft (R) Test Execution Command Line Tool Version 16.3.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...

A total of 1 test files matched the specified pattern.

Test Run Successful.
Total tests: 1
     Passed: 1
 Total time: 1.3221 Seconds
```

## Logging

I use a custom console logger because the out-of-box logger is very slow. In addition, the logger I wrote put all messages on a single line instead of two, which makes verbose logs much easier to read.

If you want to enable logging for hosting in App Services, you also have to follow these instructions: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#azure-app-service-provider.

## Application Insights

https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core

An environmental variable must be set as APPINSIGHTS_INSTRUMENTATIONKEY. Querying the /weatherforecast endpoint will log the request to App Insights.

You should check the logs for type "requests" or you can use the Live Metrics Stream.

NOTE: with the aspnetcore implementation, there is no requirement to Flush(); frankly I am not sure how that is addressed.

More details can be found here: https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core.

Implementation involves the following in Program.cs...

```c#
public static IWebHostBuilder CreateHostBuilder(string[] args)
{
    var builder = WebHost.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            // NOTE: you must create a filter that says what logs to send to AppInsights, in this case,
            //   I chose the same log level as my console logger
            logging.AddFilter<ApplicationInsightsLoggerProvider>("", AddSingleLineConsoleLoggerConfiguration.LogLevel);
        })
        .UseStartup<Startup>();
    if (HostUrl != null) builder.UseUrls(HostUrl);
    return builder;
}
```

...and the following in Startup.cs...

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpContextAccessor(); // required for x-correlation header
    services.AddApplicationInsightsTelemetry();
    services.AddSingleton<ITelemetryInitializer, CorrelationToTelemetry>();
}
```

This configuration will log:

-   All requests
-   All ILogger messages as traces
-   System metrics (CPU, memory, etc.)

It is often useful to trace a request from the client, through the API, through the microservices, and back. If you make a request with an "x-correlation" header, that header will be used as operation_Id on all requests and traces.
