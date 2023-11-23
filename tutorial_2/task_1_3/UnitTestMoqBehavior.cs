using task_1_2;

namespace task_1_3;

public class UnitTestMoqBehavior
{
    [Fact]
    public void Multiply_WithMock_ReturnsMultiply()
    {
        // Arrange
        var mockCalculator = new Mock<ICalculator>();
        mockCalculator
            .Setup(x => x.Multiply(
                It.IsAny<double>(),
                It.IsAny<double>()))
            .Returns((double a, double b) => a * b);

        // Act
        var result = mockCalculator.Object.Multiply(3, 4);

        // Assert
        Assert.Equal(12, result);
    }

    [Fact]
    public void Calc_WithMock()
    {
        // Arrange
        var mockCalculator = new Mock<ICalculator>();
        mockCalculator
            .Setup(x => x.Multiply(
                It.IsAny<double>(),
                It.IsAny<double>())
            )
            .Returns((double a, double b) => a * b);

        // Act
        var result = mockCalculator.Object.Multiply(3, 4);

        // Assert
        Assert.Equal(12, result);
    }
}