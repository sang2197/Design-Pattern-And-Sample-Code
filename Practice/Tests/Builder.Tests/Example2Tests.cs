using Exercises.Builder.Example2;
using Xunit;

namespace Builder.Tests;

public class Example2Tests
{
    [Fact]
    public void ToSql_WithColumnsAndConditions_BuildsFullQuery()
    {
        SqlQuery query = new SqlQueryBuilder()
            .Select("Id", "Name", "Price")
            .From("Products")
            .Where("Price > 100000")
            .Where("IsActive = 1")
            .Build();

        Assert.Equal("SELECT Id, Name, Price FROM Products WHERE Price > 100000 AND IsActive = 1", query.ToSql());
    }

    [Fact]
    public void ToSql_WithoutColumnsOrConditions_UsesStarAndNoWhere()
    {
        SqlQuery query = new SqlQueryBuilder().From("Products").Build();
        Assert.Equal("SELECT * FROM Products", query.ToSql());
    }

    [Fact]
    public void Build_WithoutTable_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => new SqlQueryBuilder().Build());
    }
}
