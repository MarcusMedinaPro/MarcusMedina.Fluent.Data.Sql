# MarcusMedina.Fluent.Data.Sql

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/packages/MarcusMedina.Fluent.Data.Sql/)
[![Tests](https://img.shields.io/badge/tests-65%20passed-brightgreen)]()
[![Signed](https://img.shields.io/badge/Signed-Sigstore-green?style=for-the-badge&logo=linux)](https://docs.sigstore.dev)

**Fluent SQL query builder for .NET 10+** — generate parameterized SQL strings without a database connection.

Build `SELECT`, `INSERT`, `UPDATE`, `DELETE` statements fluently with automatic escaping per database dialect.

---

## Features

- ✅ **Multi-dialect** — PostgreSQL, MySQL, SQLite, SQL Server (configurable via `DatabaseType`)
- ✅ **SELECT** — Columns, WHERE, ORDER BY, LIMIT, OFFSET, JOIN
- ✅ **INSERT** — Single and multi-row
- ✅ **UPDATE** — With WHERE, JOIN support
- ✅ **DELETE** — With WHERE, JOIN support
- ✅ **Automatic escaping** — Quoted identifiers per dialect (`` ` ``, `"`, `[]`)
- ✅ **Composable WHERE** — `And()`, `Or()`, nested conditions
- ✅ **Zero dependencies** — Pure .NET, no external packages

---

## Installation

```bash
dotnet add package MarcusMedina.Fluent.Data.Sql
```

**Requirements:** .NET 10.0+, C# 14.0+

---

## Quick Start

```csharp
using MarcusMedina.Fluent.Data.Sql;

// SELECT
var sql = new Sql(DatabaseType.PostgreSQL)
    .Table("users")
    .Select("id", "name", "email")
    .Where("active = 1")
    .OrderBy("name")
    .Build();

// => SELECT id, name, email FROM users WHERE active = 1 ORDER BY name ASC

// INSERT
var insert = new Sql(DatabaseType.MySQL)
    .Table("users")
    .Insert(new { name = "Alice", email = "alice@example.com" })
    .Build();

// => INSERT INTO users (name, email) VALUES ('Alice', 'alice@example.com')

// UPDATE
var update = new Sql(DatabaseType.SQLite)
    .Table("users")
    .Set("name", "Bob")
    .Where("id = 42")
    .Build();

// => UPDATE users SET name = 'Bob' WHERE id = 42

// DELETE
var delete = new Sql(DatabaseType.SqlServer)
    .Table("users")
    .Where("id = 42")
    .Build();

// => DELETE FROM users WHERE id = 42
```

---

## API Overview

| Method | Description |
|--------|-------------|
| `Sql(dialect)` | Create builder with database dialect |
| `Table(name)` | Set target table |
| `Select(columns)` | Set SELECT columns |
| `Insert(values)` | Build INSERT with object/ anonymous type |
| `Set(column, value)` | Add SET clause for UPDATE |
| `Where(condition)` | Add WHERE condition |
| `And()` / `Or()` | Chain conditions |
| `OrderBy(column, asc)` | Add ORDER BY |
| `Limit(n)` | Add LIMIT |
| `Offset(n)` | Add OFFSET |
| `Join(table, on, type)` | Add JOIN (INNER, LEFT, RIGHT) |
| `Build()` | Generate final SQL string |

---

## Database Dialects

| Dialect | Identifier Quotes | LIMIT/OFFSET |
|---------|------------------|--------------|
| PostgreSQL | `"..."` | `LIMIT n OFFSET n` |
| MySQL | `` `...` `` | `LIMIT n OFFSET n` |
| SQLite | `"..."` | `LIMIT n OFFSET n` |
| SQL Server | `[...]` | `OFFSET n ROWS FETCH NEXT n ROWS ONLY` |

---

## Testing

```bash
dotnet test --configuration Release
```

Tests: **65 passed** — covering all dialects, query types, edge cases, and escaping.

---

## License

MIT — see [LICENSE](LICENSE) for details.

---

## Related Projects

- [MarcusMedina.Fluent.Data](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data) — CSV, JSON, XML extensions
- [MarcusMedina.Fluent.Pattern](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Pattern) — String pattern matching and fuzzy search
- [MarcusMedina.Maths.Algebra](https://github.com/MarcusMedinaPro/MarcusMedina.Maths.Algebra) — Algebraic expressions and symbolic math