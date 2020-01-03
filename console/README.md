# Included

This sample incorporates the following features into a dotnet console application:

-   Configuration using DotEnv
-   Dependency Injection
-   Unit Testing using xUnit
-   Mocking using Moq
-   Logging using a custom console logger
-   Telemetery using App Insights

This should serve as a starting point for console applications that are common components of microservices solutions.

## Configuration

DotEnv is used for configuration management, so you can create a ".env" file in the folder you are running from. It can contain any of the following:

-   LOG_LEVEL: Can be set to Critical, Error, Trace, Debug, Information, Warning, or None and determines the minimum logging level.
-   DISABLE_COLORS: Can be set to "true" if you want suppress the logging messages being in different colors; this is particularly useful when looking at Docker logs.
-   APPINSIGHTS_KEY: You must set this to the instrumentation key for AppInsights.

Note that running "dotnet test" executes in a sub-folder, so the path has to be changed to look for .env in the root folder using the following:

```c#
string path = AppDomain.CurrentDomain.BaseDirectory.Split("/bin/")[0];
DotEnv.Config(false, path + "/.env");
```

## Application Insights

An environmental variable must be set as APPINSIGHTS_KEY. On a "dotnet run", the DoWork() step will log as a "request" in AppInsights.

## Unit Testing

Unit Tests in this sample make use of xUnit (https://xunit.net) and Moq (https://github.com/Moq/moq4/wiki/Quickstart).

Since this project is a dotnet console application, you must include the following in the .csproj file for xUnit to work:

```xml
<PropertyGroup>
    <GenerateProgramFile>false</GenerateProgramFile>
</PropertyGroup>
```

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

info 1/3/2020 8:49:21 PM real write to database.
info 1/3/2020 8:49:21 PM flushing (waiting for 5 seconds)...
info 1/3/2020 8:49:27 PM flushed.
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
