/**
 * * Performing Bulk Inserts of pizza into a collection
 * @param {Object[]} pizzaArray array of json objects
 */

function BulkPizzaCreate(pizzaArray) {

  if (!pizzaArray) throw new Error(`The Pizza Arrary is undefined or null`);

  let context = getContext();
  let collection = context.getCollection();
  let collectionLink = collection.getSelfLink();
  let response = context.getResponse();

  let pizzaCounter = 0;
  let pizzaArrayLength = pizzaArray.length;
  let resultData = {
    'noOfPizzaCreated': 0
  };

  createNewPizza(pizzaArray[pizzaCounter], callback);

  function createNewPizza(newPizza, callback) {
    let options = { disableAutomaticIdGeneration: true };

    let isAccepted = collection.createDocument(collectionLink, newPizza, options, callback);

    if (!isAccepted) response.setBody(JSON.stringify(resultData.noOfPizzaCreated = pizzaCounter));
  }

  function callback(err, newPizza) {
    if (err) throw new Error(`Error while creating new Pizza: ${err}`);
    pizzaCounter++;

    if (pizzaCounter >= pizzaArrayLength) {
      resultData.noOfPizzaCreated = pizzaCounter;
      response.setBody(JSON.stringify(resultData));
    }
    else {
      createNewPizza(pizzaArray[pizzaCounter], callback);
    }
  }
}