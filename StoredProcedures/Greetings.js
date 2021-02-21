/**
 * Greetings
 * * Copy the function to Cosmos db stored procedure,
 * * click execute, make the partition key as string and param name 'name'
 * * input parameter type as string and give the param value 'Tintin'
 * @param {string} name
 */

function Greetings(name) {
  let context = getContext();
  let response = context.getResponse();

  var greetings = 'Welcome to CosmosDB, ' + name;
  response.setBody(greetings);
}