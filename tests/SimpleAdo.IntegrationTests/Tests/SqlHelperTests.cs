using SimpleAdo;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Linq;

[TestFixture]
public class SqlHelperTests
{
    [Test]
    public async Task ExecuteNonQueryAsync_ReturnValue_GreaterThanZero()
    {
        string sql = "UPDATE Product SET DateModified = SYSDATETIME() WHERE Id = @Id";

        int rowsChanged = await new SqlHelper(sql).AddParam("Id", 1).ExecuteNonQueryAsync();

        Assert.That(rowsChanged > 0);
    }

    [Test]
    public void ExecuteNonQuery_ReturnValue_GreaterThanZero()
    {
        string sql = @"
            UPDATE Product SET DateModified = SYSDATETIME() WHERE Id = @Id
        ";

        int rowsChanged = new SqlHelper(sql).AddParam("Id", 1).ExecuteNonQuery();

        Assert.That(rowsChanged > 0);
    }

    [Test]
    public async Task ExecuteScalarAsync_GetValue_IsNotNull()
    {
        string sql = "SELECT TOP 1 Name FROM Product";

        var name = await new SqlHelper(sql).ExecuteScalarAsync();

        name.WriteToConsole();

        Assert.That(name, Is.Not.Null);
    }

    [Test]
    public void ExecuteScalar_GetValue_IsNotNull()
    {
        string sql = "SELECT TOP 1 Name FROM Product";

        var name = new SqlHelper(sql).ExecuteScalar().ToString();

        name.WriteToConsole();

        Assert.That(name, Is.Not.Null);
    }

    [Test]
    public async Task ExecuteReaderAsync_ExternalLoader_ReturnsNotNull()
    {
        string sql = "SELECT TOP 1 * FROM Product";

        var product = await new SqlHelper(sql).ExecuteReaderAsync(r => Load(r));

        product.WriteToConsole();

        Assert.That(product, Is.Not.Null);
    }

    [Test]
    public void ExecuteReader_ExternalLoader_ReturnsNotNull()
    {
        string sql = "SELECT TOP 1 * FROM Product";

        var product = new SqlHelper(sql).ExecuteReader(r => Load(r));

        product.WriteToConsole();

        Assert.That(product, Is.Not.Null);
    }


    [Test]
    public void ExecuteReader_InternalLoader_ReturnsNotNull()
    {
        string sql = "SELECT TOP 1 * FROM Product";

        var product = new SqlHelper(sql).ExecuteReader<Product>();

        product.WriteToConsole();

        Assert.That(product, Is.Not.Null);
    }

    [Test]
    public void ExecuteReaderList_InternalLoader_ReturnsItems()
    {
        string sql = "SELECT * FROM Product";

        var products = new SqlHelper(sql).ExecuteReaderList<Product>();

        products.WriteToConsole();

        Assert.That(products.Count() > 0);
    }

    [Test]
    public void ExecuteReaderList_ExternalLoader_ReturnsItems()
    {
        string sql = "SELECT * FROM Product";

        var products = new SqlHelper(sql).ExecuteReaderList(r => Load(r));

        products.WriteToConsole();

        Assert.That(products.Count() > 0);
    }

    [Test]
    public void Execute_ReturnValue_GreaterThanZero()
    {
        string sql = "SELECT * FROM Product";

        var products = new SqlHelper(sql).Execute(c => {

            var results = new List<Product>();

            using (var reader = c.ExecuteReader())
            {
                while (reader.Read())
                    results.Add(Load(reader));
            }

            return results;
        });

        products.WriteToConsole();

        Assert.That(products, Is.Not.Null);
    }

    private Product Load(SqlDataReader reader)
    {
        return new Product()
        {
            Id = (int)reader["Id"],
            Name = reader["Name"].ToString(),
            DateModified = (DateTime)reader["DateModified"]
        };
    }
}