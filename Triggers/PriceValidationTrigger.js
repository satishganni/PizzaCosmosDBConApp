/**
 * * Pre-Trigger for validation of Price Property, it should have 'pan', 'medium' and 'large'
 */

function PriceValidationTrigger() {
  let context = getContext();
  let request = context.getRequest();
  let pizzaDoc = request.getBody();

  let isValid = true;

  if (pizzaDoc.price.pan == undefined || pizzaDoc.price.medium == undefined || pizzaDoc.price.large == undefined) {
    isValid = false;
  }
  else {
    isValid = true;
  }

  if (!isValid) throw new Error("***price must contain 'pan', 'medium' and 'large'***");

  request.setBody(pizzaDoc);
}