using core.Analyzer;
using core.Interface;
using System.Data;

class Program
{
    static void Main()
    {
        /* string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=ExchangeDB;Integrated Security=True";

         IDatabaseTablesProvider tablesProvider = new DatabaseTablesProvider();
         IDatabaseAnalyzer analyzer = new DatabaseAnalyzer(tablesProvider);

         Dictionary<string, List<Tuple<string, List<string>>>> adjacencyList = analyzer.GetTableAdjacencyList(connectionString);

         foreach (var entry in adjacencyList)
         {
             string table = entry.Key;
             List<Tuple<string, List<string>>> relatedTables = entry.Value;

             Console.WriteLine("Table: " + table);

             foreach (Tuple<string, List<string>> relatedTable in relatedTables)
             {
                 string relatedTableName = relatedTable.Item1;
                 List<string> columns = relatedTable.Item2;

                 Console.WriteLine("Related Table: " + relatedTableName);
                 Console.WriteLine("Columns: " + string.Join(", ", columns));
                 Console.WriteLine();
             }

             Console.WriteLine();
         }
         Console.ReadLine();*/
        string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=TestDB;Integrated Security=True";

        IPrimaryKeyFinder primaryKeyFinder = new PrimaryKeyFinder();
        IRelatedTablesFinder relatedTablesFinder = new RelatedTablesFinder();

        TableRelationAnalyzer analyzer = new TableRelationAnalyzer(connectionString, primaryKeyFinder, relatedTablesFinder);
        DataTable resultTable = analyzer.AnalyzeDatabase();

        // Вывод результатов на консоль
        foreach (DataRow row in resultTable.Rows)
        {
            Console.WriteLine($"Table: {row["TableName"]}");
            Console.WriteLine($"Primary Key: {row["PrimaryKey"]}");
            Console.WriteLine($"Related Tables: {row["RelatedTables"]}");
            Console.WriteLine($"Foreign Key: {row["ForeignKey"]}");
            Console.WriteLine();
        }
    }
    
}