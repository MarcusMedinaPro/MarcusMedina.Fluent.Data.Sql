using System.Text.RegularExpressions;

namespace MarcusMedina.Fluent.Data.Sql.Extensions;

/// <summary>
/// SQL-inspirerade strängmetoder för mönstermatchning (LIKE, IN, BETWEEN).
/// Inspirationskälla för studerande att bygga egna hjälpklasser.
/// </summary>
public static class StringSqlExtensions
{
    /// <summary>
    /// SQL LIKE-mönstermatchning. % matchar valfri sekvens, _ matchar ett tecken.
    /// </summary>
    /// <param name="value">Strängen att testa.</param>
    /// <param name="pattern">LIKE-mönster (t.ex. "hello%", "%world", "h%d").</param>
    /// <param name="caseSensitive">Om true, görs skiftlägeskänslig matchning.</param>
    /// <example>
    /// "hello world".IsLike("hello%") → true
    /// </example>
    public static bool IsLike(this string value, string pattern, bool caseSensitive = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(pattern);

        var regexPattern = $"^{Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".")}$";
        var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
        return Regex.IsMatch(value, regexPattern, options);
    }

    /// <summary>
    /// SQL CONTAINER — kontrollerar om en sträng innehåller en sökterm.
    /// </summary>
    /// <param name="value">Strängen att söka i.</param>
    /// <param name="searchTerm">Termen att söka efter.</param>
    /// <param name="caseSensitive">Om true, görs skiftlägeskänslig sökning.</param>
    /// <example>
    /// "hello world".SqlContains("world") → true
    /// </example>
    public static bool SqlContains(this string value, string searchTerm, bool caseSensitive = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(searchTerm);

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return value.Contains(searchTerm, comparison);
    }

    /// <summary>
    /// SQL STARTS WITH — kontrollerar om en sträng börjar med ett prefix.
    /// </summary>
    /// <param name="value">Strängen att testa.</param>
    /// <param name="prefix">Prefixet att söka efter.</param>
    /// <param name="caseSensitive">Om true, görs skiftlägeskänslig matchning.</param>
    /// <example>
    /// "hello world".SqlStartsWith("hello") → true
    /// </example>
    public static bool SqlStartsWith(this string value, string prefix, bool caseSensitive = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return value.StartsWith(prefix, comparison);
    }

    /// <summary>
    /// SQL ENDS WITH — kontrollerar om en sträng slutar med ett suffix.
    /// </summary>
    /// <param name="value">Strängen att testa.</param>
    /// <param name="suffix">Suffixet att söka efter.</param>
    /// <param name="caseSensitive">Om true, görs skiftlägeskänslig matchning.</param>
    /// <example>
    /// "hello world".SqlEndsWith("world") → true
    /// </example>
    public static bool SqlEndsWith(this string value, string suffix, bool caseSensitive = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return value.EndsWith(suffix, comparison);
    }

    /// <summary>
    /// SQL IN — kontrollerar om en sträng finns i en given samling.
    /// </summary>
    /// <param name="value">Strängen att testa.</param>
    /// <param name="values">Samlingen att söka i.</param>
    /// <param name="caseSensitive">Om true, görs skiftlägeskänslig matchning.</param>
    /// <example>
    /// "hello".SqlIn(["hi", "hello", "hej"]) → true
    /// </example>
    public static bool SqlIn(this string value, IEnumerable<string> values, bool caseSensitive = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(values);

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return values.Any(v => string.Equals(value, v, comparison));
    }

    /// <summary>
    /// SQL BETWEEN — kontrollerar om en sträng ligger inom ett alfabetiskt intervall.
    /// </summary>
    /// <param name="value">Strängen att testa.</param>
    /// <param name="start">Startvärde (inklusive).</param>
    /// <param name="end">Slutvärde (inklusive).</param>
    /// <param name="caseSensitive">Om true, görs skiftlägeskänslig jämförelse.</param>
    /// <example>
    /// "banana".SqlBetween("apple", "cherry") → true
    /// </example>
    public static bool SqlBetween(this string value, string start, string end, bool caseSensitive = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(start);
        ArgumentNullException.ThrowIfNull(end);

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return string.Compare(value, start, comparison) >= 0
            && string.Compare(value, end, comparison) <= 0;
    }

    /// <summary>
    /// SQL NOT IN — kontrollerar om en sträng INTE finns i en given samling.
    /// </summary>
    /// <param name="value">Strängen att testa.</param>
    /// <param name="values">Samlingen att söka i.</param>
    /// <param name="caseSensitive">Om true, görs skiftlägeskänslig matchning.</param>
    /// <example>
    /// "xyz".SqlNotIn(["hi", "hello", "hej"]) → true
    /// </example>
    public static bool SqlNotIn(this string value, IEnumerable<string> values, bool caseSensitive = false)
    {
        return !value.SqlIn(values, caseSensitive);
    }
}
