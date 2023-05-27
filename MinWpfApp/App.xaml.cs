
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Documents;


using System.Xml;
using Microsoft.Msagl.Core.Geometry.Curves;
using System.Drawing;
using Microsoft.Msagl.Core.Layout;
using Node = Microsoft.Msagl.Drawing.Node;
using Edge = Microsoft.Msagl.Drawing.Edge;
using System.Xml.Linq;
using LineSegment = Microsoft.Msagl.Core.Geometry.Curves.LineSegment;
using P2 = Microsoft.Msagl.Core.Geometry.Point;
using Brushes = System.Drawing.Brushes;
using System.Windows.Media.Media3D;
using core.Analyzer;
using core.Interface;

namespace MinWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly RoutedUICommand LoadSampleGraphCommand = new RoutedUICommand("Open File...", "OpenFileCommand",
                                                                             typeof(App));
        public static readonly RoutedUICommand HomeViewCommand = new RoutedUICommand("Home view...", "HomeViewCommand",
                                                                                     typeof(App));
 

        Window appWindow;
        Grid mainGrid = new Grid();
        DockPanel graphViewerPanel = new DockPanel();
        ToolBar toolBar = new ToolBar();
        GraphViewer graphViewer = new GraphViewer();
        TextBox statusTextBox;
        TextBlock textBlock;
        Popup popup;
        //public Graph graph = new Graph("Database Relationships");

        protected override void OnStartup(StartupEventArgs e)
        {
            appWindow = new Window
            {
                Title = "WpfApplicationSample",
                Content = mainGrid,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState = WindowState.Normal
            };

            SetupToolbar();
            graphViewerPanel.ClipToBounds = true;
            mainGrid.Children.Add(toolBar);
            toolBar.VerticalAlignment = VerticalAlignment.Top;
            graphViewer.ObjectUnderMouseCursorChanged += graphViewer_ObjectUnderMouseCursorChanged;
            mainGrid.Children.Add(graphViewerPanel);
            graphViewer.BindToPanel(graphViewerPanel);
            graphViewer.MouseMove += GraphViewer_MouseMove;

            SetStatusBar();
            graphViewer.MouseDown += WpfApplicationSample_MouseDown;
            appWindow.Loaded += (a, b) => CreateAndLayoutAndDisplayGraph(null, null);


            //CreateAndLayoutAndDisplayGraph(null,null);
            //graphViewer.MainPanel.MouseLeftButtonUp += TestApi;

            // Определение пользовательского шаблона узла


            // Применение ресурсов


            appWindow.Show();
  

            
           
        }

        private void GraphViewer_MouseMove(object? sender, MsaglMouseEventArgs e)
        {
            var node = graphViewer.ObjectUnderMouseCursor as IViewerNode;
            if (node != null)
            {
/*                popup.IsOpen = true;
                popup.HorizontalOffset = e.X;
                popup.VerticalOffset = e.Y;
                var drawingNode = (Node)node.DrawingObject;
                textBlock.Text = "ыапыапыап";*/
                popup.IsOpen = false;
            }
            else
            {
                var edge = graphViewer.ObjectUnderMouseCursor as IViewerEdge;
                if (edge != null)
                {
                    string[] parts = ((Edge)edge.DrawingObject).SourceNode.Label.Text.Split('\n');
                    string relatedTableName;
                    string table;
                    relatedTableName = parts[0];
                    parts = ((Edge)edge.DrawingObject).TargetNode.Label.Text.Split('\n');
                    table = parts[0];
                    popup.IsOpen = true;
                    popup.HorizontalOffset = e.X;
                    popup.VerticalOffset = e.Y;
                    textBlock.Text = "Each " + table + " have many " + relatedTableName;
                }
                else
                    popup.IsOpen = false;
            }
        }

        void WpfApplicationSample_MouseDown(object sender, MsaglMouseEventArgs e)
        {
            statusTextBox.Text = "there was a click...";
            /*var node = graphViewer.ObjectUnderMouseCursor as IViewerNode;
            if (node != null)
            {
                popup.IsOpen = true;
                popup.HorizontalOffset = e.X;
                popup.VerticalOffset = e.Y;
                var drawingNode = (Node)node.DrawingObject;
                textBlock.Text = "ыапыапыап";
            }
            else
            {
                var edge = graphViewer.ObjectUnderMouseCursor as IViewerEdge;
                if (edge != null)
                {
                    string relatedTableName = ((Edge)edge.DrawingObject).SourceNode.Label.Text;
                    string table = ((Edge)edge.DrawingObject).TargetNode.Label.Text;
                    popup.IsOpen = true;
                    popup.HorizontalOffset = e.X;
                    popup.VerticalOffset = e.Y;
                    textBlock.Text = "Each " + table + " have many " + relatedTableName;
                }
                else
                    popup.IsOpen = false;
            }*/
        }

        void SetStatusBar()
        {
            var statusBar = new StatusBar();
            statusTextBox = new TextBox { Text = "No object", Background = System.Windows.Media.Brushes.Red };
            statusBar.Items.Add(statusTextBox);
            mainGrid.Children.Add(statusBar);
            statusBar.VerticalAlignment = VerticalAlignment.Bottom;


            popup = new Popup();
            textBlock = new TextBlock
            {
                Text = "",
                Background = System.Windows.Media.Brushes.Yellow,
                Foreground = System.Windows.Media.Brushes.Black,
                Padding = new Thickness(10),
                FontSize = 16
            };

            popup.Child = textBlock;
            popup.IsOpen = false;
            popup.PlacementTarget = appWindow;
            popup.Placement = PlacementMode.Relative;
            popup.VerticalOffset =0;
            popup.HorizontalOffset =0;
        }

        void graphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            var node = graphViewer.ObjectUnderMouseCursor as IViewerNode;
            if (node != null)
            {
                var drawingNode = (Node)node.DrawingObject;
                statusTextBox.Text = drawingNode.Label.Text;
            }
            else
            {
                var edge = graphViewer.ObjectUnderMouseCursor as IViewerEdge;
                if (edge != null)
                {
                    statusTextBox.Text = ((Edge)edge.DrawingObject).SourceNode.Label.Text + "->" +
                     ((Edge)edge.DrawingObject).TargetNode.Label.Text;
                  
                }
                else
                {
                    statusTextBox.Text = "No object";
                }
            }

        }




        void SetupToolbar()
        {
            SetupCommands();
            DockPanel.SetDock(toolBar, Dock.Top);
            SetMainMenu();
            //edgeRangeSlider = CreateRangeSlider();
            // toolBar.Items.Add(edgeRangeSlider.Visual);
        }


        void SetupCommands()
        {
            appWindow.CommandBindings.Add(new CommandBinding(LoadSampleGraphCommand, CreateAndLayoutAndDisplayGraph));
            appWindow.CommandBindings.Add(new CommandBinding(HomeViewCommand, (a, b) => graphViewer.SetInitialTransform()));
            appWindow.InputBindings.Add(new InputBinding(LoadSampleGraphCommand, new KeyGesture(Key.L, System.Windows.Input.ModifierKeys.Control)));
            appWindow.InputBindings.Add(new InputBinding(HomeViewCommand, new KeyGesture(Key.H, System.Windows.Input.ModifierKeys.Control)));

        }


        void SetMainMenu()
        {
            var mainMenu = new Menu { IsMainMenu = true };
            toolBar.Items.Add(mainMenu);
            SetFileMenu(mainMenu);
            SetViewMenu(mainMenu);
        }

        void SetViewMenu(Menu mainMenu)
        {
            var viewMenu = new MenuItem { Header = "_View" };
            var viewMenuItem = new MenuItem { Header = "_Home", Command = HomeViewCommand };
            viewMenu.Items.Add(viewMenuItem);
            mainMenu.Items.Add(viewMenu);
        }

        void SetFileMenu(Menu mainMenu)
        {
            var fileMenu = new MenuItem { Header = "_File" };
            var openFileMenuItem = new MenuItem { Header = "_Load Sample Graph", Command = LoadSampleGraphCommand };
            fileMenu.Items.Add(openFileMenuItem);
            mainMenu.Items.Add(fileMenu);
        }
        void CreateAndLayoutAndDisplayGraph(object sender, ExecutedRoutedEventArgs ex)
        {
            try
            {
                /*                string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=TestDB;Integrated Security=True";

                                core.Interface.IDatabaseTablesProvider tablesProvider = new core.Analyzer.DatabaseTablesProvider();
                                core.Interface.IDatabaseAnalyzer analyzer = new core.Analyzer.DatabaseAnalyzer(tablesProvider);
                                Dictionary<string, List<Tuple<string, List<string>>>> adjacencyList = analyzer.GetTableAdjacencyList(connectionString);

                                Graph graph = new Graph("Database Relationships");

                                foreach (var entry in adjacencyList)
                                {
                                    string table = entry.Key;
                                    List<Tuple<string, List<string>>> relatedTables = entry.Value;

                                    foreach (Tuple<string, List<string>> relatedTable in relatedTables)
                                    {

                                        string relatedTableName = relatedTable.Item1;
                                        graph.AddEdge(relatedTableName, table);
                                        List<string> columns = relatedTable.Item2;


                                    }

                                }*/
                Graph graph = new Graph("Database Relationships");
                string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=TestDB;Integrated Security=True";

                IPrimaryKeyFinder primaryKeyFinder = new PrimaryKeyFinder();
                IRelatedTablesFinder relatedTablesFinder = new RelatedTablesFinder();

                TableRelationAnalyzer analyzer = new TableRelationAnalyzer(connectionString, primaryKeyFinder, relatedTablesFinder);
                DataTable resultTable = analyzer.AnalyzeDatabase();

                // Вывод результатов на консоль
                foreach (DataRow row in resultTable.Rows)
                {
                    string tmp = row["PrimaryKey"].ToString();
                    Node node = new Node(row["TableName"].ToString());
                    node.LabelText = row["TableName"].ToString() + "\nPK:" + row["PrimaryKey"].ToString();
                    node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.House;
                    graph.AddNode(node);

                }
                foreach (DataRow row in resultTable.Rows)
                {
                    //graph.FindNode(row["RelatedTables"].ToString()).LabelText += "\nSize: " + row["ForeignKey"].ToString();
                    graph.AddEdge(row["TableName"].ToString(), row["RelatedTables"].ToString());

                }
                foreach (DataRow row in resultTable.Rows)
                {
                    graph.FindNode(row["RelatedTables"].ToString()).LabelText = row["TableName"].ToString() + "\nPK:" + row["PrimaryKey"].ToString() +"\nFK:" + row["ForeignKey"].ToString();

                }

                foreach (var node in graph.Nodes)
                {
                        node.Attr.Shape = Shape.Box;
                        node.DrawNodeDelegate = new DelegateToOverrideNodeRendering(DrawNode);
                        node.NodeBoundaryDelegate = new DelegateToSetNodeBoundary(GetNodeBoundary);
                }
               /* var node1 = graph.Nodes.First();
                Microsoft.Msagl.Drawing.Node visnode = new Microsoft.Msagl.Drawing.Node($"{node1.Id}: {node1.LabelText}") { UserData = "wrtwrt" };
                visnode.NodeBoundaryDelegate = GetNodeBoundary;
                visnode.DrawNodeDelegate = DrawNode;
                graph.AddNode(visnode);*/

                /*                graph.Attr.LayerDirection = LayerDirection.LR;

                                graphControl.Graph = graph;*/
                graphViewer.Graph = graph;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Load Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        ICurve GetNodeBoundary(Microsoft.Msagl.Drawing.Node node)
        {
            var x = (float)node.GeometryNode.Center.X;
            var y = (float)node.GeometryNode.Center.Y;
            return CurveFactory.CreateTestShape(500, 50);
        }



        private  bool DrawNode(Microsoft.Msagl.Drawing.Node node, object graphics)
        {
            Graphics g = (Graphics)graphics;
            Node n = (Node)node.UserData;
            var x = (float)node.GeometryNode.Center.X;
            var y = (float)node.GeometryNode.Center.Y;
            PointF p = new PointF(x,y);
            g.DrawString("1341341", new Font("Arial", 16), Brushes.Black, p);
            return true;
        }


        private Microsoft.Msagl.Drawing.Color generateColor(Microsoft.Msagl.Drawing.Color colorNode)
        {
            Random rnd = new Random();
            int beg = 100;
            int end = 255;
            colorNode.R = byte.Parse(rnd.Next(beg, end) + "");
            colorNode.G = byte.Parse(rnd.Next(beg, end) + "");
            colorNode.B = byte.Parse(rnd.Next(beg, end) + "");
            return colorNode;
        }

    }
}
