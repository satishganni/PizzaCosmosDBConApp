﻿using System;
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

      // await DatabaseCalls(configuration, serviceProvider);
      // await StoredProcedureCalls(configuration, serviceProvider);
      // await TriggerCalls(configuration, serviceProvider);
      await UDCalls(configuration, serviceProvider);
    }

    private static async Task UDCalls(IConfiguration configuration, ServiceProvider serviceProvider)
    {
      var databaseId = "pizzaDB"; var containerId = "pizzaHut";
      UDFService uDFService = serviceProvider.GetService<UDFService>();
      uDFService.SetContainer(databaseId, containerId);

      try
      {
        await uDFService.CreateUDF("ToppingsCountUDF");
        await uDFService.ExecuteUDF();
        await uDFService.ViewUserDefinedFunctions();
      }
      catch (Exception ex)
      {
        WriteLine(ex.ToString());
      }
    }

    private static async Task TriggerCalls(IConfiguration configuration, ServiceProvider serviceProvider)
    {
      var databaseId = "pizzaDB"; var containerId = "pizzaHut";
      TriggerService tService = serviceProvider.GetService<TriggerService>();
      tService.SetContainer(databaseId, containerId);
      try
      {
        await tService.ViewTriggers();

        // await tService.CreateTrigger("PriceValidationTrigger", "Pre");
        // await tService.CreateNewPizzaPre();
        // await tService.DeleteTrigger("PriceValidationTrigger");

        // await tService.CreateTrigger("PizzaTypeTrigger", "Post");
        // await tService.CreateNewPizzaPost();
        // await tService.DeleteTrigger("PriceValidationTrigger");

      }
      catch (Exception ex)
      {
        WriteLine(ex.ToString());
      }
    }

    private static async Task StoredProcedureCalls(IConfiguration configuration, ServiceProvider serviceProvider)
    {
      var databaseId = "pizzaDB"; var containerId = "pizzaHut";
      SPService spService = serviceProvider.GetService<SPService>();
      spService.SetContainer(databaseId, containerId);
      await spService.ViewStoredProcedures();
      try
      {
        // await spService.CreateStoredProcedure("Greetings");
        // await spService.ExecuteSPGreeting("Batman");
        // await spService.DeleteStoreProcedure("Greetings");

        // await spService.CreateStoredProcedure("CreateNewPizza");
        // await spService.ExecuteSPCreateNewPizza("Veg", "107TandooriPaneer");
        // await spService.ExecuteSPCreateNewPizza("Non Veg", "204SmokedChicken");
        // await spService.DeleteStoreProcedure("CreateNewPizza");

        // await spService.CreateStoredProcedure("BulkPizzaCreate");
        // await spService.ExecuteSPBulkPizzaCreate("veg", "Veg");
        // await spService.ExecuteSPBulkPizzaCreate("nonVeg", "Non Veg");
        // await spService.DeleteStoreProcedure("BulkPizzaCreate");

        // await spService.CreateStoredProcedure("GetPizzaById");
        // await spService.ExecuteSPGetPizzaById("Veg", "106");
        // await spService.ExecuteSPGetPizzaById("Non Veg", "201");
        // await spService.DeleteStoreProcedure("GetPizzaById");

        // await spService.CreateStoredProcedure("GetPizzaCount");
        // await spService.ExecuteGetPizzaCount("Veg");
        // await spService.ExecuteGetPizzaCount("Non Veg");
        // await spService.DeleteStoreProcedure("GetPizzaCount");

        // await spService.CreateStoredProcedure("GetPizzas");
        // await spService.ExecuteSPGetPizzas();
        // await spService.DeleteStoreProcedure("GetPizzas");

        // await spService.CreateStoredProcedure("DeletePizza");
        // await spService.ExecuteSPDeletePizza("Veg", "107");
        // await spService.ExecuteSPDeletePizza("Non Veg", "204");
        // await spService.DeleteStoreProcedure("DeletePizza");

        // await spService.ViewStoredProcedures();
      }
      catch (Exception ex)
      {
        WriteLine(ex.ToString());
      }
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