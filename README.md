# MarcusMedina.Fluent.Data.Sql

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/packages/MarcusMedina.Fluent.Data.Sql/)
[![Tests](https://img.shields.io/badge/tests-41%20passed-brightgreen)]()

**SQL-style string extensions for pattern matching in .NET 10+**

Perform SQL-style pattern matching — `LIKE`, `IN`, `BETWEEN` — directly on strings without a database connection.

---

## Features

- ✅ **IsLike** — SQL `LIKE` with `%` and `_` wildcards
- ✅ **SqlContains** — `LIKE '%value%'` shorthand
- ✅ **SqlStartsWith** — `LIKE 'value%'` shorthand
- ✅ **SqlEndsWith** — `LIKE '%value'` shorthand
- ✅ **SqlIn** — SQL `IN` comparison against a list
- ✅ **SqlBetween** — SQL `BETWEEN` range comparison
- ✅ **SqlNotIn** — SQL `NOT IN` negation
- ✅ **Culture-aware** — Optional `StringComparison` parameter
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

// SQL LIKE pattern matching
"hello".IsLike("h%o");           // true
"hello".IsLike("h_llo");         // true
"hello".IsLike("%ll%");          // true

// Convenience methods
"hello world".SqlContains("world");    // true
"hello".SqlStartsWith("he");           // true
"hello".SqlEndsWith("lo");             // true

// Set membership
"apple".SqlIn("apple", "banana", "cherry");  // true
"grape".SqlNotIn("apple", "banana");          // true

// Range comparison
"m".SqlBetween("a", "z");  // true (lexicographic)
```

---

## API Overview

| Method | SQL Equivalent | Description |
|--------|---------------|-------------|
| `IsLike(pattern)` | `LIKE pattern` | Match with `%` (any) and `_` (single) wildcards |
| `SqlContains(value)` | `LIKE '%value%'` | Contains substring |
| `SqlStartsWith(value)` | `LIKE 'value%'` | Starts with prefix |
| `SqlEndsWith(value)` | `LIKE '%value'` | Ends with suffix |
| `SqlIn(values)` | `IN (...)` | Match any of the provided values |
| `SqlNotIn(values)` | `NOT IN (...)` | Match none of the provided values |
| `SqlBetween(min, max)` | `BETWEEN min AND max` | Lexicographic range check |

All methods accept an optional `StringComparison` parameter for culture-aware matching.

---

## Testing

```bash
dotnet test --configuration Release
```

Tests: **41 passed** — covering case sensitivity, wildcards, edge cases, and culture-aware comparisons.

---

## License

MIT — see [LICENSE](LICENSE) for details.

---

## Related Projects

- [MarcusMedina.Fluent.Data](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data) — CSV, JSON, XML extensions
- [MarcusMedina.Maths.Algebra](https://github.com/MarcusMedinaPro/MarcusMedina.Maths.Algebra) — Algebraic expressions and symbolic math
