using core.Entity;
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
    public class TableRelationAnalyzer
    {
        private string connectionString;
        private IPrimaryKeyFinder primaryKeyFinder;
        private IRelatedTablesFinder relatedTablesFinder;

        public TableRelationAnalyzer(string connectionString, IPrimaryKeyFinder primaryKeyFinder, IRelatedTablesFinder relatedTablesFinder)
        {
            this.connectionString = connectionString;
            this.primaryKeyFinder = primaryKeyFinder;
            this.relatedTablesFinder = relatedTablesFinder;
        }

        public DataTable AnalyzeDatabase()
        {
            DataTable schemaTable = new DataTable("TableRelations");
            schemaTable.Columns.Add("TableName", typeof(string));
            schemaTable.Columns.Add("PrimaryKey", typeof(string));
            schemaTable.Columns.Add("RelatedTables", typeof(string));
            schemaTable.Columns.Add("ForeignKey", typeof(string));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable tables = connection.GetSchema("Tables");

                foreach (DataRow tableRow in tables.Rows)
                {
                    string tableName = (string)tableRow["TABLE_NAME"];
                    string primaryKey = primaryKeyFinder.GetPrimaryKey(connection, tableName);
                    List<RelatedTable> relatedTables = relatedTablesFinder.GetRelatedTables(connection, tableName);

                    foreach (RelatedTable relatedTable in relatedTables)
                    {
                        schemaTable.Rows.Add(tableName, primaryKey, relatedTable.TableName, relatedTable.ForeignKey);
                    }
                }
            }

            return schemaTable;
        }
    }

}
