/**
* * Query with continuation on both sides:  do the query in a sproc and continue paging the results; the sproc returns continuation token so it can be called multiple times and get around the 5 seconds limit.
* @param {string} pizzaType - 'Veg'/'Non Veg'
* @param {ContinuationToken} spContinuationToken
*/

function GetPizzas(pizzaType, price, spContinuationToken) {
  let context = getContext();
  let collection = context.getCollection();
  let collectionLink = collection.getSelfLink();
  let response = context.getResponse();
  let pizzaCount = 0;
  let vegPizzaCount = 0;
  let nonVegPizzaCount = 0;

  if (spContinuationToken) {
    let token = JSON.parse(spContinuationToken);
    console.log(`spContinuationToken: ${token}`);
    if (!token.countSoFar) { throw new Error('Bad token format: no count'); }
    if (!token.queryContinuationToken) { throw new Error('Bad token format: no continuation'); }

    // Retrieve "Count So Far"
    pizzaCount = token.countSoFar;
    // Retrieve queryContinuationToken to continue paging
    query(token.queryContinuationToken);
  }
  else {
    // Start a recursion
    query();
  }

  // Function within the main stored procedure function
  function query(queryContinuation) {
    let requestOptions = { continuation: queryContinuation };
    console.log(`QueryContinuation: ${queryContinuation}`);
    let sqlQuery = "SELECT * FROM c";
    if (price) {
      sqlQuery = `${sqlQuery} where c.price.large > '${price}'`;
    }
    // Query all document
    let isAccepted = collection.queryDocuments(collectionLink, sqlQuery, requestOptions,
      function (err, result, responseOptions) {
        if (err) throw err;

        console.log(`responseOptions: ${JSON.stringify(responseOptions)}`);
        
        // Scan the result
        if (result) {
          for (let i = 0; i != result.length; i++) {
            var pizza = result[i];
            // Total Pizza Count
            ++pizzaCount;
            // Filter all pizzas which are of the given pizzaType
            if (pizza.pizzaType == 'Veg') { ++vegPizzaCount; }
            if (pizza.pizzaType == 'Non Veg') { ++nonVegPizzaCount; }
          }
        }

        if (responseOptions.continuation) {
          console.log(`responseOptions.continuation: ${responseOptions.continuation}`);
          // Continue the query
          query(responseOptions.continuation);
        }
        else {
          // Return the result
          let data = {
            'continuation': null,
            'vegPizzaCount': vegPizzaCount,
            'nonVegPizzaCount': nonVegPizzaCount,
            'totalPizzaCount': pizzaCount,
            'pizzas': result
          };
          response.setBody(JSON.stringify(data));
        }
      });

    if (!isAccepted) {
      let sprocToken = {
        'countSoFar': pizzaCount,
        'queryContinuationToken': queryContinuation
      };

      response.setBody(JSON.stringify({ count: null, continuation: sprocToken }));
    }
  }
}