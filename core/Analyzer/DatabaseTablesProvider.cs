using core.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Analyzer
{
    class DatabaseTablesProvider : IDatabaseTablesProvider
    {
        public List<string> GetTables(string connectionString)
        {
            List<string> tables = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string tableName = (string)row[2];
                    tables.Add(tableName);
                }
            }

            return tables;
        }
    }
}
