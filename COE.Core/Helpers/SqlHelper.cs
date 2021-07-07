using System;
using System.Data.SqlClient;

namespace COE.Core.Helpers
{
    public class SqlHelper : IDisposable
    {
        private SqlConnection _connection;
        private string _sqlConnectionString;

        public SqlHelper(string sqlConnectionString)
        {
            this._sqlConnectionString = sqlConnectionString;
        }

        private SqlConnection OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_sqlConnectionString);
                _connection.Open();
            }
            return _connection;
        }

        private int GetRowCount(string tableName, string filter = "")
        {
            var query =
                $@"SELECT COUNT(*)
                   FROM {tableName}
                    {filter}";

            using (SqlCommand cmdCount = new SqlCommand(query, OpenConnection()))
            {
                return (int)cmdCount.ExecuteScalar();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
