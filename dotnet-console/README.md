# Included

This sample incorporates the following features into a dotnet console application:

-   Configuration using DotEnv and/or AppConfig
-   Dependency Injection
-   Unit Testing using xUnit
-   Mocking using Moq
-   Logging using a custom console logger
-   Telemetery using App Insights

This should serve as a starting point for console applications that are common components of microservices solutions.

## Installation

To install all components into a new project, you can run the following:

```bash
dotnet new console
dotnet add package dotenv.net
dotnet add package Microsoft.ApplicationInsights
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Moq
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
dotnet restore
```

## Configuration

https://github.com/bolorundurowb/dotenv.net

DotEnv is used for configuration management, so you can create a ".env" file in the folder you are running from. It can contain any of the following:

-   LOG_LEVEL: Can be set to Critical, Error, Trace, Debug, Information, Warning, or None and determines the minimum logging level.
-   DISABLE_COLORS: Can be set to "true" if you want suppress the logging messages being in different colors; this is particularly useful when looking at Docker logs.
-   APPINSIGHTS_INSTRUMENTATIONKEY: You must set this to the instrumentation key for AppInsights.

## HttpClient

It is important to use the IHttpClientFactory for all HTTP/S calls. The pattern that is implemented in this sample comes from our recommendation here: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1#use-ihttpclientfactory-in-a-console-app.

## Unit Testing

https://xunit.net
https://github.com/Moq/moq4/wiki/Quickstart

The Unit Test in this sample makes use of xUnit (testing) and Moq (mocking).

```xml
<PropertyGroup>
    <GenerateProgramFile>false</GenerateProgramFile>
</PropertyGroup>
```

In addition, this plug-in should be installed for vscode:
https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer

### ProgramFake

This sample uses FakeDatabaseWriter which inherited from IDatabaseWriter and implemented via Dependency Injection so that Unit Tests could be conducted without connecting to a database.

There are some cases where this approach might make sense:

-   The object inherits from a base class with logic that you need to keep intact for testing.
-   The object is complex - using this method might make it easier to understand.

### ProgramMock

This sample uses Moq to create a mock implementation of an object based on IDatabaseWriter. There is only a single Write() method that needs to be overridden, so this is a perfect example of an object that is mockable. The sample still uses Dependency Injection, but creates the mocked object as a function of AddTransient.

This is perhaps the most common method that should be used.

### ProgramMockCollection

This sample uses the CollectionFixture feature of xUnit to create a Singleton object using Moq that can be shared across any number of Unit Test classes.

Make note that the object is a Singleton.

### Running the application

```
> dotnet run

info 1/6/2020 4:14:46 PM LOG_LEVEL = 'Trace'
info 1/6/2020 4:14:46 PM DISABLE_COLORS = 'False'
info 1/6/2020 4:14:46 PM APPINSIGHTS_INSTRUMENTATIONKEY = '(set)'
dbug 1/6/2020 4:14:46 PM appinsights: StartOperation()
dbug 1/6/2020 4:14:46 PM appinsights: StopOperation()
info 1/6/2020 4:14:46 PM real write to database.
info 1/6/2020 4:14:46 PM flushing (waiting for 5 seconds)...info 1/3/2020 8:49:27 PM flushed.
info 1/3/2020 8:49:27 PM real dispose.
```

### Testing the application

```
> dotnet test

Microsoft (R) Test Execution Command Line Tool Version 16.3.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...

A total of 1 test files matched the specified pattern.
info 1/3/2020 8:45:17 PM fake write to database.
info 1/3/2020 8:45:17 PM fake dispose.
info 1/3/2020 8:45:17 PM mock-collection write to database.
info 1/3/2020 8:45:17 PM mock write to database.
info 1/3/2020 8:45:17 PM mock-collection dispose.
info 1/3/2020 8:45:17 PM mock dispose.

Test Run Successful.
Total tests: 3
     Passed: 3
 Total time: 0.9314 Seconds
```

## Logging

I use a custom console logger because the out-of-box logger is very slow. In addition, the logger I wrote put all messages on a single line instead of two, which makes verbose logs much easier to read.

## Application Insights

https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core

An environmental variable must be set as APPINSIGHTS_INSTRUMENTATIONKEY. On a "dotnet run", the DoWork() step will log as a "request" in AppInsights.

You should check the logs for type "requests".
