using MarcusMedina.Fluent.Data.Sql;

namespace MarcusMedina.Fluent.Data.Sql.Tests;

public class SqlBuilderTests
{
    #region Basic

    [Fact]
    public void Build_SelectStar_ByDefault()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\";");
    }

    [Fact]
    public void Build_WithSelectColumns()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Select("id", "name", "email")
            .Build();

        sql.Should().Be("SELECT \"id\", \"name\", \"email\" FROM \"users\";");
    }

    [Fact]
    public void Build_WithoutTable_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Build();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Table*");
    }

    [Fact]
    public void Build_WhitespaceTable_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Table("  ");
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Conditions

    [Fact]
    public void Is_GeneratesEquals()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("name", "Anna")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = 'Anna';");
    }

    [Fact]
    public void IsNot_GeneratesNotEqual()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .IsNot("name", "Anna")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" <> 'Anna';");
    }

    [Fact]
    public void Is_Numeric_NoQuotes()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("age", 42)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"age\" = 42;");
    }

    [Fact]
    public void Is_Bool_AsBit()
    {
        var sql = new Sql(DatabaseType.SQLite)
            .Table("users")
            .Is("active", true)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"active\" = 1;");
    }

    [Fact]
    public void Contains_GeneratesLike()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Contains("name", "Ann")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" LIKE '%Ann%';");
    }

    [Fact]
    public void StartsWith_GeneratesLikePrefix()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .StartsWith("name", "Ann")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" LIKE 'Ann%';");
    }

    [Fact]
    public void EndsWith_GeneratesLikeSuffix()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .EndsWith("name", "son")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" LIKE '%son';");
    }

    [Fact]
    public void In_GeneratesInClause()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .In("name", "Anna", "Bob", "Charlie")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" IN ('Anna', 'Bob', 'Charlie');");
    }

    [Fact]
    public void NotIn_GeneratesNotInClause()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .NotIn("name", "Anna", "Bob")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" NOT IN ('Anna', 'Bob');");
    }

    [Fact]
    public void In_WithNumbers_NoQuotes()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .In("id", 1, 2, 3)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"id\" IN (1, 2, 3);");
    }

    [Fact]
    public void Between_GeneratesBetween()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Between("age", 18, 65)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"age\" BETWEEN 18 AND 65;");
    }

    [Fact]
    public void Between_StringValues_QuoteThem()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Between("name", "A", "M")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" BETWEEN 'A' AND 'M';");
    }

    [Fact]
    public void IsNull_GeneratesIsNull()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .IsNull("deleted_at")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"deleted_at\" IS NULL;");
    }

    [Fact]
    public void IsNotNull_GeneratesIsNotNull()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .IsNotNull("email")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"email\" IS NOT NULL;");
    }

    [Fact]
    public void When_AppendsRawCondition()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .When("age > 18")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE age > 18;");
    }

    [Fact]
    public void MultipleConditions_AndsThem()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("name", "Anna")
            .When("age > 18")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = 'Anna' AND age > 18;");
    }

    [Fact]
    public void MultipleConditions_AllFluent()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("active", true)
            .Between("age", 18, 65)
            .StartsWith("name", "A")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"active\" = 1 AND \"age\" BETWEEN 18 AND 65 AND \"name\" LIKE 'A%';");
    }

    #endregion

    #region Escaping

    [Fact]
    public void EscapeName_PostgreSQL_UsesDoubleQuotes()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("full name", "test")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"full name\" = 'test';");
    }

    [Fact]
    public void EscapeName_MySQL_UsesBackticks()
    {
        var sql = new Sql(DatabaseType.MySQL)
            .Table("users")
            .Is("name", "Anna")
            .Build();

        sql.Should().Be("SELECT * FROM `users` WHERE `name` = 'Anna';");
    }

    [Fact]
    public void EscapeName_SQLite_UsesDoubleQuotes()
    {
        var sql = new Sql(DatabaseType.SQLite)
            .Table("users")
            .Is("name", "Anna")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = 'Anna';");
    }

    [Fact]
    public void EscapeName_SqlServer_UsesBrackets()
    {
        var sql = new Sql(DatabaseType.SqlServer)
            .Table("users")
            .Is("name", "Anna")
            .Build();

        sql.Should().Be("SELECT * FROM [users] WHERE [name] = 'Anna';");
    }

    [Fact]
    public void EscapeString_SingleQuote_DoublesIt()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("name", "O'Brien")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = 'O''Brien';");
    }

    [Fact]
    public void NullValue_GeneratesNull()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("name", null!)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = NULL;");
    }

    #endregion

    #region Validation

    [Fact]
    public void Is_EmptyField_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Is("", "val");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Is_NullField_Throws()
    {
        string? field = null;
        var act = () => new Sql(DatabaseType.PostgreSQL).Is(field!, "val");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Contains_EmptyField_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Contains("", "val");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Contains_NullValue_Throws()
    {
        string? value = null;
        var act = () => new Sql(DatabaseType.PostgreSQL).Contains("name", value!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void When_EmptyCondition_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).When("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Table_NullOrWhitespace_Throws()
    {
        var act1 = () => new Sql(DatabaseType.PostgreSQL).Table("");
        var act2 = () => new Sql(DatabaseType.PostgreSQL).Table(null!);
        act1.Should().Throw<ArgumentException>();
        act2.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Chaining

    [Fact]
    public void Chaining_FluentApi_AllowsContinuedCalls()
    {
        var sql = new Sql(DatabaseType.MySQL)
            .Table("orders")
            .Select("id", "total")
            .Is("status", "active")
            .When("total > 0")
            .Build();

        sql.Should().Be("SELECT `id`, `total` FROM `orders` WHERE `status` = 'active' AND total > 0;");
    }

    [Fact]
    public void Contains_Apostrophes_DoublesThem()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("items")
            .Contains("name", "it's")
            .Build();

        sql.Should().Be("SELECT * FROM \"items\" WHERE \"name\" LIKE '%it''s%';");
    }

    [Fact]
    public void Build_NoConditions_NoWhereKeyword()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("products")
            .Build();

        sql.Should().Be("SELECT * FROM \"products\";");
    }

    [Fact]
    public void In_SingleValue_GeneratesInClause()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .In("id", 1)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"id\" IN (1);");
    }

    [Fact]
    public void NotIn_SingleValue_GeneratesNotInClause()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .NotIn("id", 999)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"id\" NOT IN (999);");
    }

    #endregion

    #region Or

    [Fact]
    public void Or_ConnectsNextConditionWithOr()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("name", "Anna")
            .Or()
            .Is("name", "Bob")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = 'Anna' OR \"name\" = 'Bob';");
    }

    [Fact]
    public void Or_Multiple_AlternatesCorrectly()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("status", "active")
            .Or()
            .Is("role", "admin")
            .Or()
            .Is("role", "mod")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"status\" = 'active' OR \"role\" = 'admin' OR \"role\" = 'mod';");
    }

    [Fact]
    public void Or_OnlyAffectsNextCondition()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("name", "Anna")
            .Or()
            .Is("name", "Bob")
            .Is("active", true)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = 'Anna' OR \"name\" = 'Bob' AND \"active\" = 1;");
    }

    [Fact]
    public void Or_FirstCondition_UsesAnd()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Or()
            .Is("name", "Anna")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"name\" = 'Anna';");
    }

    #endregion

    #region Join

    [Fact]
    public void Join_InnerJoin_GeneratesInnerJoin()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Join("orders", "orders.user_id = users.id")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" INNER JOIN \"orders\" ON orders.user_id = users.id;");
    }

    [Fact]
    public void Join_LeftJoin_GeneratesLeftJoin()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Join("orders", "orders.user_id = users.id", JoinType.Left)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" LEFT JOIN \"orders\" ON orders.user_id = users.id;");
    }

    [Fact]
    public void Join_RightJoin_GeneratesRightJoin()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Join("orders", "orders.user_id = users.id", JoinType.Right)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" RIGHT JOIN \"orders\" ON orders.user_id = users.id;");
    }

    [Fact]
    public void Join_CrossJoin_GeneratesCrossJoin()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Join("orders", "1=1", JoinType.Cross)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" CROSS JOIN \"orders\" ON 1=1;");
    }

    [Fact]
    public void Join_MultipleJoins()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Join("orders", "orders.user_id = users.id")
            .Join("products", "products.id = orders.product_id", JoinType.Left)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" INNER JOIN \"orders\" ON orders.user_id = users.id LEFT JOIN \"products\" ON products.id = orders.product_id;");
    }

    [Fact]
    public void Join_EmptyTable_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Join("", "x = y");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Join_EmptyOn_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Join("t", "");
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region OrderBy

    [Fact]
    public void OrderBy_Ascending_Default()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .OrderBy("name")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" ORDER BY \"name\" ASC;");
    }

    [Fact]
    public void OrderBy_Descending_WithFalse()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .OrderBy("name", false)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" ORDER BY \"name\" DESC;");
    }

    [Fact]
    public void OrderBy_MultipleFields()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .OrderBy("last_name")
            .OrderBy("first_name")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" ORDER BY \"last_name\" ASC, \"first_name\" ASC;");
    }

    [Fact]
    public void OrderBy_MixedDirection()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .OrderBy("last_name")
            .OrderBy("age", false)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" ORDER BY \"last_name\" ASC, \"age\" DESC;");
    }

    [Fact]
    public void OrderBy_WithWhere()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Is("active", true)
            .OrderBy("name")
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" WHERE \"active\" = 1 ORDER BY \"name\" ASC;");
    }

    [Fact]
    public void OrderBy_EmptyField_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).OrderBy("");
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Limit / Offset

    [Fact]
    public void Limit_GeneratesLimit()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Limit(10)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" LIMIT 10;");
    }

    [Fact]
    public void Limit_WithOffset()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Limit(10)
            .Offset(20)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" LIMIT 10 OFFSET 20;");
    }

    [Fact]
    public void Offset_WithoutLimit()
    {
        var sql = new Sql(DatabaseType.PostgreSQL)
            .Table("users")
            .Offset(5)
            .Build();

        sql.Should().Be("SELECT * FROM \"users\" LIMIT -1 OFFSET 5;");
    }

    [Fact]
    public void Limit_Negative_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Limit(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Offset_Negative_Throws()
    {
        var act = () => new Sql(DatabaseType.PostgreSQL).Offset(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Limit_SqlServer_UsesFetchNext()
    {
        var sql = new Sql(DatabaseType.SqlServer)
            .Table("users")
            .OrderBy("id")
            .Limit(10)
            .Build();

        sql.Should().Be("SELECT * FROM [users] ORDER BY [id] ASC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;");
    }

    [Fact]
    public void Limit_SqlServerWithoutOrderBy_AddsDummyOrder()
    {
        var sql = new Sql(DatabaseType.SqlServer)
            .Table("users")
            .Limit(10)
            .Build();

        sql.Should().Be("SELECT * FROM [users] ORDER BY (SELECT NULL) OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;");
    }

    [Fact]
    public void Offset_SqlServer_RequiresOrderBy()
    {
        var sql = new Sql(DatabaseType.SqlServer)
            .Table("users")
            .Offset(5)
            .Build();

        sql.Should().Be("SELECT * FROM [users] ORDER BY (SELECT NULL) OFFSET 5 ROWS;");
    }

    [Fact]
    public void Limit_SqlServer_WithOffset()
    {
        var sql = new Sql(DatabaseType.SqlServer)
            .Table("users")
            .OrderBy("id")
            .Limit(10)
            .Offset(20)
            .Build();

        sql.Should().Be("SELECT * FROM [users] ORDER BY [id] ASC OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY;");
    }

    #endregion

    #region FullQuery

    [Fact]
    public void FullQuery_AllFeatures()
    {
        var sql = new Sql(DatabaseType.MySQL)
            .Table("users")
            .Select("u.id", "u.name", "o.total")
            .Join("orders", "o.user_id = u.id", JoinType.Left)
            .Is("u.active", true)
            .Or()
            .Is("u.role", "admin")
            .OrderBy("u.name")
            .Limit(25)
            .Offset(0)
            .Build();

        sql.Should().Be("SELECT `u.id`, `u.name`, `o.total` FROM `users` LEFT JOIN `orders` ON o.user_id = u.id WHERE `u.active` = 1 OR `u.role` = 'admin' ORDER BY `u.name` ASC LIMIT 25 OFFSET 0;");
    }

    #endregion
}
