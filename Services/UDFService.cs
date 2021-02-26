using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;

using static System.Console;

namespace PizzaCosmosDBConApp.Services
{
  public class UDFService
  {
    private readonly CosmosClient _cosmosClient;
    private Container _container;

    public UDFService(Cosmos cosmos)
    {
      _cosmosClient = cosmos.Client;
    }
    public void SetContainer(string databaseId, string containerId)
    {
      _container = _cosmosClient.GetContainer(databaseId, containerId);
    }

    public async Task ViewUserDefinedFunctions()
    {
      string message = "View UserDefinedFunctions";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      using (FeedIterator<UserDefinedFunctionProperties> feedIterator = scripts.GetUserDefinedFunctionQueryIterator<UserDefinedFunctionProperties>())
      {
        FeedResponse<UserDefinedFunctionProperties> response = await feedIterator.ReadNextAsync();
        foreach (var item in response)
        {
          WriteLine($"UDF Id: {item.Id},\nUDF SelfLink: {item.SelfLink}");
        }

      }
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task CreateUDF(string udfName)
    {
      var udf = File.ReadAllText($@"UserDefinedFunctions/{udfName}.js");
      string message = "Cerate new UserDefinedFunction";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      UserDefinedFunctionProperties udfProperties = new UserDefinedFunctionProperties
      {
        Id = udfName,
        Body = udf
      };
      UserDefinedFunctionResponse udfResponse = await scripts.CreateUserDefinedFunctionAsync(udfProperties);
      WriteLine($"EDF Created with the Id: {udfResponse.ActivityId}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task DeleteUDF(string udfName)
    {
      string message = "Delete UserDefinedFunction";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      UserDefinedFunctionResponse udfResponse = await scripts.DeleteUserDefinedFunctionAsync(udfName);
      WriteLine($"{udfName} -- UDF Deleted");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task ExecuteUDF()
    {
      var queryText = "SELECT c.pizzaName, c.pizzaType, c.toppings, udf.ToppingsCountUDF(c.toppings) as ToppingsCount FROM c";

      FeedIterator<dynamic> feedIterator = _container.GetItemQueryIterator<dynamic>(queryText);
      while (feedIterator.HasMoreResults)
      {
        FeedResponse<dynamic> response = await feedIterator.ReadNextAsync();
        WriteLine($"Response Count: {response.Count}");
        int ctr = 1;
        foreach (var item in response.Resource)
        {
          WriteLine($"{ctr++} - \n{item}");
        }
      }
    }

  }
}