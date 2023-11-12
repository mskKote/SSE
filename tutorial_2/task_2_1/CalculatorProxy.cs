namespace task_2_1;

public class CalculatorProxy : ICalculator
{
    private readonly IList<(double, double, char, double)> _cache =
        new List<(double, double, char, double)>();

    private readonly ICalculator _calculator = new Calculator();

    public double Multiply(double lhs, double rhs)
    {
        if (GetFromCacheIfExist((lhs, rhs, '*'), out var cached))
            return cached;

        var result = _calculator.Multiply(lhs, rhs);
        AddInCacheAndKeep10Elements((lhs, rhs, '*'), result);
        return result;
    }

    public double Divide(double lhs, double rhs)
    {
        if (GetFromCacheIfExist((lhs, rhs, '/'), out var cached))
            return cached;

        var result = _calculator.Divide(lhs, rhs);
        AddInCacheAndKeep10Elements((lhs, rhs, '/'), result);
        return result;
    }

    #region Cache

    private bool GetFromCacheIfExist(
        (double, double, char) key,
        out double result)
    {
        foreach (var (lhs, rhs, operation, cachedResult) in _cache)
            if (lhs == key.Item1 && rhs == key.Item2 && operation == key.Item3)
            {
                result = cachedResult;
                return true;
            }

        result = default;
        return false;
    }

    private void AddInCacheAndKeep10Elements(
        (double, double, char) key,
        double value)
    {
        _cache.Add((key.Item1, key.Item2, key.Item3, value));

        if (_cache.Count > 10) _cache.RemoveAt(0);
    }

    #endregion
}