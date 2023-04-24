using System;
using System.Collections;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Used for allowing a struct to be mutated in a PropertyGrid.
/// </summary>
public sealed class ValueTypeTypeConverter : ExpandableObjectConverter
{
    public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context) => true;

    public override object? CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
    {
        if (context?.PropertyDescriptor is not { } pd)
            return null;

        var boxed = pd.GetValue(context.Instance);
        foreach (DictionaryEntry entry in propertyValues)
        {
            if (entry.Key.ToString() is not { } propName)
                continue;
            var pi = pd.PropertyType.GetProperty(propName);
            if (pi?.CanWrite == true)
                pi.SetValue(boxed, Convert.ChangeType(entry.Value, pi.PropertyType), null);
        }
        return boxed;
    }
}

public sealed class TypeConverterU32 : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is uint u)
            return $"{u:X8}"; // no 0x prefix
        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        if (value is not string input)
            return base.ConvertFrom(context, culture, value);
        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            input = input[2..];
        return uint.TryParse(input, System.Globalization.NumberStyles.HexNumber, culture, out var result) ? result : 0u;
    }
}

public sealed class TypeConverterU64 : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is ulong u)
            return $"{u:X16}"; // no 0x prefix
        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        if (value is not string input)
            return base.ConvertFrom(context, culture, value);
        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            input = input[2..];
        return ulong.TryParse(input, System.Globalization.NumberStyles.HexNumber, culture, out var result) ? result : 0ul;
    }
}
