namespace Primitives;

/// <summary>
///     Общие ошибки
/// </summary>
public static class GeneralErrors
{
    public static Error NotFound(long? id = null)
    {
        var forId = id == null ? "" : $" for Id '{id}'";
        return new Error("record.not.found", $"Record not found{forId}");
    }

    public static Error ValueIsInvalid(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(name);
        return new Error("value.is.invalid", $"Value is invalid for {name}");
    }

    public static Error ValueIsRequired(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(name);
        return new Error("value.is.required", $"Value is required for {name}");
    }

    public static Error InvalidLength(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(name);
        return new Error("invalid.string.length", $"Invalid {name} length");
    }

    public static Error CollectionIsTooSmall(int min, int current)
    {
        return new Error(
            "collection.is.too.small",
            $"The collection must contain {min} items or more. It contains {current} items.");
    }

    public static Error CollectionIsTooLarge(int max, int current)
    {
        return new Error(
            "collection.is.too.large",
            $"The collection must contain {max} items or more. It contains {current} items.");
    }

    public static Error InternalServerError(string message)
    {
        return new Error("internal.server.error", message);
    }
}