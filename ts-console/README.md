# Included

This sample incorporates the following features into a TypeScript console application:

-   Configuration using DotEnv
-   Dependency Injection
-   Unit Testing using Mocha and Chai
-   Mocking using TypeMoq
-   Logging using a custom console logger
-   Telemetery using App Insights
-   TSLint for linting
-   Prettier for styling

This should serve as a starting point for microservices-based solutions.

## Installation

To install all components into a new project, you can run the following:

```bash
npm init
npm install --save applicationinsights dotenv findup-sync
npm install --save tsyringe uuidv4 winston reflect-metadata
npm install --save-dev typescript tslint prettier
npm install --save-dev @types/node @types/findup-sync
npm install --save-dev mocha @types/mocha mocha-typescript
npm install --save-dev chai @types/chai
npm install --save-dev nyc typemoq source-map-support
```

You might also setup the following scripts in the package.json file:

```json
"scripts": {
"prestart": "tsc",
"start": "node dist/main.js",
"pretest": "tsc",
"test": "nyc mocha dist/test.js",
"watch": "mocha-typescript-watch",
"prepare": "tsc"
}
```

You can copy the tsconfig.json and tslint.json files to keep the same configuration.

## Configuration

https://www.npmjs.com/package/dotenv

DotEnv is used for configuration management, so you can create a ".env" file in the folder you are running from. It can contain any of the following:

-   LOG_LEVEL: Can be set to Critical, Error, Trace, Debug, Information, Warning, or None and determines the minimum logging level.
-   DISABLE_COLORS: Can be set to "true" if you want suppress the logging messages being in different colors; this is particularly useful when looking at Docker logs.
-   APPINSIGHTS_KEY: [REQUIRED] You must set this to the instrumentation key for AppInsights.

## Dependency Injection

https://github.com/microsoft/tsyringe

This project uses tsyringe from Microsoft to support dependency injection.

## Unit Testing

https://mochajs.org/
https://www.chaijs.com/
https://www.npmjs.com/package/mocha-typescript
https://www.npmjs.com/package/nyc

This project uses Mocha (unit testing framework), Mocha TypeScript (integration for TypeScript), Chai (assertions), and Istanbul (code coverage).

### Mocking

https://github.com/florinn/typemoq

To build mock objects, this solution uses TypeMoq.

### Running the application

```
> npm start

> ts-console@1.0.0 prestart /Users/plasne/Documents/samples/ts-console
> tsc


> ts-console@1.0.0 start /Users/plasne/Documents/samples/ts-console
> node dist/main.js

dbug 2020-01-07T20:21:15.547Z wrote to real database.
info 2020-01-07T20:21:15.549Z written as "43fbf4df-bc55-4977-b219-7d671639b954".
info 2020-01-07T20:21:15.550Z waiting on telemetry to flush...
```

### Testing the application

```
> npm test

> ts-console@1.0.0 pretest /Users/plasne/Documents/samples/ts-console
> tsc

> ts-console@1.0.0 test /Users/plasne/Documents/samples/ts-console
> nyc mocha dist/test.js

  Process Tests
    ✓ can process

  1 passing (8ms)

--------------|---------|----------|---------|---------|-------------------
File          | % Stmts | % Branch | % Funcs | % Lines | Uncovered Line #s
--------------|---------|----------|---------|---------|-------------------
All files     |     100 |      100 |     100 |     100 |
 Processor.ts |     100 |      100 |     100 |     100 |
 test.ts      |     100 |      100 |     100 |     100 |
--------------|---------|----------|---------|---------|-------------------
```

## Logging

The custom console logger provider here is done to...

-   show a more complex Dependency Injection scenario
-   provide abstraction for logging
-   to provide a consistent logging format between Node and dotnet.

## Application Insights

https://www.npmjs.com/package/applicationinsights

An environmental variable must be set as APPINSIGHTS_INSTRUMENTATIONKEY. On a "npm start", a "request" will be logged to AppInsights.

You should check the logs for type "requests".

## Linting and Styling

https://palantir.github.io/tslint/
https://prettier.io/

This sample uses TSLint (linting) and Prettier (styling).

In addition, you should install the following extentions for VSCode:

-   https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-typescript-tslint-plugin
-   https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode

You should also consider adding Prettier as the default formatter for certain types in VSCode:

```json
{
    "[javascript]": {
        "editor.defaultFormatter": "esbenp.prettier-vscode"
    },
    "[typescript]": {
        "editor.defaultFormatter": "esbenp.prettier-vscode"
    }
}
```
