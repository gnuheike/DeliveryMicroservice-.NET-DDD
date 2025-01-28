namespace Primitives.Extensions;

public static class StringExtension
{
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}