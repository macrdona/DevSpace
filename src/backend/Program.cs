using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;
using backend.Helpers;
using backend.Services;
using System.Net.Mime;
using backend.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using backend.Helpers.Wrappers;
using backend.Models;
using AutoMapper;

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

                //registering service for overriding API ModelState error response
                services.AddControllers().ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new ValidationFailedResult(context.ModelState);

                        //responses are formatted to JSON
                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        return result;
                    };
                });

                //allow calls from external origins
                services.AddCors();

                var appSettings = builder.Configuration.GetSection("AppSettings");
                services.Configure<AppSettings>(appSettings);

                //registering MongoDB Database and adding it to DI container
                services.AddSingleton<IMongoDatabaseWrapper, MongoDatabaseWrapper>(conn =>
                {
                    var client = new MongoClient(builder.Configuration.GetSection("DatabaseConnection:ConnectionString").Value);
                    var database = client.GetDatabase(builder.Configuration.GetSection("DatabaseSettings:Database").Value);
                    return new MongoDatabaseWrapper(database);  
                });

                //instance are the same within a request, but different accross different requests
                services.AddScoped<IAccountServices, AccountsServices>();
                services.AddScoped<IJwtUtils, JwtUtils>();
                services.AddScoped<IBCryptWrapper, BCryptWrapper>();

                //allows mapping data from one object to another
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                //allowing use of HTTPContext in other classes
                services.AddScoped<IUrlHelper>(factory =>
                {
                    var actionContext = factory.GetService<IActionContextAccessor>()
                                               .ActionContext;
                    return new UrlHelper(actionContext);
                });
                services.AddHttpContextAccessor();
            }
            

            var app = builder.Build();

            { //configuration of HTTP request pipeline

                app.UseCors(x => x 
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

                //custome application error handling
                app.UseMiddleware<ErrorHandlerMiddleware>();

                //custorm jwt authorization handler
                app.UseMiddleware<JwtMiddleware>();

                app.UseAuthorization();

                app.MapControllers();
            }

            app.Run("http://localhost:4000");
        }

    }
}