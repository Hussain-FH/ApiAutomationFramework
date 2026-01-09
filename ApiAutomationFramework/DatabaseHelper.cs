
using System;
using System.Data;
using System.Data.SqlClient;

namespace ApiAutomationFramework
{
    public static class DatabaseHelper
    {
        private static string _connectionString = "Server=YOUR_SERVER;Database=YOUR_DB;User Id=USERNAME;Password=PASSWORD;";

        public static DataTable ExecuteQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
