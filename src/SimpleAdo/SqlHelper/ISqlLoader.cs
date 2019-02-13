using System.Data.SqlClient;

namespace SimpleAdo
{
    public interface ISqlLoader
    {
        ISqlLoader Load(SqlDataReader reader);
    }
}