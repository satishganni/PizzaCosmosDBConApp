/**
 * * Query for a pizza on bases of id and if found delete the pizza
 * @param {string} pizzaId
 */

function DeletePizza(pizzaId) {
  let context = getContext();
  let collection = context.getCollection();
  let collectionLink = collection.getSelfLink();
  let response = context.getResponse();

  if (!pizzaId) throw new Error('Please give a PizzaId');

  let queryText = `SELECT * FROM c where c.id='${pizzaId}'`;
  let resultData = {
    'pizzaCount': 0,
    'pizza': '',
    'pizzaDeleted': 'false'
  };

  let isAccepted = collection.queryDocuments(collectionLink, queryText, callback);

  function callback(err, result) {
    if (err) throw new Error(`Error: ${err}`);

    if (!result || result.length == 0) {
      resultData.pizzaCount = result.length;
      resultData.pizza = `No Pizza found with PizzaId: ${pizzaId}`;
      resultData.pizzaDeleted = 'false';
    }
    else {
      if (result.length == 1) {
        resultData.pizzaCount = result.length;
        resultData.pizza = result[0];
        isAccepted = collection.deleteDocument(result[0]._self, {}, deleteCallback);
      }
    }
  }

  function deleteCallback(err, deleteResult) {
    if (err) throw new Error(`Error while Deleting Pizza: ${err}`);
    if (deleteResult) {
      resultData.pizzaDeleted = 'true';
      response.setBody(JSON.stringify(resultData));
    }
  }

  if (!isAccepted) throw new Error('The query/delete was not accepted by the server');
}