using task_1_2;

namespace task_2_tests;

public class CalculatorTests
{
    [Fact]
    public void Multiply_TwoNumbers_ReturnsProduct()
    {
        // Arrange
        var calculator = new Calculator();
        var a = 2.5;
        var b = 3.0;
        var expected = 7.5;

        // Act
        var result = calculator.Multiply(a, b);

        // Assert
        Assert.Equal(expected, result, 3);
    }

    [Fact]
    public void Divide_DivisorNotZero_ReturnsQuotient()
    {
        // Arrange
        var calculator = new Calculator();
        var dividend = 10.0;
        var divisor = 2.0;
        var expected = 5.0;

        // Act
        var result = calculator.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expected, result, 3);
    }

    [Fact]
    public void Divide_DivisorZero_ReturnsNaN()
    {
        // Arrange
        var calculator = new Calculator();
        var dividend = 10.0;
        var divisor = 0.0;

        // Act
        var result = calculator.Divide(dividend, divisor);

        // Assert
        Assert.True(double.IsNaN(result));
    }
}