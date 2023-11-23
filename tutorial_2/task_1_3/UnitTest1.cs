using System.Globalization;
using task_1_2;

namespace task_1_3;

public class CalculatorTests
{
    public static IEnumerable<object[]> TestDataFromFile =>
        File.ReadLines("./mock_for_multiply.txt")
            .Select(line => line.Split(", "));

    [Fact]
    public void ReadFromFileIoException()
    {
        // Arrange
        const string nonExistPath = "abc.txt";

        // Act and Assert
        Assert.Throws<IOException>(() =>
            FileHelper.ReadFloatTriplesFromFile(nonExistPath));
    }

    [Fact]
    public void CalcMultiply_WithMock()
    {
        // Arrange
        const string multiplyMockPath = "mock_for_multiply.txt";
        var multiplyMock = FileHelper.ReadFloatTriplesFromFile(multiplyMockPath);
        var calculator = new Calculator();

        // Act and Assert
        multiplyMock.ForEach(x => Assert.Equal(calculator.Multiply(x.Item1, x.Item2), x.Item3));
    }

    [Theory]
    [MemberData(nameof(TestDataFromFile))]
    public void Multiply_WithFileData_ReturnsExpectedResult(string a, string b, string expected)
    {
        // Arrange
        var calculator = new Calculator();
        var aValue = double.Parse(a, CultureInfo.InvariantCulture);
        var bValue = double.Parse(b, CultureInfo.InvariantCulture);
        var expectedValue = double.Parse(expected, CultureInfo.InvariantCulture);

        // Act
        var result = calculator.Multiply(aValue, bValue);

        // Assert
        Assert.Equal(expectedValue, result, 10);
    }


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
    public void Multiply_WithMock_ReturnsExpectedResult(double a, double b, double expected)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Multiply(a, b);

        // Assert
        Assert.Equal(expected, result, 10);
    }
}