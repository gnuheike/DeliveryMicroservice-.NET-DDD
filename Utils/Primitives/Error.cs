using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace Primitives;

[ExcludeFromCodeCoverage]
public sealed class Error : ValueObject
{
    private const string Separator = "||";

    private Error()
    {
    }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    ///     Код ошибки
    /// </summary>
    public string Code { get; }

    /// <summary>
    ///     Текст ошибки
    /// </summary>
    public string Message { get; }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Code;
        yield return Message;
    }

    public string Serialize()
    {
        return $"{Code}{Separator}{Message}";
    }

    public static Error Deserialize(string serialized)
    {
        if (serialized == "A non-empty request body is required.")
            return GeneralErrors.ValueIsRequired(nameof(serialized));

        var data = serialized.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

        if (data.Length < 2)
            throw new FormatException($"Invalid error serialization: '{serialized}'");

        return new Error(data[0], data[1]);
    }
}