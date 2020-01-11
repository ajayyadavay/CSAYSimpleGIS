using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace CSAY_Simple_GIS
{
    public partial class FrmAttributeTable : Form
    {
        public FrmAttributeTable()
        {
            
            InitializeComponent();
           // dataGridView1.DataSource = DTable;
        }

        private void FrmAttributeTable_Load(object sender, EventArgs e)
        {
            LblShapeFileName.Text = FrmSimpleGISMain.ShpNameToDisplayInAttributeTable;

            dataGridView1.DataSource = FrmSimpleGISMain.Dt[FrmSimpleGISMain.LayerHandler_at];
            int colcount = dataGridView1.ColumnCount;
            for(int i = 0; i < colcount; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //dataGridViewMusking.Columns[HeadIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void CopyAlltoClipboard()
        {
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridView1.MultiSelect = true;
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        private void ExportToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            { 
            CopyAlltoClipboard();
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Excel.Application();
            xlexcel.Visible = true;
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            Excel.Range CR = (Excel.Range)xlWorkSheet.Cells[3, 1];
            CR.Select();
            xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            // xlWorkBook.Close();
            //  xlexcel.Quit();
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlWorkSheet);

            MessageBox.Show("Attribute Table Successfully Exported!");

            }
            catch
            {
                

            }
        }
    }
}
