using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PizzaCosmosDBConApp.Models;
using PizzaCosmosDBConApp.Services;
using static System.Console;

namespace PizzaCosmosDBConApp
{
  class Program
  {
    static IConfiguration BuildConfiguration(string[] args)
    {
      IConfiguration configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddEnvironmentVariables()
      .AddCommandLine(args)
      .Build();
      return configuration;
    }

    static ServiceProvider ConfigureService(IConfiguration configuration)
    {
      ServiceCollection services = new ServiceCollection();
      services.Configure<CosmosUtility>(cfg =>
      {
        cfg.CosmosEndpoint = configuration["cosmosConnectionString:cosmosEndpoint"];
        cfg.CosmosKey = configuration["cosmosConnectionString:cosmosKey"];
      });
      services.AddSingleton<Cosmos>();
      services.AddScoped<DBService>();
      services.AddScoped<SPService>();
      services.AddScoped<TriggerService>();
      services.AddScoped<UDFService>();
      return services.BuildServiceProvider();
    }
    static async Task Main(string[] args)
    {
      IConfiguration configuration = BuildConfiguration(args);
      ServiceProvider serviceProvider = ConfigureService(configuration);

      await DatabaseCalls(configuration, serviceProvider);
      // await StoredProcedureCalls(configuration, serviceProvider);
      // await TriggerCalls(configuration, serviceProvider);
      // await UDCalls(configuration, serviceProvider);
    }

    private static async Task UDCalls(IConfiguration configuration, ServiceProvider serviceProvider)
    {
      var databaseId = "pizzaDB"; var containerId = "pizzaHut";
    }

    private static async Task TriggerCalls(IConfiguration configuration, ServiceProvider serviceProvider)
    {
      var databaseId = "pizzaDB"; var containerId = "pizzaHut";
    }

    private static async Task StoredProcedureCalls(IConfiguration configuration, ServiceProvider serviceProvider)
    {
      var databaseId = "pizzaDB"; var containerId = "pizzaHut";
    }

    private static async Task DatabaseCalls(IConfiguration configuration, ServiceProvider serviceProvider)
    {
      DBService dbService = serviceProvider.GetService<DBService>();
      dbService.SetIds(databaseId: "pizzaDB", containerId: "pizzaHut", partitionName: "pizzaType");
      // await dbService.CreateDatabase();
      // await dbService.CreateContainer();

      dbService.DatabaseName();
      dbService.ContainerName();

      await dbService.GetAllDatabases();
      await dbService.GetAllContainers();
    }
  }
}
