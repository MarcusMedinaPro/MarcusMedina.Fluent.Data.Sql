# MarcusMedina.Fluent.Data.Sql

[![NuGet](https://img.shields.io/nuget/v/MarcusMedina.Fluent.Data.Sql.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/MarcusMedina.Fluent.Data.Sql/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MarcusMedina.Fluent.Data.Sql.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/MarcusMedina.Fluent.Data.Sql/)
[![C#](https://img.shields.io/badge/C%23-14.0-239120?style=for-the-badge&logo=csharp&logoColor=white)](#)
[![.NET](https://img.shields.io/badge/.NET-10.0+-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)
[![Open Source](https://raw.githubusercontent.com/MarcusMedinaPro/MarcusMedina.Fluent.Data.Sql/main/assets/open-source.svg)](https://opensource.org)
[![Build](https://img.shields.io/github/actions/workflow/status/MarcusMedinaPro/MarcusMedina.Fluent.Data.Sql/release.yml?branch=main&label=Build&style=for-the-badge&logo=github)](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data.Sql/actions)
[![Signed](https://img.shields.io/badge/Signed-Sigstore-green?style=for-the-badge&logo=linux)](https://docs.sigstore.dev)

**Fluent SQL query builder for .NET 10+** — generate parameterized SQL strings without a database connection.

Build `SELECT`, `INSERT`, `UPDATE`, `DELETE` statements fluently with automatic escaping per database dialect.

> This came from watching my students struggle with writing SQL by hand. I designed this to help them along the way — though, cheekily, before the course ended they still had to wrestle with raw query strings anyway.
>
> In this case, I wanted a builder that produces the exact same parameterised SQL a student would eventually have to write themselves, so it doubles as a way to check your own work.

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

MIT — see [LICENSE](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data.Sql/blob/main/LICENSE) for details.

---

## Built with Human + AI Collaboration

This library was written by **Marcus Medina** together with **Claude Code** (Anthropic) — not through "vibe coding" where you just describe and accept, but through genuine collaboration: planning together, reviewing each other's decisions, pushing back when something felt wrong, and iterating until the result felt right.

The goal was always to write code worth reading and code worth using — the kind a student can open, understand, and learn from, and the kind any programmer can drop into real, professional work without wanting to rewrite it from scratch. AI was a partner in that process, not a shortcut around it.

If you're curious about this way of working, the source code and git history are open. Every decision has a reason behind it.

## Made for Curious Minds

This library was built with students in mind — not as a black box to copy and paste, but as a real-world example of how clean, purposeful code is written and shared. At the same time, it's built to be genuinely useful in professional projects too — for any developer who's tired of writing the same code over and over.

Whether you're discovering C# for the first time, need a reliable helper for your school project, want a dependable building block for production work, or are simply trying to fall in love with writing code — you're exactly who this was made for.

The source is open. Read it, fork it, break it, improve it. That's the whole point.

---

## Related Projects

- [MarcusMedina.Fluent.Data](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data) — CSV, JSON, XML extensions
- [MarcusMedina.Fluent.Pattern](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Pattern) — String pattern matching and fuzzy search
- [MarcusMedina.Maths.Algebra](https://github.com/MarcusMedinaPro/MarcusMedina.Maths.Algebra) — Algebraic expressions and symbolic math