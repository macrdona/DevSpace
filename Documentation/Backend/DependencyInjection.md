# Dependency Injection

## AddSingleton()

- AddSingleton() creates a single instance of the service when it is first requested and reuses that same instance in all the places where that service is needed.

```
//registering MongoClient and adding it to DI container
services.AddSingleton<IMongoClient, MongoClient>(conn => new MongoClient(builder.Configuration.GetSection("DatabaseConnection:ConnectionString").Value));
```

## AddScoped()

- In a scoped service, with every HTTP request, we get a new instance. However, within the same HTTP request, if the service is required in multiple places, like in the view and in the controller, then the same instance is provided for the entire scope of that HTTP request. But every new HTTP request will get a new instance of the service.

```
//instance are the same within a request, but different accross different requests
services.AddScoped<IAccountServices, AccountsServices>();
services.AddScoped<IJwtUtils, JwtUtils>();
```

## AddTransient()

- With a transient service, a new instance is provided every time a service instance is requested whether it is in the scope of the same HTTP request or across different HTTP requests.

## Registering Configurations

- This section binds the `DatabaseSettings` data model to the data stored in secrets.json, and registers the configuration in the Dependancy Injection container

```
var databaseSettings = builder.Configuration.GetSection("DatabaseSettings");
services.Configure<DatabaseSettings>(databaseSettings);
```
