using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using PizzaCosmosDBConApp.Models;

using static System.Console;

namespace PizzaCosmosDBConApp
{
  public class Cosmos
  {
    private readonly string _cosmosEndpoint;
    private readonly string _cosmosKey;

    public CosmosClient Client { get; private set; }

    public Cosmos(IOptions<CosmosUtility> cosmosUtility)
    {
      _cosmosEndpoint = cosmosUtility.Value.CosmosEndpoint;
      _cosmosKey = cosmosUtility.Value.CosmosKey;
      Client = new CosmosClient(_cosmosEndpoint, _cosmosKey);
    }

    public void DisplayCosmosConnection() => WriteLine($"CosmosEndpoint: ${_cosmosEndpoint}\nCosmosKey: {_cosmosKey}");
  }
}