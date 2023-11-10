namespace Calc;

public static class Calculator
{
  public static double Multiply(double a, double b)
  {
    return a * b;
  }

  public static double Divide(double dividend, double divisor)
  {
    return divisor == 0
     ? double.NaN
     : dividend / divisor;
  }
}
