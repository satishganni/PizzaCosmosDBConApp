/**
 * * Query for a pizza on the bases of Id
* @param {string} pizzaId
 */

function GetPizzaById(pizzaId) {
  let context = getContext();
  let collection = context.getCollection();
  let collectionLink = collection.getSelfLink();
  let response = context.getResponse();

  if (!pizzaId || pizzaId == undefined) throw new Error(`Please give a pizzaId`);
  let queryText = `SELECT * FROM c WHERE c.id='${pizzaId}'`
  let isAccepted = collection.queryDocuments(collectionLink, queryText, callback);

  function callback(err, result) {
    if (err) throw new Error(`Error: ${err}`);
    let resultData = {};
    if (!result || result.length == 0) {
      resultData = {
        'pizzaCount': result.length,
        'pizza': `No Pizza found with the Id: ${pizzaId}`
      };
    }
    else {
      resultData = {
        'pizzaCount': result.length,
        'pizza': result[0]
      };
    }
    response.setBody(JSON.stringify(resultData));
  }
}