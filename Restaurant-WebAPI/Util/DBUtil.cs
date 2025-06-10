using System;
using System.Configuration;
using System.Data;
using Npgsql;

namespace Restaurant_WebAPI.Util
{
    public static class DBUtil
    {
        private static readonly string _connectionString = ConfigurationManager
            .ConnectionStrings["MyDbConnection"].ConnectionString;

        public static NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // Execute a query that returns a single value (like SELECT COUNT(*))
        public static object ExecuteScalar(string query, params NpgsqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }

        // Execute a query that returns rows (like SELECT * FROM AppOrder)
        public static NpgsqlDataReader ExecuteReader(string query, params NpgsqlParameter[] parameters)
        {
            var conn = GetConnection();
            var cmd = new NpgsqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        // Execute a non-query (like INSERT, UPDATE, DELETE)
        public static int ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        // Method to create transaction with callback
        public static T ExecuteTransaction<T>(Func<NpgsqlConnection, NpgsqlTransaction, T> action)
        {
            using (var conn = GetConnection())
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    T result = action(conn, transaction);
                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // Helper method to create parameters
        public static NpgsqlParameter CreateParameter(string name, DbType type, object value, byte precision = 0, byte scale = 0)
        {
            var param = new NpgsqlParameter(name, type)
            {
                Value = value ?? DBNull.Value
            };

            // PostgreSQL handles precision/scale differently, but if needed, you can manually cast in SQL

            return param;
        }
    }
}
