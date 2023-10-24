using System;
using Microsoft.OpenApi.Models;
using SearchEngine.API.Interfaces;
using SearchEngine.API.Services;

namespace SearchEngine.API.Extensions;

/// <summary>
/// This static class will be used by the program.cs to configure the needed services via ServiceCollections.
/// It will add and configure controllers, DI services, CORS policies and dev API tester Swagger.
/// </summary>
public static class ServiceCollections
{
    /// <summary>
    /// Configuring controllers for the API
    /// As this is a WebAPI project, we will use the AddControllers service.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers();
    }
    /// <summary>
    /// Adding services to the DI container.
    /// The LuceneSearchEngineService will be registered in the DI container to be injected to the required classes.
    /// Singleton method is selected as the lifetime of the LuceneSearchEngineService to be used throughout the lifetime of the app.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<ILuceneSearchEngineService, LuceneSearchEngineService>();
    }
    /// <summary>
    /// Configuring CORS for the permission of access to the API via other services.
    /// AllowAnyOrigin will be used but it must be configured in a production environment.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureCORS(this IServiceCollection services)
    {
        services.AddCors(policy =>
        {
            policy.AddPolicy("CorsPolicy", opt => opt
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
    }
    /// <summary>
    /// Adding and configuring the Swagger and the EndpointsApiExplorer services
    /// Properties of the Swagger instance can be set if needed.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Search Engine",
                Version = "v1",
                Description = "Lucene Search Engine Service: API Microservice",
                Contact = new OpenApiContact
                {
                    Name = "Iman Janghorban",
                    Email = "iman.janghorban5@gmail.com"
                },
            });
        });
    }
}
