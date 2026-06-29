using MarcusMedina.Fluent.Data.Sql.Extensions;

namespace MarcusMedina.Fluent.Data.Sql.Tests.Extensions;

public class StringSqlExtensionsTests
{
    #region IsLike

    [Fact]
    public void IsLike_PercentWildcard_MatchesAnySequence()
    {
        "hello world".IsLike("hello%").Should().BeTrue();
    }

    [Fact]
    public void IsLike_PercentWildcard_Suffix()
    {
        "hello world".IsLike("%world").Should().BeTrue();
    }

    [Fact]
    public void IsLike_Underscore_MatchesSingleChar()
    {
        "hello world".IsLike("hello_world").Should().BeTrue();
    }

    [Fact]
    public void IsLike_PercentBothEnds()
    {
        "hello world".IsLike("h%d").Should().BeTrue();
    }

    [Fact]
    public void IsLike_CaseInsensitiveByDefault()
    {
        "hello world".IsLike("HELLO%").Should().BeTrue();
    }

    [Fact]
    public void IsLike_CaseSensitive_FailsOnCaseMismatch()
    {
        "hello world".IsLike("HELLO%", caseSensitive: true).Should().BeFalse();
    }

    [Fact]
    public void IsLike_ExactMatch_ReturnsTrue()
    {
        "hello".IsLike("hello").Should().BeTrue();
    }

    [Fact]
    public void IsLike_NoMatch_ReturnsFalse()
    {
        "hello".IsLike("world").Should().BeFalse();
    }

    [Fact]
    public void IsLike_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.IsLike("%");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsLike_NullPattern_ThrowsArgumentNullException()
    {
        string? pattern = null;
        var act = () => "hello".IsLike(pattern!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region SqlContains

    [Fact]
    public void SqlContains_FindsSubstring_ReturnsTrue()
    {
        "hello world".SqlContains("world").Should().BeTrue();
    }

    [Fact]
    public void SqlContains_CaseInsensitiveByDefault()
    {
        "hello world".SqlContains("WORLD").Should().BeTrue();
    }

    [Fact]
    public void SqlContains_CaseSensitive_FailsOnCaseMismatch()
    {
        "hello world".SqlContains("WORLD", caseSensitive: true).Should().BeFalse();
    }

    [Fact]
    public void SqlContains_NotFound_ReturnsFalse()
    {
        "hello world".SqlContains("xyz").Should().BeFalse();
    }

    [Fact]
    public void SqlContains_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.SqlContains("test");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlContains_NullSearchTerm_ThrowsArgumentNullException()
    {
        string? term = null;
        var act = () => "hello".SqlContains(term!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region SqlStartsWith

    [Fact]
    public void SqlStartsWith_MatchingPrefix_ReturnsTrue()
    {
        "hello world".SqlStartsWith("hello").Should().BeTrue();
    }

    [Fact]
    public void SqlStartsWith_CaseInsensitiveByDefault()
    {
        "hello world".SqlStartsWith("HELLO").Should().BeTrue();
    }

    [Fact]
    public void SqlStartsWith_CaseSensitive_FailsOnCaseMismatch()
    {
        "hello world".SqlStartsWith("HELLO", caseSensitive: true).Should().BeFalse();
    }

    [Fact]
    public void SqlStartsWith_NoMatch_ReturnsFalse()
    {
        "hello world".SqlStartsWith("world").Should().BeFalse();
    }

    [Fact]
    public void SqlStartsWith_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.SqlStartsWith("hello");
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region SqlEndsWith

    [Fact]
    public void SqlEndsWith_MatchingSuffix_ReturnsTrue()
    {
        "hello world".SqlEndsWith("world").Should().BeTrue();
    }

    [Fact]
    public void SqlEndsWith_CaseInsensitiveByDefault()
    {
        "hello world".SqlEndsWith("WORLD").Should().BeTrue();
    }

    [Fact]
    public void SqlEndsWith_CaseSensitive_FailsOnCaseMismatch()
    {
        "hello world".SqlEndsWith("WORLD", caseSensitive: true).Should().BeFalse();
    }

    [Fact]
    public void SqlEndsWith_NoMatch_ReturnsFalse()
    {
        "hello world".SqlEndsWith("hello").Should().BeFalse();
    }

    [Fact]
    public void SqlEndsWith_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.SqlEndsWith("world");
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region SqlIn

    [Fact]
    public void SqlIn_MatchingValue_ReturnsTrue()
    {
        "hello".SqlIn(["hello", "world"]).Should().BeTrue();
    }

    [Fact]
    public void SqlIn_CaseInsensitiveByDefault()
    {
        "HELLO".SqlIn(["hello", "world"]).Should().BeTrue();
    }

    [Fact]
    public void SqlIn_CaseSensitive_FailsOnCaseMismatch()
    {
        "HELLO".SqlIn(["hello", "world"], caseSensitive: true).Should().BeFalse();
    }

    [Fact]
    public void SqlIn_NoMatch_ReturnsFalse()
    {
        "test".SqlIn(["hello", "world"]).Should().BeFalse();
    }

    [Fact]
    public void SqlIn_EmptyCollection_ReturnsFalse()
    {
        "hello".SqlIn([]).Should().BeFalse();
    }

    [Fact]
    public void SqlIn_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.SqlIn(["hello"]);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlIn_NullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<string>? values = null;
        var act = () => "hello".SqlIn(values!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region SqlBetween

    [Fact]
    public void SqlBetween_ValueInRange_ReturnsTrue()
    {
        "bob".SqlBetween("alice", "charlie").Should().BeTrue();
    }

    [Fact]
    public void SqlBetween_ValueAtStart_ReturnsTrue()
    {
        "alice".SqlBetween("alice", "charlie").Should().BeTrue();
    }

    [Fact]
    public void SqlBetween_ValueAtEnd_ReturnsTrue()
    {
        "charlie".SqlBetween("alice", "charlie").Should().BeTrue();
    }

    [Fact]
    public void SqlBetween_ValueOutsideRange_ReturnsFalse()
    {
        "dave".SqlBetween("alice", "charlie").Should().BeFalse();
    }

    [Fact]
    public void SqlBetween_CaseInsensitiveByDefault()
    {
        "BOB".SqlBetween("alice", "charlie").Should().BeTrue();
    }

    [Fact]
    public void SqlBetween_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.SqlBetween("a", "z");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlBetween_NullStart_ThrowsArgumentNullException()
    {
        string? start = null;
        var act = () => "hello".SqlBetween(start!, "z");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlBetween_NullEnd_ThrowsArgumentNullException()
    {
        string? end = null;
        var act = () => "hello".SqlBetween("a", end!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region SqlNotIn

    [Fact]
    public void SqlNotIn_NonMatchingValue_ReturnsTrue()
    {
        "test".SqlNotIn(["hello", "world"]).Should().BeTrue();
    }

    [Fact]
    public void SqlNotIn_MatchingValue_ReturnsFalse()
    {
        "hello".SqlNotIn(["hello", "world"]).Should().BeFalse();
    }

    [Fact]
    public void SqlNotIn_CaseInsensitiveByDefault()
    {
        "HELLO".SqlNotIn(["hello", "world"]).Should().BeFalse();
    }

    [Fact]
    public void SqlNotIn_CaseSensitive_FailsOnCaseMismatch()
    {
        "HELLO".SqlNotIn(["hello", "world"], caseSensitive: true).Should().BeTrue();
    }

    [Fact]
    public void SqlNotIn_EmptyCollection_ReturnsTrue()
    {
        "hello".SqlNotIn([]).Should().BeTrue();
    }

    [Fact]
    public void SqlNotIn_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.SqlNotIn(["hello"]);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion
}
