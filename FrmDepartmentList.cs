using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BLL;
using DAL;

namespace PersonalTracking
{
    public partial class FrmDepartmentList : Form
    {
        List<Department> list = new List<Department>();
        Department depDetail = new Department();
        public FrmDepartmentList()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmDepartment frm = new FrmDepartment();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            list = DepartmentBLL.GetDepartments();
            dataGridView1.DataSource = list;
            dataGridView1.Columns[0].HeaderText = "Department ID";
            dataGridView1.Columns[1].HeaderText = "Department name";

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (depDetail.ID == 0)
            {
                MessageBox.Show("Select a department");
            }
            else
            {
                FrmDepartment frm = new FrmDepartment();
                frm.isUpdate = true;
                frm.depDetail = depDetail;
                this.Hide();
                frm.ShowDialog();
                FillAllData();
                this.Visible = true;
            }
        }

        private void FrmDepartmentList_Load(object sender, EventArgs e)
        {
            FillAllData();
            dataGridView1.Columns[0].HeaderText = "Department ID";
            dataGridView1.Columns[1].HeaderText = "Department name";
            dataGridView1.Columns[0].Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            depDetail.ID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            depDetail.DepartmentName= dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        void FillAllData()
        {
            list = DepartmentBLL.GetDepartments();
            dataGridView1.DataSource = list;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (depDetail.ID == 0)
            {
                MessageBox.Show("No department selected");
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    DepartmentBLL.DeleteDepartment(depDetail.ID);
                    MessageBox.Show("Department deleted");
                    FillAllData();
                }
            }
        }
    }
}
