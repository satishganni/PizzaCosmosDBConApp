using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Console;

namespace PizzaCosmosDBConApp.Services
{
  public class SPService
  {
    private readonly CosmosClient _cosmosClient;
    private Container _container;

    public SPService(Cosmos cosmos)
    {
      _cosmosClient = cosmos.Client;
    }

    public void SetContainer(string databaseId, string containerId)
    {
      _container = _cosmosClient.GetContainer(databaseId, containerId);
    }

    public async Task ViewStoredProcedures()
    {
      string message = "View StoredProcedures";
      Printer.PrintLine(message: message);

      using (FeedIterator<StoredProcedureProperties> feedIterator = _container.Scripts.GetStoredProcedureQueryIterator<StoredProcedureProperties>())
      {
        FeedResponse<StoredProcedureProperties> feedResponse = await feedIterator.ReadNextAsync();
        foreach (var item in feedResponse)
        {
          WriteLine($"StoredProc Id: {item.Id}, SelfLink: {item.SelfLink}");
        }
        Printer.PrintLine(noOfTimes: (101 + message.Length));
      }
    }

    public async Task CreateStoredProcedure(string spName)
    {
      var spBody = File.ReadAllText($"StoredProcedures/{spName}.js");

      Scripts scripts = _container.Scripts;

      StoredProcedureProperties storedProcedure = new StoredProcedureProperties
      {
        Id = spName,
        Body = spBody
      };

      StoredProcedureResponse storedProcedureResponse = await scripts.CreateStoredProcedureAsync(storedProcedure);
      var spResult = storedProcedureResponse.Resource;
      string message = "Create StoredProcedure";
      Printer.PrintLine(message: message);
      WriteLine($"SP Created, Id: {spResult.Id}, \n\tSelfLink: {spResult.SelfLink}");
      Printer.PrintLine(noOfTimes: (101 + message.Length));
    }

    public async Task DeleteStoreProcedure(string spName)
    {
      string message = $"Delete SP {spName}";
      Printer.PrintLine(message: message);

      Scripts scripts = _container.Scripts;
      await scripts.DeleteStoredProcedureAsync(spName);
      WriteLine($"Stored Procedure: {spName} delted");
      Printer.PrintLine(noOfTimes: (101 + message.Length));
    }

    public async Task ExecuteSPGreeting(string name)
    {
      string message = "Execute SPGreeting";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("Greetings", new PartitionKey("/pizzaType"), new dynamic[] { name });
      WriteLine($"SP Greeting Result: {sprocResponse.Resource}");
      Printer.PrintLine(noOfTimes: (101 + message.Length));
    }
  }
}