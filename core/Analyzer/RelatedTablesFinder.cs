using core.Entity;
using core.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Analyzer
{
    public class RelatedTablesFinder : IRelatedTablesFinder
    {
        public List<RelatedTable> GetRelatedTables(SqlConnection connection, string tableName)
        {
            List<RelatedTable> relatedTables = new List<RelatedTable>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT OBJECT_NAME(f.parent_object_id) AS TableName, c.name AS ForeignKey FROM sys.foreign_keys AS f INNER JOIN sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id INNER JOIN sys.columns AS c ON fc.parent_column_id = c.column_id AND fc.parent_object_id = c.object_id WHERE OBJECT_NAME(f.referenced_object_id) = '{tableName}'";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string relatedTable = reader["TableName"].ToString();
                    string foreignKey = reader["ForeignKey"].ToString();
                    relatedTables.Add(new RelatedTable(relatedTable, foreignKey));
                }

                reader.Close();
            }

            return relatedTables;
        }
    }
}
