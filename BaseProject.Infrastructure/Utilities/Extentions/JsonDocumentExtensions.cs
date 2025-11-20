using System.Text.Json;

namespace BaseProject.Infrastructure.Utilities.Extentions;
public static class JsonDocumentExtensions
{
    public static T? GetPropertyValue<T>(this JsonDocument doc, string path)
    {
        ArgumentNullException.ThrowIfNull(doc);

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Property path cannot be null or empty.", nameof(path));

        JsonElement current = doc.RootElement;

        foreach (var part in path.Split('.'))
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(part, out current))
            {
                return default;
            }
        }

        try
        {
            object? value = current.ValueKind switch
            {
                JsonValueKind.String when typeof(T) == typeof(Guid) => Guid.Parse(current.GetString()!),
                JsonValueKind.String => current.GetString(),
                JsonValueKind.Number when typeof(T) == typeof(int) => current.GetInt32(),
                JsonValueKind.Number when typeof(T) == typeof(long) => current.GetInt64(),
                JsonValueKind.Number when typeof(T) == typeof(decimal) => current.GetDecimal(),
                JsonValueKind.True or JsonValueKind.False when typeof(T) == typeof(bool) => current.GetBoolean(),
                _ => JsonSerializer.Deserialize<T>(current.GetRawText())
            };

            return (T?)value;
        }
        catch
        {
            return default;
        }
    }
}