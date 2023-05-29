using core.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Analyzer
{
    public class SqlDatabaseSchemaReader : IDatabaseSchemaReader
    {
        public DataTable GetTablesSchema(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string[] restrictions = new string[4] { null, null, null, "BASE TABLE" };
                DataTable schema = connection.GetSchema("Tables", restrictions);

                return schema;
            }
        }
    }
}
