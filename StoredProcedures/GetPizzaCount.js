/**
 * * Get the count of pizza's
 */

function GetPizzaCount() {
  var context = getContext();
  var collection = context.getCollection();
  var collectionLink = collection.getSelfLink();
  var response = context.getResponse();

  var pizzaCount = 0;
  var vegPizzaCount = 0;
  var nonVegPizzaCount = 0;
  var query = "SELECT * FROM c"

  var isAccepted = collection.queryDocuments(collectionLink, query, {},
    function (err, result, responseOptions) {
      if (err) throw err;

      if (result) {
        // console.log(`result: ${result[0]}`);
        for (let i = 0; i != result.length; i++) {
          var pizza = result[i];
          // console.log(`pizza: ${pizza}`);
          ++pizzaCount;
          // filter document with 'pizzaType' as 'Veg'
          if (pizza.pizzaType == 'Veg') {
            ++vegPizzaCount;
          }
          else if (pizza.pizzaType == 'Non Veg') {
            ++nonVegPizzaCount;
          }
        }
      }
      var resultData = {
        'resultLength': result.length,
        'totalPizzaCount': pizzaCount,
        'vegPizzaCount': vegPizzaCount,
        'nonVegPizzaCount': nonVegPizzaCount
      };

      response.setBody(JSON.stringify(resultData));
    });

  if (!isAccepted) throw new Error('The query was not accepted by the server');
}