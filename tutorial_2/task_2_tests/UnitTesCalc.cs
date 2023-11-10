using Calc;

namespace task_2_tests;

public class CalculatorTests
{
    [Fact]
    public void Multiply_TwoNumbers_ReturnsProduct()
    {
        // Arrange
        var calculator = new Calculator();
        double a = 2.5;
        double b = 3.0;
        double expected = 7.5;

        // Act
        double result = calculator.Multiply(a, b);

        // Assert
        Assert.Equal(expected, result, 3);
    }

    [Fact]
    public void Divide_DivisorNotZero_ReturnsQuotient()
    {
        // Arrange
        var calculator = new Calculator();
        double dividend = 10.0;
        double divisor = 2.0;
        double expected = 5.0;

        // Act
        double result = calculator.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expected, result, 3);
    }

    [Fact]
    public void Divide_DivisorZero_ReturnsNaN()
    {
        // Arrange
        var calculator = new Calculator();
        double dividend = 10.0;
        double divisor = 0.0;

        // Act
        double result = calculator.Divide(dividend, divisor);

        // Assert
        Assert.True(double.IsNaN(result));
    }
}