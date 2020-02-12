# engineering-samples

A set of samples using our CSE engineering fundamentals.

The following samples are included:

-   aspnetcore: A sample of an aspnetcore application
-   dotnet-console: A sample of a dotnet console application
-   ts-console: A sample of a TypeScript console application

There are detailed README.md files in each of those folders.

## Common

The C# projects reference a common project file. There are a few things you should do with this for your project:

1. You should build your data contracts into this common library. Whenever you have multiple projects that are going to need to communicate with one another, we need to place to store those interfaces/classes.

2. You should pack the common project into a NuGet package and use a reference to the NuGet package instead of the project reference. This allows you to version the contracts and allows you to make changes without breaking other services.

You can create a nuget.config file if you want to point to a local folder with your NuGet package or a remote ADO instance...

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear/>
        <add key="local" value="/Users/plasne/Documents/Hackathon.EMShare.BackEnd/svc-tools/out/"/>
        <add key="remote" value="https://pkgs.dev.azure.com/sample/00000000-0000-0000-0000-000000000000/_packaging/sample/nuget/v3/index.json"/>
    </packageSources>
</configuration>
```

## Using App Config

The common code base allows you to pull configuration values from Azure AppConfig. Unless you change the defaults, access to the configuration store will use the AzureServiceTokenProvider which attempt to get an access_token from Managed Identity and then from the az-cli (make sure you do an "az login" first). This allows you to easily use the same code for debugging and deployment. If you do not want to use MI, you can use a service principal by setting AUTH_TYPE=app, TENANT_ID, CLIENT_ID, and CLIENT_SECRET. Regardless, to give an identity access to pull values requires the "App Configuration Data Reader" role.

There are static helper functions that can extract meaningful values from environment variables, such as these examples...

```c#
public static string[] HostUrl
{
    get
    {
        return AppConfig.GetArrayOnce("HOST_URL");
    }
}

public static string AppInsightsInstrumentationKey
{
    get
    {
        return AppConfig.GetStringOnce("APPINSIGHTS_INSTRUMENTATIONKEY");
    }
}
```

You need to add the AppConfig class using dependency injection as shown in the Startup.cs...

```c#
public void ConfigureServices(IServiceCollection services)
{

    // add logging
    services.AddSingleLineConsoleLogger();

    // add HttpClient
    services
        .AddHttpClient<AppConfig>()
        .ConfigurePrimaryHttpMessageHandler(() => new ProxyHandler());

    // add configuration
    services.AddSingleton<AppConfig, AppConfig>();

}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
{

    // load the configuration
    var logger = provider.GetService<ILogger<Startup>>();
    logger.LogInformation("Loading configuration...");
    var config = provider.GetService<AppConfig>();
    config.Apply().Wait(); // block until loaded

    // confirm and log the configuration
    // NOTE: .Require() will throw an exception if a value was not obtained
    // NOTE: both .Require() and Optional() will log using ILogger
    config.Require("HOST_URL", Program.HostUrl);
    config.Optional("APPINSIGHTS_INSTRUMENTATIONKEY", hideValue: true);

}
```

The above example showed static methods, but there are also non-static methods that allow you to get values via a more complex flow. If the environment variable is a URL to an Azure Key Vault (you can also use a Key Vault reference in App Config), the value will be read as a secret from the Key Vault. In addition, the non-static methods are cached. The .Cache dictionary is public so you can clear it if needed.

Unless you change the defaults, access to the vault will use the AzureServiceTokenProvider which attempt to get an access_token from Managed Identity and then from the az-cli (make sure you do an "az login" first). This allows you to easily use the same code for debugging and deployment. If you do not want to use MI, you can use a service principal by setting AUTH_TYPE=app, TENANT_ID, CLIENT_ID, and CLIENT_SECRET. Regardless, to give an identity access to pull values requires a "GET SECRET" policy.

```c#
var config = provider.GetService<AppConfig>();
var secret = await config.GetString("SECRET_FROM_KEYVAULT");
config.Optional("SECRET_FROM_KEYVAULT", secret, hideValue: true);
```

In the rare event that you need separate credentials for App Config and Key Vault, there are AUTH_TYPE_CONFIG, AUTH_TYPE_VAULT, TENANT_ID_CONFIG, CLIENT_ID_CONFIG, CLIENT_SECRET_CONFIG, TENANT_ID_VAULT, CLIENT_ID_VAULT, and CLIENT_SECRET_VAULT variables.

## Protecting Static Web Assets

You might not wish to expose your static web assets to an unauthenticated endpoint. You could require authentication to get to your static assets, but then you might need to break apart assets you want to protect and those that are benign to get the performance you need. Instead, you might consider hosting your static web assets in NGINX and using a routing rule. The one shown below checks for a "user" cookie, if it is found, the user can get to the assets, otherwise, they are redirected to an authorize endpoint.

```nginx
server {
    listen       80;
    server_name  localhost;

    # see if there is a user cookie
    set $user 0;
    if ($cookie_user) {
        set $user 1;
    }

    # health endpoint
    location /health {
        return 200;
    }

    # present files in static
    location / {
        if ($user = 0) {
            return 302 https://auth.plasne.com/cas/authorize;
        }
        root   /usr/share/nginx/html;
        index  index.html index.htm;
    }

    #error_page  404              /404.html;

    # redirect server error pages to the static page /50x.html
    error_page   500 502 503 504  /50x.html;
    location = /50x.html {
        root   /usr/share/nginx/html;
    }

}
```

## AKS Ingress Controller Samples

We have a managed ingress controller solution for AKS based on App Gateway. Below are some common routing templates.

Sample #1: route URLs like https://plasne.com/api/endpoint

```yaml
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
    name: api
    annotations:
        kubernetes.io/ingress.class: azure/application-gateway
        appgw.ingress.kubernetes.io/backend-path-prefix: '/'
        appgw.ingress.kubernetes.io/ssl-redirect: 'true'
spec:
    tls:
        - secretName: ssl
    rules:
        - http:
              paths:
                  - path: /api/*
                    backend:
                        serviceName: api
                        servicePort: 80
```

Sample #2: route URLs like https://api.plasne.com/endpoint

```yaml
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
    name: api
    annotations:
        kubernetes.io/ingress.class: azure/application-gateway
        appgw.ingress.kubernetes.io/backend-path-prefix: '/'
        appgw.ingress.kubernetes.io/ssl-redirect: 'true'
spec:
    tls:
        - secretName: ssl
    rules:
        - host: api.plasne.com
          http:
              paths:
                  - backend:
                        serviceName: api
                        servicePort: 80
```

Sample #3: route URLs like https://plasne.com/endpoint

```yaml
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
    name: portal
    annotations:
        kubernetes.io/ingress.class: azure/application-gateway
        appgw.ingress.kubernetes.io/backend-path-prefix: '/'
        appgw.ingress.kubernetes.io/ssl-redirect: 'true'
spec:
    tls:
        - secretName: ssl
    rules:
        - http:
              paths:
                  - path: /*
                    backend:
                        serviceName: portal
                        servicePort: 80
```
