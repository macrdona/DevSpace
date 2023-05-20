using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;
using backend.Helpers;
using backend.Services;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            { //registering application services
                var services = builder.Services;
                var environment = builder.Environment;

                services.AddControllers();

                //allow calls from external origins
                services.AddCors();

                /*This section binds the `DatabaseSettings` object to the data stored in appsettings.json, 
                 * and registers the configuration in the Dependancy Injection container
                 */
                var databaseSettings = builder.Configuration.GetSection("DatabaseSettings");
                services.Configure<DatabaseSettings>(databaseSettings);

                //instance are the same within a request, but different accross different requests
                services.AddScoped<IAccountServices, AccountsServices>();

                //allows mapping data from one object to another
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            }
            

            var app = builder.Build();

            { //configuration of HTTP request pipeline

                app.UseCors(x => x 
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

                //custome application error handling
                app.UseMiddleware<ErrorHandlerMiddleware>();

                app.MapControllers();
            }

            app.Run("http://localhost:4000");
        }

    }
}