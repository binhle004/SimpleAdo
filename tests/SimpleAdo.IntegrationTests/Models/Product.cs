using SimpleAdo;
using System;
using System.Data.SqlClient;

public class Product : ISqlLoader
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateModified { get; set; }

    public ISqlLoader Load(SqlDataReader reader)
    {
        return new Product()
        {
            Id = (int)reader["Id"],
            Name = reader["Name"].ToString(),
            DateModified = (DateTime)reader["DateModified"],
        };
    }
}