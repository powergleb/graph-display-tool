using core.Analyzer;
using core.Entity;
using core.Interface;
using System.Data;
using System.Data.SqlClient;


class Program
{
    static void Main()
    {
        

        string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=TSJ;Integrated Security=True";

        IDatabaseSchemaReader schemaReader = new SqlDatabaseSchemaReader();
        IDatabaseAnalyzer analyzer = new DatabaseAnalyzer(schemaReader);

        List<Table> tables = analyzer.GetTables(connectionString);

            foreach (Table table in tables)
            {
                Console.WriteLine("Table: " + table.TableName);
                Console.WriteLine("Primary Key: " + table.PrimaryKey);

                if (table.ForeignKeys.Count > 0)
                {
                    Console.WriteLine("Foreign Keys:");
                    foreach (Tuple<string, Table> foreignKey in table.ForeignKeys)
                    {
                        Console.WriteLine(foreignKey.Item1 + " -> " + foreignKey.Item2.TableName);
                    }
                }

                Console.WriteLine();
            }

            Console.ReadLine();
        
    }
}