# App Secrets

- This sample shows how to add app secrets to your application.
- NOTE: The Secret Manager tool doesn't encrypt the stored secrets and shouldn't be treated as a trusted store. It's for development purposes only. The keys and values are stored in a JSON configuration file in the user profile directory.

# Steps

1. Create secrets.json file `dotnet user-secrets init`
2. Open secrets.json file or run command `dotnet user-secrets set "key" "value"`
3. Access secrets from your program `string connectionUri = builder.Configuration["key"];`

- More information here: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#read-the-secret-via-the-configuration-api
