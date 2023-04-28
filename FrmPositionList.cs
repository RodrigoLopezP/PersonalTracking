using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BLL;
using DAL;
using DAL.DTO;

namespace PersonalTracking
{
    public partial class FrmPositionList : Form
    {
        List<PositionDTO> positionList = new List<PositionDTO>();
        PositionDTO posDetail = new PositionDTO();
        public FrmPositionList()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmPosition frm = new FrmPosition();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            fillGrid();

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (posDetail.ID == 0)
            {
                MessageBox.Show("Select a position");
            }

            FrmPosition frm = new FrmPosition();
            frm.isUpdate = true;
            frm.posDetail = posDetail;
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            fillGrid();
        }

        private void FrmPositionList_Load(object sender, EventArgs e)
        {
            fillGrid();
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[0].HeaderText = "Department name";
            dataGridView1.Columns[3].HeaderText = "Position name";
        }

        void fillGrid()
        {
            positionList = PositionBBL.GetPositions();
            dataGridView1.DataSource = positionList;

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            posDetail.ID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
            posDetail.PositionName = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            //posDetail.DepartmentName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(); // non serve
            posDetail.OldDepartmentID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[4].Value);
            posDetail.Department_id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[4].Value);

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (posDetail.ID == 0)
            {
                MessageBox.Show("No position selected");
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure about that? Employees with that position are going to be deleted too", "Warning", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    PositionBBL.DeletePosition(posDetail.ID);
                    MessageBox.Show("Position deleted.");
                    fillGrid();
                }
            }
        }
    }
}
