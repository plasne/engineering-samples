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

## Configuration

DotEnv is used for configuration management, so you can create a ".env" file in the folder you are running from. It can contain any of the following:

-   LOG_LEVEL: Can be set to Critical, Error, Trace, Debug, Information, Warning, or None and determines the minimum logging level.
-   DISABLE_COLORS: Can be set to "true" if you want suppress the logging messages being in different colors; this is particularly useful when looking at Docker logs.
-   HOST_URL: If set, this will determine what names and ports the solution is hosted on. Despite the name, you can use a semicolon to separate multiple entries. For hosting behind a gateway (which is common), you might use http://*:80 and allow the gateway to terminate SSL.
-   ALLOWED_ORIGINS: In order for CORS to work, you must specify a semicolon-delimited list of origins to allow via CORS. For example, "http://localhost:5000;https://sample.plasne.com" would allow for both testing locally and using a published domain.
-   ASPNETCORE_ENVIRONMENT: You can set this to "Development", "Staging", or "Production".
-   APPINSIGHTS_KEY: [REQUIRED] You must set this to the instrumentation key for AppInsights.

Note that running "dotnet test" executes in a sub-folder, so the path has to be changed to look for .env in the root folder using the following:

```c#
string path = AppDomain.CurrentDomain.BaseDirectory.Split("/bin/")[0];
DotEnv.Config(false, path + "/.env");
```

## CORS

The implementation of CORS can be seen in the code, however, if you trying to test using cURL or Postman, you will need to do the following:

-   Origin:http://localhost:5000 (matching whatever you used in ALLOWED_ORIGINS)

For OPTIONS preflight requests, you must also include:

-   Access-Control-Request-Method:GET (matching whatever method you intend to use)

## Unit Testing

The Unit Test in this sample makes use of xUnit (https://xunit.net) and Moq (https://github.com/Moq/moq4/wiki/Quickstart).

You must include the following in the .csproj file for xUnit to work:

```xml
<PropertyGroup>
    <GenerateProgramFile>false</GenerateProgramFile>
</PropertyGroup>
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

An environmental variable must be set as APPINSIGHTS_KEY. Querying the /weatherforecast endpoint will log the request to App Insights.

You should check the logs for type "requests" or you can use the Live Metrics Stream.

NOTE: with the aspnetcore implementation, there is no requirement to Flush(); frankly I am not sure how that is addressed.

More details can be found here: https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core.
