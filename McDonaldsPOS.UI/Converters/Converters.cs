using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace McDonaldsPOS.UI.Converters;

/// <summary>
/// Converts a boolean to visibility (true = visible, false = collapsed)
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            if (parameter?.ToString() == "Inverse")
                boolValue = !boolValue;
            return boolValue;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true;
    }
}

/// <summary>
/// Converts a hex color string to a SolidColorBrush
/// </summary>
public class HexColorToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrEmpty(hex))
        {
            try
            {
                return new SolidColorBrush(Color.Parse(hex));
            }
            catch
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a decimal to currency format
/// </summary>
public class CurrencyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            return $"${decimalValue:F2}";
        }
        return "$0.00";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            str = str.Replace("$", "").Trim();
            if (decimal.TryParse(str, out var result))
                return result;
        }
        return 0m;
    }
}

/// <summary>
/// Converts PIN string to masked display (with spacing)
/// </summary>
public class PinToMaskConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string pin && pin.Length > 0)
        {
            return string.Join("  ", Enumerable.Repeat("●", pin.Length));
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts count to visibility (> 0 = visible)
/// </summary>
public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            bool isVisible = count > 0;
            if (parameter?.ToString() == "Inverse")
                isVisible = !isVisible;
            return isVisible;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Inverts a boolean value
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}

/// <summary>
/// Returns true if string is not null or empty
/// </summary>
public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts bool to selected/unselected background color
/// </summary>
public class BoolToSelectionBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            if (parameter?.ToString() == "Error")
                return new SolidColorBrush(Color.Parse("#D32F2F")); // Red
            return new SolidColorBrush(Color.Parse("#4CAF50")); // Green
        }
        return new SolidColorBrush(Color.Parse("#424242")); // Dark gray
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts bool to error/success status colors
/// </summary>
public class BoolToStatusBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isError)
        {
            return isError
                ? new SolidColorBrush(Color.Parse("#D32F2F")) // Red for error
                : new SolidColorBrush(Color.Parse("#4CAF50")); // Green for success
        }
        return new SolidColorBrush(Color.Parse("#646464")); // Gray
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts bool to active/inactive foreground color for nav tabs
/// </summary>
public class BoolToNavColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive
                ? new SolidColorBrush(Color.Parse("#FFC72C")) // McDonald's yellow for active
                : new SolidColorBrush(Color.Parse("#888888")); // Gray for inactive
        }
        return new SolidColorBrush(Color.Parse("#888888"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
