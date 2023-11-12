using task_2_1;

namespace task_2_1_tests;

public class CalculatorTests
{
    private readonly CalculatorProxy _cachedCalculatorProxy = new();

    [Theory]
    [InlineData(2.0, 3.0, 6.0)]
    [InlineData(1.5, 4.0, 6.0)]
    [InlineData(0.0, 5.0, 0.0)]
    [InlineData(-2.0, -3.0, 6.0)]
    [InlineData(2.5, 2.0, 5.0)]
    [InlineData(-1.0, -1.0, 1.0)]
    [InlineData(3.0, 0.0, 0.0)]
    [InlineData(0.0, 0.0, 0.0)]
    [InlineData(2.0, -2.0, -4.0)]
    [InlineData(-0.5, 4.0, -2.0)]
    public void TestMultiply(double lhs, double rhs, double expected)
    {
        var c = new Calculator();
        var result = c.Multiply(lhs, rhs);
        Assert.Equal(expected, result);
    }


    [Fact]
    public void TestMultiply_WithCache()
    {
        var data = new List<(double, double, double)>
        {
            (2.0, 3.0, 6.0),
            (1.5, 4.0, 6.0),
            (1.5, 4.0, 6.0),
            (0.0, 5.0, 0.0),
            (-2.0, -3.0, 6.0),
            (2.5, 2.0, 5.0),
            (-1.0, -1.0, 1.0),
            (3.0, 0.0, 0.0),
            (1.0, 0.0, 0.0),
            (2.0, 0.0, 0.0),
            (3.0, 0.0, 0.0),
            (4.0, 0.0, 0.0),
            (5.0, 0.0, 0.0),
            (6.0, 0.0, 0.0),
            (2.0, -2.0, -4.0),
            (-0.5, 4.0, -2.0)
        };
        foreach (var (lhs, rhs, expected) in data)
        {
            var result = _cachedCalculatorProxy.Multiply(lhs, rhs);
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void TestDivide()
    {
        var c = new Calculator();
        var result = c.Divide(6, 3);
        Assert.Equal(2, result);
    }

    [Fact]
    public void TestDivideByZero()
    {
        var c = new Calculator();
        var result = c.Divide(6, 0);
        Assert.Equal(double.NaN, result);
    }
}