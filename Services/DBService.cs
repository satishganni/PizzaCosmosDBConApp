using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

using static System.Console;

namespace PizzaCosmosDBConApp.Services
{
  public class DBService
  {
    private readonly CosmosClient _cosmosClient;
    private string _databaseId, _containerId, _partitionName;
    private Database _database;
    private Container _container;

    public DBService(Cosmos cosmos)
    {
      _cosmosClient = cosmos.Client;
    }

    public void SetIds(string databaseId, string containerId, string partitionName)
    {
      _databaseId = databaseId;
      _containerId = containerId;
      _partitionName = partitionName;
    }

    public async Task<bool> CreateDatabase()
    {
      string message = "Create Database";
      DatabaseResponse dbResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseId, 400);
      if (dbResponse.StatusCode == System.Net.HttpStatusCode.Created)
      {
        Printer.PrintLine(message: message);
        WriteLine($"Database: '{_databaseId}', Created Successfully");
        _database = _cosmosClient.GetDatabase(_databaseId);
        Printer.PrintLine(noOfTimes: (100 + message.Length));
        return true;
      }
      return false;
    }

    public async Task<bool> CreateContainer()
    {
      string message = "Create Container";
      ContainerProperties containerProperties = new ContainerProperties(_containerId, $"/{_partitionName}");
      ContainerResponse cResponse = await _database.CreateContainerIfNotExistsAsync(containerProperties);
      if (cResponse.StatusCode == System.Net.HttpStatusCode.Created)
      {
        Printer.PrintLine(message: message);
        WriteLine($"Container: '{_containerId}', Created Successfully in the Database: '{_database.Id}'");
        _container = _database.GetContainer(_containerId);
        Printer.PrintLine(noOfTimes: (100 + message.Length));
        return true;
      }
      return false;
    }

    public void DatabaseName()
    {
      _database = _cosmosClient.GetDatabase(_databaseId);
      WriteLine($"Database Name/Id: {_database.Id}");
    }
    public void ContainerName()
    {
      _container = _database.GetContainer(_containerId);
      WriteLine($"Container Name/Id: {_container.Id}");
    }

    public async Task GetAllDatabases()
    {
      string message = "Get all databases";
      var queryText = "Select * from c";
      using (FeedIterator<DatabaseProperties> feedIterator = _cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>(queryText))
      {
        FeedResponse<DatabaseProperties> response = await feedIterator.ReadNextAsync();
        Printer.PrintLine(message: message);
        foreach (var item in response)
        {
          WriteLine($"Database details, Id: {item.Id}, ETag: {item.ETag}, \n\tSelfLink: {item.SelfLink}");
        }
        Printer.PrintLine(noOfTimes: (100 + message.Length));
      }
    }

    public async Task GetAllContainers()
    {
      string message = "Get all containers";
      using (FeedIterator<ContainerProperties> feedIterator = _database.GetContainerQueryIterator<ContainerProperties>())
      {
        while (feedIterator.HasMoreResults)
        {
          Printer.PrintLine(message: message);
          FeedResponse<ContainerProperties> response = await feedIterator.ReadNextAsync();
          foreach (var item in response)
          {
            WriteLine($"Container Details, Id: {item.Id}, PartitionKeyDefinitionVersion: {item.PartitionKeyDefinitionVersion ?? 0}, PartitionKeyPath: {item.PartitionKeyPath}, \n\tSelfLink: {item.SelfLink}");
          }
          Printer.PrintLine(noOfTimes: (100 + message.Length));
        }
      }
    }
  }
}