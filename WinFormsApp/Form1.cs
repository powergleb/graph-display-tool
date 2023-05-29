using core.Analyzer;
using core.Interface;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Data;
using System.Security.Cryptography;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using P2 = Microsoft.Msagl.Core.Geometry.Point;
using MouseButtons = System.Windows.Forms.MouseButtons;
using System.Windows.Documents;
using System.Windows;
using Microsoft.Msagl.Layout.Layered;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        Microsoft.Msagl.Drawing.Label labelToChange;
        IViewerObject viewerEntityCorrespondingToLabelToChange;
        readonly System.Windows.Forms.ToolTip toolTip1 = new System.Windows.Forms.ToolTip();
        object selectedObject;
        AttributeBase selectedObjectAttr;
        GViewer viewer = new GViewer();

        public Form1()
        {
#if TEST_MSAGL
            DisplayGeometryGraph.SetShowFunctions();
#endif
            InitializeComponent();
            SuspendLayout();
            this.Controls.Add(viewer);
            viewer.Dock = DockStyle.Fill;
            ResumeLayout();
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.AsyncLayout = true;
            toolTip1.Active = true;
            toolTip1.AutoPopDelay = 2500;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 300;
            InitGraph();
        }
        System.Drawing.Point myMouseDownPoint;
        System.Drawing.Point myMouseUpPoint;


        ICurve GetNodeBoundary(Microsoft.Msagl.Drawing.Node node)
        {
            int widthOffset = node.LabelText.Split('\n').Max(substring => substring.Length) * 17;
            int heightOffset = node.LabelText.Split('\n').Length * 40;


            return CurveFactory.CreateRectangle(widthOffset, heightOffset, new P2());
        }

        bool DrawNode(Node node, object graphics)
        {
            Graphics g = (Graphics)graphics;
            using (System.Drawing.Drawing2D.Matrix m = g.Transform)
            {
                using (System.Drawing.Drawing2D.Matrix saveM = m.Clone())
                {


                    using (var m2 = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, 2 * (float)node.GeometryNode.Center.Y))
                        m.Multiply(m2);

                    g.Transform = m;
                    var x = (int)((int)node.GeometryNode.Center.X - node.GeometryNode.Width / 2);
                    var y = (int)((int)node.GeometryNode.Center.Y - node.GeometryNode.Height / 2);



                    int widthOffset = (node.LabelText.Split('\n').Max(substring => substring.Length) - 10) * 10;
                    var rect = new Rectangle(x, y, (int)node.GeometryNode.Width, (int)node.GeometryNode.Height);
                    var rect1 = new Rectangle(x, y , (int)node.GeometryNode.Width, 30);



                    SolidBrush blueBrush = new SolidBrush(System.Drawing.Color.Blue);
                    SolidBrush biegeBrush = new SolidBrush(System.Drawing.Color.Beige);
                    SolidBrush redBrush = new SolidBrush(System.Drawing.Color.Red);
                    var font = new Font("Arial", 20);


                    g.FillRectangle(biegeBrush, rect);
                    g.FillRectangle(blueBrush, rect1);


                    g.DrawString(node.LabelText.Split('\n')[0], font, biegeBrush, rect1);
                    string str = new string("\n");
                    for (int i = 1; i < node.LabelText.Split('\n').Length; i++)
                    {
                        str += node.LabelText.Split('\n')[i] + "\n";
                        Console.WriteLine(i);
                    }
                    g.DrawString(str, font, blueBrush, rect);

                    g.Transform = saveM;
                    g.ResetClip();
                }
            }

            return true;
        }



        void GViewerObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            selectedObject = e.OldObject != null ? e.OldObject.DrawingObject : null;

            if (selectedObject != null)
            {
                RestoreSelectedObjAttr();
                viewer.Invalidate(e.OldObject);
                selectedObject = null;
            }

            if (viewer.ObjectUnderMouseCursor == null)
            {
                viewer.SetToolTip(toolTip1, "");
            }
            else
            {
                selectedObject = viewer.ObjectUnderMouseCursor.DrawingObject;
                var edge = selectedObject as Edge;
                if (edge != null)
                {
                    selectedObjectAttr = edge.Attr.Clone();
                    edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                    viewer.Invalidate(e.NewObject);
                    viewer.SetToolTip(toolTip1, String.Format("Each {0} have many {1}", edge.Source, edge.Target));
                }
                else if (selectedObject is Node)
                {
                    selectedObjectAttr = (selectedObject as Node).Attr.Clone();
                    (selectedObject as Node).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                    viewer.SetToolTip(toolTip1,
                                       String.Format("Table {0}",
                                                     (selectedObject as Microsoft.Msagl.Drawing.Node).Attr.Id));
                    viewer.Invalidate(e.NewObject);
                }
            }
        }
        void RestoreSelectedObjAttr()
        {
            var edge = selectedObject as Edge;
            if (edge != null && selectedObjectAttr is EdgeAttr atr)
            {
                edge.Attr = atr;
            }
            else
            {
                var node = selectedObject as Microsoft.Msagl.Drawing.Node;
                if (node != null && selectedObjectAttr is NodeAttr attr)
                    node.Attr = attr;
            }
        }

        static internal PointF PointF(P2 p) { return new PointF((float)p.X, (float)p.Y); }

        float radiusRatio = 0.5f;


        private void InitGraph()
        {
            Graph graph = new Graph("Database Relationships");


            //PUT YOUR CONNECTION STRING HERE!
            string connectionString = "Data Source=DESKTOP-6M0QU9E\\SQLEXPRESS;Initial Catalog=TestDB;Integrated Security=True";

            IDatabaseSchemaReader schemaReader = new SqlDatabaseSchemaReader();
            IDatabaseAnalyzer analyzer = new DatabaseAnalyzer(schemaReader);

            List<core.Entity.Table> tables = analyzer.GetTables(connectionString);

            foreach (core.Entity.Table table in tables)
            {
  
                Node node = new Node(table.TableName);
                node.LabelText = table.TableName + "\nPK:" + table.PrimaryKey;
                node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box;
                graph.AddNode(node);

            }

            foreach (core.Entity.Table table in tables)
            {

                if (table.ForeignKeys.Count > 0)
                {

                    foreach (Tuple<string, core.Entity.Table> foreignKey in table.ForeignKeys)
                    {
                        graph.FindNode(foreignKey.Item2.TableName).LabelText += "\nFK: " + foreignKey.Item1;
                        graph.AddEdge(table.TableName,foreignKey.Item2.TableName).Attr.ArrowheadAtTarget = ArrowStyle.ODiamond;
                    }
                }

            }



            foreach (var node in graph.Nodes)
            {

                node.Label.FontColor = Microsoft.Msagl.Drawing.Color.Navy;
                node.Attr.Shape = Shape.DrawFromGeometry;
                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Beige;
                node.DrawNodeDelegate = new DelegateToOverrideNodeRendering(DrawNode);
                node.NodeBoundaryDelegate = new DelegateToSetNodeBoundary(GetNodeBoundary);
            }
            double width = 150;
            double height = 150;

            graph.Attr.LayerSeparation = 500;
            graph.Attr.NodeSeparation = 500;
            double arrowHeadLenght = width / 10;
            foreach (Microsoft.Msagl.Drawing.Edge e in graph.Edges)
                e.Attr.ArrowheadLength = (float)arrowHeadLenght;
            graph.LayoutAlgorithmSettings = new SugiyamaLayoutSettings();
            viewer.Graph = graph;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            viewer.ObjectUnderMouseCursorChanged += GViewerObjectUnderMouseCursorChanged;
        }


    }
}