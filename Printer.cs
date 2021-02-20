using static System.Console;

namespace PizzaCosmosDBConApp
{
  public static class Printer
  {
    public static void PrintLine(string pattern = "-", string message = "", int noOfTimes = 100)
    {
      int center = (noOfTimes / 2);
      WriteLine("");
      for (int i = 0; i < noOfTimes; i++)
      {
        Write($"{pattern}");
        if (i == center) Write($"{message}");
      }
      WriteLine("");
    }
  }
}