using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Web.Interfaces;
using Web.Services;

[assembly: FunctionsStartup(typeof(Web.Startup))]

namespace Web
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<SqlDbContext>(
                options => SqlServerDbContextOptionsExtensions.UseSqlServer(
                    options, Environment.GetEnvironmentVariable("MSSQLDB_CONNECTION")));

            // 3rd parties
            builder.Services.AddAutoMapper(Assembly.GetAssembly(this.GetType()));
            builder.Services.AddSingleton<ICosmosDbClientFactory>(x => new CosmosDbClientFactory(
                new CosmosDbSetting(
                    Environment.GetEnvironmentVariable("COSMOSDB_URI"),
                    Environment.GetEnvironmentVariable("COSMOSDB_KEY"),
                    Environment.GetEnvironmentVariable("COSMOSDB_DATABASENAME")
                )
            ));
            builder.Services.Configure<TelemetryConfiguration>((o) =>
            {
                o.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
                o.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            });
            builder.Services.Configure<BlobStorageOptions>((o) =>
            {
                o.ConnectionString = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION");
                o.Containers = new Dictionary<string, string> { { "your_container_key", "your_container_name" } };
            });
            builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

            // Our services
            builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped<ISampleRepository, SampleRepository>();
            builder.Services.AddScoped<ITemplateCosmosRepository, TemplateCosmosRepository>();
            builder.Services.AddScoped<ISampleViewModelService, SampleViewModelService>();
        }
    }
}
