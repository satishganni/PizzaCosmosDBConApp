/**
** User Defined Function for getting the count of toppings of a given pizza
*/

function ToppingsCountUDF(toppings) {
  var toppingsLength = toppings.length;
  return toppingsLength;
}