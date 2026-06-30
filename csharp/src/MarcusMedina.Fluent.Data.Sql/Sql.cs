using System.Text;

namespace MarcusMedina.Fluent.Data.Sql;

/// <summary>Databastyp för SQL-syntaxanpassning.</summary>
public enum DatabaseType
{
    /// <summary>PostgreSQL quoting: "name"</summary>
    PostgreSQL,
    /// <summary>MySQL quoting: `name`</summary>
    MySQL,
    /// <summary>SQLite quoting: "name"</summary>
    SQLite,
    /// <summary>SQL Server quoting: [name]</summary>
    SqlServer
}

/// <summary>Typ av JOIN.</summary>
public enum JoinType
{
    /// <summary>INNER JOIN</summary>
    Inner,
    /// <summary>LEFT JOIN</summary>
    Left,
    /// <summary>RIGHT JOIN</summary>
    Right,
    /// <summary>FULL OUTER JOIN</summary>
    Full,
    /// <summary>CROSS JOIN</summary>
    Cross
}

/// <summary>
/// Fluent SQL query builder. Bygger SQL-strängar med en kedjbar API.
/// Inspirationskälla för studerande att bygga egna querybyggare.
/// </summary>
/// <example>
/// new Sql(DatabaseType.PostgreSQL)
///     .Table("users")
///     .Is("name", "Anna")
///     .When("age > 18")
///     .OrderBy("name")
///     .Limit(10)
///     .Build()
/// // → SELECT * FROM "users" WHERE "name" = 'Anna' AND age > 18 ORDER BY "name" ASC LIMIT 10;
/// </example>
public class Sql
{
    private readonly DatabaseType _dbType;
    private string _table = "";
    private string[] _columns = ["*"];
    private readonly List<(string clause, bool isOr)> _where = [];
    private bool _nextOr;
    private readonly List<(string field, bool asc)> _orderBy = [];
    private int? _limit;
    private int? _offset;
    private readonly List<(JoinType type, string table, string on)> _joins = [];

    /// <summary>Skapar en ny SQL-byggare för angiven databastyp.</summary>
    /// <param name="dbType">Databastyp som styr escaping av namn.</param>
    public Sql(DatabaseType dbType)
    {
        _dbType = dbType;
    }

    /// <summary>Vilken tabell frågan gäller.</summary>
    public Sql Table(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _table = name;
        return this;
    }

    /// <summary>Vilka kolumner som ska selectas. Default: *</summary>
    public Sql Select(params string[] columns)
    {
        _columns = columns.Length > 0 ? columns : ["*"];
        return this;
    }

    /// <summary>Nästa villkor blir OR istället för AND.</summary>
    public Sql Or()
    {
        _nextOr = true;
        return this;
    }

    /// <summary>Lägg till ett raw-villkor direkt, t.ex. .When("age &gt; 18")</summary>
    public Sql When(string rawCondition)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawCondition);
        AppendWhere(rawCondition);
        return this;
    }

    /// <summary>WHERE field = value</summary>
    public Sql Is(string field, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        AppendWhere($"{EscapeName(field)} = {EscapeValue(value)}");
        return this;
    }

    /// <summary>WHERE field &lt;&gt; value</summary>
    public Sql IsNot(string field, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        AppendWhere($"{EscapeName(field)} <> {EscapeValue(value)}");
        return this;
    }

    /// <summary>WHERE field LIKE '%value%'</summary>
    public Sql Contains(string field, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentNullException.ThrowIfNull(value);
        AppendWhere($"{EscapeName(field)} LIKE '%{EscapeString(value)}%'");
        return this;
    }

    /// <summary>WHERE field LIKE 'value%'</summary>
    public Sql StartsWith(string field, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentNullException.ThrowIfNull(value);
        AppendWhere($"{EscapeName(field)} LIKE '{EscapeString(value)}%'");
        return this;
    }

    /// <summary>WHERE field LIKE '%value'</summary>
    public Sql EndsWith(string field, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentNullException.ThrowIfNull(value);
        AppendWhere($"{EscapeName(field)} LIKE '%{EscapeString(value)}'");
        return this;
    }

    /// <summary>WHERE field IN (value, ...)</summary>
    public Sql In(string field, params object[] values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentNullException.ThrowIfNull(values);
        var escaped = string.Join(", ", values.Select(EscapeValue));
        AppendWhere($"{EscapeName(field)} IN ({escaped})");
        return this;
    }

    /// <summary>WHERE field NOT IN (value, ...)</summary>
    public Sql NotIn(string field, params object[] values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentNullException.ThrowIfNull(values);
        var escaped = string.Join(", ", values.Select(EscapeValue));
        AppendWhere($"{EscapeName(field)} NOT IN ({escaped})");
        return this;
    }

    /// <summary>WHERE field BETWEEN a AND b</summary>
    public Sql Between(string field, object a, object b)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        AppendWhere($"{EscapeName(field)} BETWEEN {EscapeValue(a)} AND {EscapeValue(b)}");
        return this;
    }

    /// <summary>WHERE field IS NULL</summary>
    public Sql IsNull(string field)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        AppendWhere($"{EscapeName(field)} IS NULL");
        return this;
    }

    /// <summary>WHERE field IS NOT NULL</summary>
    public Sql IsNotNull(string field)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        AppendWhere($"{EscapeName(field)} IS NOT NULL");
        return this;
    }

    /// <summary>Lägg till en JOIN. Anropa efter .Table().</summary>
    public Sql Join(string table, string on, JoinType type = JoinType.Inner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(table);
        ArgumentException.ThrowIfNullOrWhiteSpace(on);
        _joins.Add((type, table, on));
        return this;
    }

    /// <summary>ORDER BY-fält.</summary>
    public Sql OrderBy(string field, bool asc = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        _orderBy.Add((field, asc));
        return this;
    }

    /// <summary>LIMIT n (max antal rader).</summary>
    public Sql Limit(int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Limit must be non-negative.");
        _limit = count;
        return this;
    }

    /// <summary>OFFSET n (hoppa över rader). Kräver LIMIT för SQL Server.</summary>
    public Sql Offset(int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Offset must be non-negative.");
        _offset = count;
        return this;
    }

    /// <summary>Bygg den fullständiga SQL-strängen.</summary>
    public string Build()
    {
        if (string.IsNullOrWhiteSpace(_table))
            throw new InvalidOperationException("Table must be specified. Call .Table(name) before .Build().");

        var sb = new StringBuilder();
        BuildSelect(sb);
        BuildFrom(sb);
        BuildJoins(sb);
        BuildWhere(sb);
        BuildOrderBy(sb);
        BuildLimitOffset(sb);
        sb.Append(';');
        return sb.ToString();
    }

    private void AppendWhere(string clause)
    {
        _where.Add((clause, _nextOr));
        _nextOr = false;
    }

    private void BuildSelect(StringBuilder sb)
    {
        sb.Append("SELECT ");
        sb.Append(string.Join(", ", _columns.Select(c => c == "*" ? c : EscapeName(c))));
    }

    private void BuildFrom(StringBuilder sb)
    {
        sb.Append(" FROM ");
        sb.Append(EscapeName(_table));
    }

    private void BuildJoins(StringBuilder sb)
    {
        foreach (var (type, table, on) in _joins)
        {
            var keyword = type switch
            {
                JoinType.Inner => "INNER JOIN",
                JoinType.Left => "LEFT JOIN",
                JoinType.Right => "RIGHT JOIN",
                JoinType.Full => "FULL JOIN",
                JoinType.Cross => "CROSS JOIN",
                _ => "INNER JOIN"
            };
            sb.Append($" {keyword} {EscapeName(table)} ON {on}");
        }
    }

    private void BuildWhere(StringBuilder sb)
    {
        if (_where.Count == 0) return;
        sb.Append(" WHERE ");
        for (int i = 0; i < _where.Count; i++)
        {
            if (i > 0)
                sb.Append(_where[i].isOr ? " OR " : " AND ");
            sb.Append(_where[i].clause);
        }
    }

    private void BuildOrderBy(StringBuilder sb)
    {
        if (_orderBy.Count == 0) return;
        sb.Append(" ORDER BY ");
        sb.Append(string.Join(", ", _orderBy.Select(o => $"{EscapeName(o.field)} {(o.asc ? "ASC" : "DESC")}")));
    }

    private void BuildLimitOffset(StringBuilder sb)
    {
        if (!_limit.HasValue && !_offset.HasValue) return;

        if (_dbType == DatabaseType.SqlServer)
            BuildLimitOffsetSqlServer(sb);
        else
            BuildLimitOffsetStandard(sb);
    }

    private void BuildLimitOffsetStandard(StringBuilder sb)
    {
        if (_limit.HasValue)
        {
            sb.Append($" LIMIT {_limit}");
            if (_offset.HasValue) sb.Append($" OFFSET {_offset}");
        }
        else if (_offset.HasValue)
        {
            sb.Append($" LIMIT -1 OFFSET {_offset}");
        }
    }

    private void BuildLimitOffsetSqlServer(StringBuilder sb)
    {
        if (_orderBy.Count == 0)
            sb.Append(" ORDER BY (SELECT NULL)");

        sb.Append($" OFFSET {_offset ?? 0} ROWS");
        if (_limit.HasValue)
            sb.Append($" FETCH NEXT {_limit} ROWS ONLY");
    }

    private string EscapeName(string name)
    {
        var quote = _dbType switch
        {
            DatabaseType.PostgreSQL => '"',
            DatabaseType.MySQL => '`',
            DatabaseType.SQLite => '"',
            DatabaseType.SqlServer => '[',
            _ => ' '
        };

        if (_dbType == DatabaseType.SqlServer)
            return $"[{name}]";

        return $"{quote}{name}{quote}";
    }

    private string EscapeValue(object? value)
    {
        if (value is null) return "NULL";
        if (value is int or long or short or byte or float or double or decimal)
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "NULL";
        if (value is bool b) return b ? "1" : "0";
        return $"'{EscapeString(value.ToString() ?? string.Empty)}'";
    }

    private static string EscapeString(string value)
    {
        return value
            .Replace("'", "''")
            .Replace("\\", "\\\\");
    }
}
