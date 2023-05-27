using core.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Analyzer
{
    public class PrimaryKeyFinder : IPrimaryKeyFinder
    {
        public string GetPrimaryKey(SqlConnection connection, string tableName)
        {
            string primaryKey = "";

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = '{tableName}'";
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    primaryKey = (string)result;
                }
            }

            return primaryKey;
        }
    }
}
