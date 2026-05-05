using System.Globalization;

namespace Football.MauiApp.Converters;

/// <summary>
/// Converts a tackle count (int) to a bar height in pixels for the bar chart.
/// Max tackles (30) maps to max height (120px).
/// </summary>
public class TackleHeightConverter : IValueConverter
{
    private const double MaxTackles = 30.0;
    private const double MaxHeight = 120.0;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int tackles)
            return Math.Max(8, tackles / MaxTackles * MaxHeight);
        return 8;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
