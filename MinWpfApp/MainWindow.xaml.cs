
using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
          
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=TSJ;Integrated Security=True";

            core.Interface.IDatabaseTablesProvider tablesProvider = new core.Analyzer.DatabaseTablesProvider();
            core.Interface.IDatabaseAnalyzer analyzer = new core.Analyzer.DatabaseAnalyzer(tablesProvider);

            Dictionary<string, List<Tuple<string, List<string>>>> adjacencyList = analyzer.GetTableAdjacencyList(connectionString);

            Graph graph = new Graph("Database Relationships");
            foreach (var entry in adjacencyList)
            {
                string table = entry.Key;
                List<Tuple<string, List<string>>> relatedTables = entry.Value;

                /*Console.WriteLine("Table: " + table);*/
                foreach (Tuple<string, List<string>> relatedTable in relatedTables)
                {
                    
                    string relatedTableName = relatedTable.Item1;
                    graph.AddEdge(relatedTableName, table);
                    List<string> columns = relatedTable.Item2;

 /*                   Console.WriteLine("Related Table: " + relatedTableName);
                    Console.WriteLine("Columns: " + string.Join(", ", columns));
                    Console.WriteLine();*/
                }

                /*Console.WriteLine();*/
            }
            /*Console.ReadLine();*/
            // Создание пустого графа
            




           

        

            graph.Attr.LayerDirection = LayerDirection.LR;

            graphControl.Graph = graph;
        }
    }
}
