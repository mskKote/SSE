namespace task_1_3;

public static class FileHelper
{
    public static List<(double, double, double)> ReadFloatTriplesFromFile(string filePath)
    {
        var floatPairs = new List<(double, double, double)>();

        try
        {
            using var reader = new StreamReader(filePath);
            string line;
            while ((line = reader.ReadLine() ?? string.Empty) != null)
            {
                Console.WriteLine(line);
                if (TryParseFloatTriple(line, out var floatPair)) floatPairs.Add(floatPair);
                // If parsing fails for a line, you may want to handle it or skip it.
                throw new IOException("Не получилось считать из файла");
            }
        }
        catch (Exception ex)
        {
            // Handle file IO exception (e.g., file not found, permission issues, etc.)
            Console.WriteLine($"Error reading file: {ex.Message}");
            throw new IOException(ex.Message);
        }

        return floatPairs;
    }

    private static bool TryParseFloatTriple(string input, out (double, double, double) floatTriple)
    {
        floatTriple = default;

        var values = input.Split(", ", StringSplitOptions.RemoveEmptyEntries);

        if (values.Length != 3) return false;
        if (!double.TryParse(values[0], out var firstNumber)) return false;
        if (!double.TryParse(values[1], out var secondNumber)) return false;
        if (!double.TryParse(values[2], out var thirdNumber)) return false;
        floatTriple = (firstNumber, secondNumber, thirdNumber);
        return true;
    }
}