using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SimpleAdo.Config
{
    public static class DefaultConnectionString
    {
        private static string _connectionStringName = "Default";
        private static string _applicationName;

        public static string ConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName];

                if (connectionString == null)
                    throw new ArgumentException(string.Format("The connection string named \"{0}\" does not exist in app.config or web.config", _connectionStringName));

                var sb = new SqlConnectionStringBuilder(connectionString.ToString());
                sb.ApplicationName = _applicationName ?? sb.ApplicationName;

                return sb.ToString();
            }
        }

        public static void SetName(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }
        public static void SetApplicationName(string applicationName)
        {
            _applicationName = applicationName;
        }
    }
}