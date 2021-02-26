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

    public async Task ExecuteSPCreateNewPizza(string pizzaType, string pizzaFileName)
    {
      string newPizza = File.ReadAllText($"Models/PizzaData/{pizzaFileName}.json");
      string message = "Execute SPCreateNewPizza";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("CreateNewPizza", new PartitionKey(pizzaType), new dynamic[] { newPizza });
      WriteLine($"SP Create New Pizza Result: \n{JsonConvert.DeserializeObject(sprocResponse.Resource)}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task ExecuteSPBulkPizzaCreate(string pizzaFile, string partitionKey)
    {
      var jsonArray = JArray.Parse(File.ReadAllText($@"Models/PizzaData/PizzaCollection/{pizzaFile}PizzaCollection.json"));
      string message = "Execute SPBulkPizzaCreate";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("BulkPizzaCreate", new PartitionKey($"{partitionKey}"), new dynamic[] { jsonArray });
      WriteLine($"SP Bulk Pizza Create Result: \n{JsonConvert.DeserializeObject(sprocResponse.Resource)}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task ExecuteSPGetPizzaById(string pizzaType, string pizzaId)
    {
      string message = "Execute SPGetPizzaById";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      StoredProcedureExecuteResponse<string> sprcoResponse = await scripts.ExecuteStoredProcedureAsync<string>("GetPizzaById", new PartitionKey(pizzaType), new dynamic[] { pizzaId });
      WriteLine($"SP GetPizzaById Result: \n{JsonConvert.DeserializeObject(sprcoResponse.Resource)}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }


    public async Task ExecuteGetPizzaCount(string partitionKey)
    {
      string message = "Execute GetPizzaCount";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("GetPizzaCount", new PartitionKey($"{partitionKey}"), new dynamic[] { });
      WriteLine($"SP GetPizzaCount Result: {JsonConvert.DeserializeObject(sprocResponse.Resource)}");
      Printer.PrintLine(noOfTimes: (101 + message.Length));
    }

    public async Task ExecuteSPGetPizzas(string pizzaType = "Veg", decimal price = 300)
    {
      string message = "Execute SPGetPizzas";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("GetPizzas", new PartitionKey(pizzaType), new dynamic[] { pizzaType, price });
      // StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("GetPizzas", new PartitionKey(pizzaType), new dynamic[] { pizzaType });
      // StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("GetPizzas", new PartitionKey(pizzaType), new dynamic[] { });

      WriteLine($"SP Get VegPizza Result: \n{JsonConvert.DeserializeObject(sprocResponse.Resource)}");
      Printer.PrintLine(noOfTimes: (101 + message.Length));
    }

    public async Task ExecuteSPDeletePizza(string pizzaType, string pizzaId)
    {
      string message = "Execute SPDeletePizza";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      StoredProcedureExecuteResponse<string> sprocResponse = await scripts.ExecuteStoredProcedureAsync<string>("DeletePizza", new PartitionKey(pizzaType), new dynamic[] { pizzaId });
      WriteLine($"SP DeletePizza Result: \n{JsonConvert.DeserializeObject(sprocResponse.Resource)}");
      Printer.PrintLine(noOfTimes: (101 + message.Length));
    }


  }
}