/**
* * Query with continuation on both sides:  do the query in a sproc and continue paging the results; the sproc returns continuation token so it can be called multiple times and get around the 5 seconds limit.
* @param {string} pizzaType - 'Veg'/'Non Veg'
* @param {string} price
* @param {ContinuationToken} spContinuationToken
*/

function GetPizzas(pizzaType, price, spContinuationToken) {

}