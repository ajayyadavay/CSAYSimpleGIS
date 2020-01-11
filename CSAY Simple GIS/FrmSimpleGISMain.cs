using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using AxMapWinGIS;
using MapWinGIS;



namespace CSAY_Simple_GIS
{
    public partial class FrmSimpleGISMain : Form
    {
        //int LayerNumbers = 0;
        string[] AllLoadedFileName = new string[500];
        public static String ShpNameToDisplayInAttributeTable;
        //public static DataTable Dt = new DataTable();
        public static DataTable[] Dt = new DataTable[100];
        public static int LayerHandler_at;

        public FrmSimpleGISMain()
        {
            InitializeComponent();
        }

        private void CursorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMap1.CursorMode = MapWinGIS.tkCursorMode.cmNone;
        }

        private void PanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMap1.CursorMode = MapWinGIS.tkCursorMode.cmPan;
        }

        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMap1.CursorMode = MapWinGIS.tkCursorMode.cmZoomIn;
        }

        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMap1.CursorMode = MapWinGIS.tkCursorMode.cmZoomOut;
        }

        private void ZoomToExtentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMap1.ZoomToMaxExtents();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmSimpleGISMain_Load(object sender, EventArgs e)
        {
            //Level 1
            treeView1.Nodes.Add("Layers","Layers");      //0   

            //Level 2
            //treeView1.Nodes[0].Nodes.Add("Series");         //0 0
            //treeView1.Nodes[0].Nodes.Add("Stand alone");    //0 1
        }

        private void VisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int indx = treeView1.SelectedNode.Index;
            int LayerHandler = axMap1.get_LayerHandle(indx);

            if(axMap1.get_LayerVisible(LayerHandler))
            {
                axMap1.set_LayerVisible(LayerHandler, false); //make layer invisible
                treeView1.SelectedNode.ForeColor = Color.Red;
                //treeView1.SelectedNode.NodeFont = new Font("Arial", 5);
            }
            else
            {
                axMap1.set_LayerVisible(LayerHandler, true); //make layer visible
                treeView1.SelectedNode.ForeColor = Color.Black;
            }
        }

        private void RemoveSelectedLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //RemoveLayerIndx
            //MessageBox.Show("RemoveHandle: " + RemoveLayerIndx.ToString());

            int indx = treeView1.SelectedNode.Index;
            int RemoveLayerIndx = axMap1.get_LayerHandle(indx);

            //MessageBox.Show(indx.ToString() +" : " +treeView1.SelectedNode.Text + " : RemoveHandle: " + RemoveLayerIndx.ToString());

            axMap1.RemoveLayer(RemoveLayerIndx);
            treeView1.SelectedNode.Remove();
           
            //TxtLayerNumber.Text = LayerNumbers.ToString();
        }

        private void SelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMap1.CursorMode = MapWinGIS.tkCursorMode.cmSelection;
            axMap1.SendSelectBoxFinal = true;
        }

        private void RemoveAllLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMap1.RemoveAllLayers();
            treeView1.Nodes[0].Nodes.Clear();
        }

        private void TreeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // DoDragDrop(e.Item, DragDropEffects.Move);

            // Move the dragged node when the left mouse button is used. 
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        
        private void TreeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TreeView1_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            System.Drawing.Point targetPoint = treeView1.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Retrieve the node that was dragged.
            TreeNode draggedNode =(TreeNode)e.Data.GetData(typeof(TreeNode));

            // Sanity check
            if (draggedNode == null)
            {
                return;
            }

            // Retrieve the node at the drop location.
            TreeNode targetNode = treeView1.GetNodeAt(targetPoint);

            // Did the user drop the node 
            if (targetNode == null)
            {
                //draggedNode.Remove();
                //treeView1.Nodes[0].Nodes.Add(draggedNode);
                //treeView1.Nodes.Add(draggedNode);
                //draggedNode.Expand();
            }
            else
            {
                TreeNode parentNode = targetNode;

                // Confirm that the node at the drop location is not 
                // the dragged node and that target node isn't null
                // (for example if you drag outside the control)
                if (!draggedNode.Equals(targetNode) && targetNode != null)
                {
                    bool canDrop = true;
                    /*while (canDrop && (parentNode != null))
                    {
                        canDrop = !Object.ReferenceEquals(draggedNode, parentNode);
                        parentNode = parentNode.Parent;
                    }*/

                    if (canDrop)
                    {
                        // Is the targets parent node null?
                        if (targetNode.Parent == null)
                        {
                            // The target node has no parent. That means 
                            // the target node is at the root level. We'll 
                            // insert the node at the root level below the 
                            // target node.
                            //treeView1.Nodes.Insert(targetNode.Index + 1, draggedNode);
                        }
                        else
                        {
                            // The target node has a valid parent so we'll 
                            // drop the node into it's index.
                            draggedNode.Remove();
                            targetNode.Parent.Nodes.Insert(targetNode.Index + 1, draggedNode);
                        }
                    }
                }
            }
        }

        private void ZoomToLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int indx = treeView1.SelectedNode.Index;
            int LayerHandler = axMap1.get_LayerHandle(indx);

            axMap1.ZoomToLayer(LayerHandler);
        }

        private void ToolStripZoomIn_Click(object sender, EventArgs e)
        {
            ZoomInToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            ZoomInToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            ZoomOutToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            ZoomToExtentToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButton4_Click(object sender, EventArgs e)
        {
            PanToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            CursorToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButton6_Click(object sender, EventArgs e)
        {
            AddVectorLayerShpToolStripMenuItem1_Click(sender, e);
        }

        private void ToolStripButton7_Click(object sender, EventArgs e)
        {
            AddRasterLayerToolStripMenuItem1_Click(sender, e);
        }

        private void AddVectorLayerShpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string FilePath, Filename;
            FilePath = @"F:\AY\SCREEN_CAPTURE\ARCPY\Catchment\cat_poly\cat_poly.shp";

            OpenFileDialog openfiledialog1 = new OpenFileDialog();
            openfiledialog1.Filter = "Shapefile(*.shp)|*.shp|All Files(*.*)|*.*";
            openfiledialog1.FilterIndex = 1;

            if (openfiledialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = openfiledialog1.FileName;
            }
            //else if (openfiledialog1.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            //{
            //return;
            //}

            //FilePath = @"F:\AY\SCREEN_CAPTURE\ARCPY\Catchment\cat_poly\cat_poly.shp";

            int layerHandle = -1;
            if (FilePath.ToLower().EndsWith(".shp"))
            {
                Shapefile sf = new Shapefile();
                if (sf.Open(FilePath, null))
                {
                    layerHandle = axMap1.AddLayer(sf, true);
                    //AllLoadedFileName[layerHandle] = FilePath.ToString();

                    //HandleValue[LayerNumbers] = axMap1.AddLayer(sf, true);
                    //AllLoadedFileName[LayerNumbers] = FilePath.ToString();

                    //Level 2
                    //treeView1.Nodes[0].Nodes.Add(FilePath.ToString(), FilePath.ToString());         //0 0
                    Filename = Path.GetFileName(FilePath);
                    treeView1.Nodes[0].Nodes.Add(Filename);         //0 0
                    //LayerNumbers++;
                }
                else
                    MessageBox.Show(sf.ErrorMsg[sf.LastErrorCode]);
            }
            //TxtLayerNumber.Text = LayerNumbers.ToString();
        }

        private void AddRasterLayerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string FilePath, Filename;
            FilePath = @"F:\AY\SCREEN_CAPTURE\ARCPY\Catchment\cat_poly\cat_poly.shp";

            OpenFileDialog openfiledialog1 = new OpenFileDialog();
            openfiledialog1.Filter = "Raster tif(*.tif)|*.tif|Raster png(*.png)|*.png|All Files(*.*)|*.*";
            openfiledialog1.FilterIndex = 1;

            if (openfiledialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = openfiledialog1.FileName;
            }
            else if (openfiledialog1.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                //return;
            }

            //FilePath = @"F:\AY\SCREEN_CAPTURE\ARCPY\Catchment\cat_poly\cat_poly.shp";

            int layerHandle = -1;
            if (FilePath.ToLower().EndsWith(".tif") ||
                FilePath.ToLower().EndsWith(".png"))
            {
                MapWinGIS.Image img = new MapWinGIS.Image();
                if (img.Open(FilePath, ImageType.TIFF_FILE, false, null))
                {
                    layerHandle = axMap1.AddLayer(img, true);
                    //HandleValue[LayerNumbers] = axMap1.AddLayer(img, true);
                    //AllLoadedFileName[LayerNumbers] = FilePath.ToString();
                    AllLoadedFileName[layerHandle] = FilePath.ToString();

                    //Level 2
                    //treeView1.Nodes[0].Nodes.Add(FilePath.ToString(), FilePath.ToString());         //0 0

                    Filename = Path.GetFileName(FilePath);
                    treeView1.Nodes[0].Nodes.Add(Filename);
                    ///LayerNumbers++;
                }
                else
                    MessageBox.Show(img.ErrorMsg[img.LastErrorCode]);
            }
            //TxtLayerNumber.Text = LayerNumbers.ToString();
        }

        private void ToolStripButton8_Click(object sender, EventArgs e)
        {
            RemoveAllLayersToolStripMenuItem_Click(sender, e);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout fabout = new FrmAbout();
            fabout.Show();
        }

        private void ShowAttributeTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Application.DoEvents();
            int indx = treeView1.SelectedNode.Index;
            int LayerHandler = axMap1.get_LayerHandle(indx);

            ShowingAttributeTable();
            FrmAttributeTable fattrTable = new FrmAttributeTable();

            /*FrmAttributeTable[] fat = new FrmAttributeTable[20];

            fat[LayerHandler] = new FrmAttributeTable();
            fat[LayerHandler].Show();*/
            
            fattrTable.Show();
            //fattrTable.ShowDialog();
        }

        public void ShowingAttributeTable()
        {
            int indx = treeView1.SelectedNode.Index;
            //int LayerHandler_at = axMap1.get_LayerHandle(indx);
            LayerHandler_at = axMap1.get_LayerHandle(indx);

            //creating object i.e. instance of class 
            Dt[LayerHandler_at] = new DataTable();

            Shapefile sf = axMap1.get_Shapefile(LayerHandler_at);

            string tempFile = sf.Filename;
            ShpNameToDisplayInAttributeTable = "Attribute Table of " + Path.GetFileName(tempFile);
            //MessageBox.Show(ShpNameToDisplayInAttributeTable);

            //clearing rows and columns
            /*Dt.Rows.Clear();
            Dt.Columns.Clear(); */

            Dt[LayerHandler_at].Rows.Clear();
            Dt[LayerHandler_at].Columns.Clear();

            //Adding columns of attribute table of shapefile to Datatable
            for (int i = 0; i < sf.NumFields; i++) //sf.NumFields gives number of column in the attribute table of shapefile
            {
                //Dt.Columns.Add(sf.Field[i].Name);
                Dt[LayerHandler_at].Columns.Add(sf.Field[i].Name);
            }

            //Adding rows of attribute table of shapefile to Datatable
            DataRow DtRow;
            for(int j = 0; j < sf.NumShapes; j++)
            {
                //DtRow = Dt.NewRow();
                DtRow = Dt[LayerHandler_at].NewRow();

                for (int k = 0; k < sf.NumFields; k++)
                {
                    DtRow[k] = sf.CellValue[k, j];
                }

                //Dt.Rows.Add(DtRow);
                Dt[LayerHandler_at].Rows.Add(DtRow);
            }
        }

        private void ShowLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int Attr_field;
                string LabelText;
                double x, y;

                int indx = treeView1.SelectedNode.Index;
                int LayerHandlerlbl = axMap1.get_LayerHandle(indx);

                //Shapefile[] sf1 = new Shapefile[500];
                //sf1[LayerHandlerlbl] = new Shapefile();

                Shapefile sf = axMap1.get_Shapefile(LayerHandlerlbl);
                //sf1[LayerHandlerlbl] = axMap1.get_Shapefile(LayerHandlerlbl);

                sf.Labels.Visible = true;
                //sf1[LayerHandlerlbl].Labels.Visible = true;
                sf.Labels.Clear();

                Attr_field = 1; //Field (or column index in attribute table of the shapefile)
                for (int i = 0; i < sf.NumShapes; i++)
                //for (int i = 0; i < sf1[LayerHandlerlbl].NumShapes; i++)
                {
                    LabelText = sf.CellValue[Attr_field, i].ToString();
                    
                    //LabelText = sf1[LayerHandlerlbl].CellValue[Attr_field, i].ToString();

                    if(sf.ShapefileType == ShpfileType.SHP_POLYGON)
                    {
                        MapWinGIS.Point pnt = sf.Shape[i].Centroid;
                        sf.Labels.AddLabel(LabelText, pnt.x, pnt.y);
                    }
                    else
                    {
                        x = (sf.Shape[i].Extents.xMin + sf.Shape[i].Extents.xMax)/2;
                        y = (sf.Shape[i].Extents.yMin + sf.Shape[i].Extents.yMax)/2;

                        sf.Labels.AddLabel(LabelText, x, y);
                    }

                    //MapWinGIS.Point pnt = sf1[LayerHandlerlbl].Shape[i].Centroid;

                    //sf1[LayerHandlerlbl].Labels.AddLabel(LabelText, pnt.x, pnt.y);
                    //MessageBox.Show(LabelText);
                }
                
                sf.Labels.Synchronized = true;
                //sf1[LayerHandlerlbl].Labels.Synchronized = true;
                axMap1.Redraw();
                //MessageBox.Show("Labeled!!!!");
            }
            catch
            {

            }
            
        }

        private void HideLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int indx = treeView1.SelectedNode.Index;
                int LayerHandlerlbl = axMap1.get_LayerHandle(indx);

                Shapefile sf = axMap1.get_Shapefile(LayerHandlerlbl);

                sf.Labels.Visible = false;
                axMap1.Redraw();
            }
            catch
            {

            }
            
        }
    }
}
