using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASCOM.GeminiTelescope
{
    public partial class frmJPLMultiple : Form
    {
        public List<List<string>> choices { get; set; } = new List<List<string>>();
        public string recordNumber = "";
        public frmJPLMultiple()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if(grid.SelectedRows.Count > 0)
            {
                recordNumber = grid.SelectedRows[0].Cells[0].Value as string;
                DialogResult = DialogResult.OK;
            }
        }

        private void frmTLE_Load(object sender, EventArgs e)
        {
            UpdateTable();
        }

        bool bUpdate = false;

        private void UpdateTable()
        {
            bUpdate = false;
            grid.Rows.Clear();
            grid.Columns.Clear();

            var c1 = grid.Columns.Add("Record #", "Record #");
            grid.Columns[c1].ReadOnly = true;


            var c = grid.Columns.Add("Epoch Year", "Epoch Year");

            grid.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        
            var p = grid.Columns.Add("Object Name", "Object Name");
            grid.Columns[p].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            for (int i = 0; i < choices.Count; ++i)
            {
                var row = grid.Rows.Add();
                grid.Rows[row].Cells[0].Value = choices[i][0];

                grid.Rows[row].Cells[1].Value = choices[i][1];
                grid.Rows[row].Cells[2].Value = choices[i][3] + "/" + choices[i][4];
            }

            bUpdate = true;
        }

        private void grid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0)
            {
                recordNumber = grid.Rows[e.RowIndex].Cells[0].Value as string;
                DialogResult = DialogResult.OK;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
