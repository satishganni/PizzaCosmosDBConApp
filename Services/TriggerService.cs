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

    public async Task ViewTriggers()
    {
      string message = "View Triggers";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;
      using (FeedIterator<TriggerProperties> feedIterator = scripts.GetTriggerQueryIterator<TriggerProperties>())
      {
        FeedResponse<TriggerProperties> triggerResponse = await feedIterator.ReadNextAsync();
        foreach (var item in triggerResponse)
        {
          WriteLine($"TriggerId: {item.Id}, Trigger Type: {item.TriggerType}, Trigger Operation: {item.TriggerOperation}, \nTrigger SelfLinkn: {item.SelfLink}");
        }
      }
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task CreateTrigger(string triggerName, string triggerType = "Post")
    {
      string message = "Create Triggers";
      Printer.PrintLine(message: message);
      var triggerBody = File.ReadAllText($"Triggers/{triggerName}.js");
      TriggerType tt = string.Equals(triggerType, "Pre") ? TriggerType.Pre : TriggerType.Post;
      Scripts scripts = _container.Scripts;
      TriggerProperties triggerProperties = new TriggerProperties
      {
        Id = triggerName,
        Body = triggerBody,
        TriggerType = tt,
        TriggerOperation = TriggerOperation.Create
      };

      TriggerResponse triggerResponse = await scripts.CreateTriggerAsync(triggerProperties);
      WriteLine($"Trigger Details, Id: {triggerResponse.Resource.Id}, SelfLink: {triggerResponse.Resource.SelfLink},\nTriggerOperation: {triggerResponse.Resource.TriggerOperation}, TriggerType: {triggerResponse.Resource.TriggerType}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task DeleteTrigger(string triggerName)
    {
      string message = "Delete Triggers";
      Printer.PrintLine(message: message);
      Scripts scripts = _container.Scripts;

      TriggerResponse triggerResponse = await scripts.DeleteTriggerAsync(triggerName);
      WriteLine($"Delete Trigger, Deleted Trigger with Id: {triggerName}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task CreateNewPizzaPost()
    {
      var pizza = JObject.Parse(File.ReadAllText(@"Services/TriggerPizza.json"));
      string message = "Create New Pizza With Post-Trigger";
      Printer.PrintLine(message: message);
      var itemResponse = await _container.CreateItemAsync(pizza, new PartitionKey("Veg"),
          new ItemRequestOptions { PostTriggers = new List<string> { "PizzaTypeTrigger" } });
      WriteLine($"Response: {itemResponse.Resource}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }

    public async Task CreateNewPizzaPre()
    {
      var pizza = JObject.Parse(File.ReadAllText(@"Services/TriggerPizza.json"));
      string message = "Create New Pizza With Pre-Trigger";
      Printer.PrintLine(message: message);
      var itemResponse = await _container.CreateItemAsync(pizza, new PartitionKey("Veg"),
          new ItemRequestOptions { PreTriggers = new List<string> { "PriceValidationTrigger" } });
      WriteLine($"Response: {itemResponse.Resource}");
      Printer.PrintLine(noOfTimes: (100 + message.Length));
    }
  }
}