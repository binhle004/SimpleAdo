using SimpleAdo.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SimpleAdo
{
    public partial class SqlHelper
    {
        #region Private Fields

        private readonly string _commandText;
        private readonly string _connectionString;
        private readonly CommandType _commandType;

        private List<SqlParameter> _parameters = new List<SqlParameter>();

        #endregion

        #region Constructors

        public SqlHelper(string commandText)
        {
            _commandText = commandText;
            _commandType = _commandType == 0 ? CommandType.Text : _commandType;
            _connectionString = _connectionString ?? DefaultConnectionString.ConnectionString;
        }

        public SqlHelper(string commandText, string connectionString) : this(commandText)
        {
            _connectionString = connectionString;
        }

        public SqlHelper(string commandText, CommandType commandType) : this(commandText)
        {
            _commandType = commandType;
        }

        public SqlHelper(string commandText, CommandType commandType, string connectionString)
        {
            _commandText = commandText;
            _commandType = commandType;
            _connectionString = connectionString;
        }

        #endregion

        #region AddParam Methods

        public SqlHelper AddParam(string parameterName, object value)
        {
            _parameters.Add(new SqlParameter(parameterName, value));
            return this;
        }

        public SqlHelper AddParam(SqlParameter parameter)
        {
            _parameters.Add(parameter);
            return this;
        }

        public SqlHelper AddParam(IEnumerable<SqlParameter> parameters)
        {
            _parameters.AddRange(parameters);
            return this;
        }

        #endregion

        #region Synchronous Execute Methods


        public object ExecuteScalar()
        {
            return Process(c => c.ExecuteScalar());
        }

        public int ExecuteNonQuery()
        {
            return Process(c => c.ExecuteNonQuery());
        }

        public T ExecuteReader<T>(Func<SqlDataReader, T> loader) where T : new()
        {
            return Process(c => SingleRowExecuteReader(c, loader));
        }
        public T ExecuteReader<T>() where T : ISqlLoader, new()
        {
            return Process(c => SingleRowExecuteReader(c, r => (T)new T().Load(r)));
        }

        public IEnumerable<T> ExecuteReaderList<T>(Func<SqlDataReader, T> loader) where T : new()
        {
            return Process(c => MultiRowExecuteReader(c, loader));
        }
        public IEnumerable<T> ExecuteReaderList<T>() where T : ISqlLoader, new()
        {
            return Process(c => MultiRowExecuteReader(c, r => (T)new T().Load(r)));
        }

        private T Process<T>(Func<SqlCommand, T> executer)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(_commandText, conn))
            {
                cmd.CommandType = _commandType;

                if (_parameters.Count > 0)
                    cmd.Parameters.AddRange(_parameters.ToArray());

                conn.Open();

                return executer(cmd);
            }
        }

        private T SingleRowExecuteReader<T>(SqlCommand cmd, Func<SqlDataReader, T> loader)
        {
            using (var reader = cmd.ExecuteReader())
                return reader.Read() ? loader(reader) : default(T);
        }

        private IEnumerable<T> MultiRowExecuteReader<T>(SqlCommand cmd, Func<SqlDataReader, T> loader) where T : new()
        {
            var list = new List<T>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    list.Add(loader(reader));
            }

            return list;
        }

        public T Execute<T>(Func<SqlCommand, T> command) where T : new()
        {
            return Process(c => command(c));
        }

        #endregion

        #region Async Execute Methods

        public async Task<int> ExecuteNonQueryAsync()
        {
            return await ProcessAsync(c => c.ExecuteNonQueryAsync());
        }

        public async Task<object> ExecuteScalarAsync()
        {
            return await ProcessAsync(c => c.ExecuteScalarAsync());
        }

        public async Task<T> ExecuteReaderAsync<T>(Func<SqlDataReader, T> loader) where T : new()
        {
            return await ProcessAsync(c => SingleRowReaderAsync(c, loader));
        }

        private async Task<T> ProcessAsync<T>(Func<SqlCommand, Task<T>> executerAsync)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(_commandText, conn))
            {
                cmd.CommandType = _commandType;

                if (_parameters.Count > 0)
                    cmd.Parameters.AddRange(_parameters.ToArray());

                conn.Open();

                return await executerAsync(cmd);
            }
        }

        private async Task<T> SingleRowReaderAsync<T>(SqlCommand cmd, Func<SqlDataReader, T> loader)
        {
            using (var reader = await cmd.ExecuteReaderAsync())
                return reader.Read() ? loader(reader) : default(T);
        }

        #endregion
    }
}
