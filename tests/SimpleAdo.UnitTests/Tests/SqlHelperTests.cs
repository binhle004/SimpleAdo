using NUnit.Framework;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using SimpleAdo;

//[TestFixture]
//public class SqlHelperTests
//{
//    [Test]
//    public void SqlHelper_AddParam_ReturnsNotNull()
//    {
//        string sql = "";
//        var sqlHelper = new SqlHelper(sql);

//        var param1 = new SqlParameter("Param", "Param1Value");
//        sqlHelper.AddParam(param1);
//        sqlHelper.AddParam("Param2", "Param2Value");
//        sqlHelper.AddParam("Param3", 33);

//        //sqlHelper._parameters = new List<SqlParameter>() { param3 };

//        sqlHelper._parameters.ForEach(p => p.SqlValue.WriteToConsole());
//    }

//}
