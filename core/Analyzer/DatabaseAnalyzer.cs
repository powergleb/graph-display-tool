using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using core.Interface;

namespace core.Analyzer
{
    public class DatabaseAnalyzer : IDatabaseAnalyzer
    {
        private readonly IDatabaseSchemaReader _schemaReader;

        public DatabaseAnalyzer(IDatabaseSchemaReader schemaReader)
        {
            _schemaReader = schemaReader;
        }
        public List<Table> GetTables(string connectionString)
        {
            List<Table> tables = new List<Table>();

            DataTable schema = _schemaReader.GetTablesSchema(connectionString);

            foreach (DataRow row in schema.Rows)
            {
                string tableName = (string)row[2];

                Table table = new Table
                {
                    TableName = tableName,
                    PrimaryKey = GetPrimaryKey(connectionString, tableName),
                    ForeignKeys = GetForeignKeys(connectionString, tableName)
                };

                tables.Add(table);
            }

            return tables;
        }
        private string GetPrimaryKey(string connectionString, string tableName)
        {
            string primaryKey = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = $@"SELECT COLUMN_NAME
                              FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                              WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1
                                    AND TABLE_NAME = '{tableName}'";

                SqlCommand command = new SqlCommand(query, connection);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    primaryKey = (string)result;
                }
            }

            return primaryKey;
        }

        private List<Tuple<string, Table>> GetForeignKeys(string connectionString, string tableName)
        {
            List<Tuple<string, Table>> foreignKeys = new List<Tuple<string, Table>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = $@"
    SELECT
        OBJECT_NAME(fkc.parent_object_id) AS TableName,
        COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
        OBJECT_NAME(fkc.referenced_object_id) AS ReferencedTable,
        COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn
    FROM
        sys.foreign_key_columns fkc
    WHERE
        OBJECT_NAME(fkc.referenced_object_id) = '{tableName}'
";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string referencedTable = reader.GetString(0);
                    string foreignKeyName = reader.GetString(1);

                    Table referencedTableEntity = new Table
                    {
                        TableName = referencedTable
                    };

                    foreignKeys.Add(Tuple.Create(foreignKeyName, referencedTableEntity));
                }

                reader.Close();
            }

            return foreignKeys;
        }
    }

    
}