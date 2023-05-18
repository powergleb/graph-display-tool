using core.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Analyzer
{
    public class DatabaseAnalyzer : IDatabaseAnalyzer
    {
        private readonly IDatabaseTablesProvider tablesProvider;

        public DatabaseAnalyzer(IDatabaseTablesProvider tablesProvider)
        {
            this.tablesProvider = tablesProvider;
        }

        public Dictionary<string, List<Tuple<string, List<string>>>> GetTableAdjacencyList(string connectionString)
        {
            List<string> tables = tablesProvider.GetTables(connectionString);

            Dictionary<string, List<Tuple<string, List<string>>>> adjacencyList = new Dictionary<string, List<Tuple<string, List<string>>>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (string table in tables)
                {
                    string query = $@"SELECT OBJECT_NAME(fk.parent_object_id) AS RelatedTable,
                                         c.name AS ColumnName
                                  FROM sys.foreign_key_columns fk
                                  INNER JOIN sys.columns c ON fk.parent_object_id = c.object_id AND fk.parent_column_id = c.column_id
                                  INNER JOIN sys.tables t ON fk.referenced_object_id = t.object_id
                                  WHERE t.name = '{table}'";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string relatedTable = reader.GetString(0);
                        string column = reader.GetString(1);

                        if (adjacencyList.ContainsKey(table))
                        {
                            bool alreadyExists = false;
                            foreach (Tuple<string, List<string>> existingEntry in adjacencyList[table])
                            {
                                if (existingEntry.Item1 == relatedTable)
                                {
                                    existingEntry.Item2.Add(column);
                                    alreadyExists = true;
                                    break;
                                }
                            }

                            if (!alreadyExists)
                            {
                                adjacencyList[table].Add(Tuple.Create(relatedTable, new List<string> { column }));
                            }
                        }
                        else
                        {
                            adjacencyList[table] = new List<Tuple<string, List<string>>> { Tuple.Create(relatedTable, new List<string> { column }) };
                        }
                    }

                    reader.Close();
                }
            }

            return adjacencyList;
        }
    }
}
