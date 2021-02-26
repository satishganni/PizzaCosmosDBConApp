/**
 * * After adding a new pizza, return the message with the PizzaType created 
 */

function PizzaTypeTrigger() {
  let context = getContext();
  let request = context.getRequest();
  let pizzaDoc = request.getBody();
  let response = context.getResponse();

  let data = {
    operationType: request.getOperationType(),
    headerValue: request.getValue(),
    message: '',
    pizza: ''
  }

  if (pizzaDoc.pizzaType == "Veg") {
    data.message = "New Veg Pizza Added";
  } else {
    data.message = "New Non Veg Pizza Added";
  }

  data.pizza = pizzaDoc;
  response.setBody(data);
}