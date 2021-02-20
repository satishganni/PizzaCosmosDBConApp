using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json.Linq;
using static System.Console;
namespace PizzaCosmosDBConApp.Services
{
  public class TriggerService
  {
    private readonly CosmosClient _cosmosClient;
    private Container _container;
    public TriggerService(Cosmos cosmos)
    {
      _cosmosClient = cosmos.Client;
    }
    public void SetContainer(string databaseId, string containerId)
    {
      _container = _cosmosClient.GetContainer(databaseId, containerId);
    }
  }
}