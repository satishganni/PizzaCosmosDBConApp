/**
 * * Insert New Pizza by uploading a json
 * @param {pizzaJson} docToCreate
 */

function CreateNewPizza(docToCreate) {
  let context = getContext();
  let collection = context.getCollection();
  let collectionLink = collection.getSelfLink();
  let response = context.getResponse();

  // If required do the validation

  let options = { disableAutomaticIdGeneration: true };

  let pizzaCreated = collection.createDocument(collectionLink, docToCreate, options, function (err, docCreated) {
    if (err) {
      throw new Error(`Error Creating new Pizza ${err.Message}`);
    }
    let responseBody = { newPizzaCreate: docCreated };
    response.setBody(JSON.stringify(responseBody));
  });

  if (!pizzaCreated) throw new Error("The query wasn't accepted by sever. Try again/use continution token between API and SCRIPT");
}