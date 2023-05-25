using Microsoft.Msagl.Core.Geometry.Curves;
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
            //statusTextBox.Text = "there was a click...";
            var node = graphViewer.ObjectUnderMouseCursor as IViewerNode;
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
            }
        }

        void WpfApplicationSample_MouseDown(object sender, MsaglMouseEventArgs e)
        {
            //statusTextBox.Text = "there was a click...";
            var node = graphViewer.ObjectUnderMouseCursor as IViewerNode;
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
            }
        }

        void SetStatusBar()
        {
            var statusBar = new StatusBar();
            statusTextBox = new TextBox { Text = "No object", Background = Brushes.Red };
            statusBar.Items.Add(statusTextBox);
            mainGrid.Children.Add(statusBar);
            statusBar.VerticalAlignment = VerticalAlignment.Bottom;


            popup = new Popup();
            textBlock = new TextBlock
            {
                Text = "",
                Background = Brushes.Yellow,
                Foreground = Brushes.Black,
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
                string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=ExchangeDB;Integrated Security=True";

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

                }


/*                graph.Attr.LayerDirection = LayerDirection.LR;

                graphControl.Graph = graph;*/

                graphViewer.Graph = graph;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Load Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
