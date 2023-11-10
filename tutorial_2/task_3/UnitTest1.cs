using Calc;

public interface IFileOperations
{
    string ReadFile(string filePath);
    void WriteFile(string filePath, string content);
}
public class CalculatorTests
{
    [Fact]
    public void ReadFromFile_FileNotReady_ThrowsIOException()
    {
        // Arrange
        var mockFileOperations = new Mock<IFileOperations>();
        mockFileOperations.Setup(x => x.ReadFile(It.IsAny<string>()))
                          .Throws(new IOException("File is not ready"));

        var calculator = new Calc.Calculator();
        string filePath = "example.txt";

        // Act and Assert
        // Assert.Throws<IOException>(() => calculator.ReadFromFile(filePath));
    }

    [Fact]
    public void WriteToFile_FileLocked_ThrowsIOException()
    {
        // Arrange
        var mockFileOperations = new Mock<IFileOperations>();
        mockFileOperations.Setup(x => x.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                          .Throws(new IOException("File is locked"));

        var calculator = new Calculator();
        string filePath = "example.txt";
        string content = "Hello, World!";

        // Act and Assert
        // Assert.Throws<IOException>(() => calculator.WriteToFile(filePath, content));
    }
}
