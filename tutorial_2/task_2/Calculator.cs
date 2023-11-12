namespace task_2;

public class Calculator : ICalculator
{
  public double Multiply(double a, double b)
  {
    return a * b;
  }

  public double Divide(double dividend, double divisor)
  {
    return divisor == 0
     ? double.NaN
     : dividend / divisor;
  }
}
