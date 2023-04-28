using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BLL;
using DAL;
namespace PersonalTracking
{
    public partial class FrmDepartment : Form
    {
        public bool isUpdate=false;
        public Department depDetail = new Department();
        public FrmDepartment()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isUpdate)
            {
                Department department = new Department();
                department.DepartmentName = txtDepartment.Text;
                BLL.DepartmentBLL.AddDepartment(department);
                MessageBox.Show("Department was added");
                txtDepartment.Clear();
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Department depToUpdate = new Department();
                    depToUpdate.ID = depDetail.ID;
                    depToUpdate.DepartmentName = txtDepartment.Text;

                    DepartmentBLL.UpdateDepartment(depToUpdate);

                    MessageBox.Show("Department was updated");
                    this.Close();
                }
            }

        }

        private void FrmDepartment_Load(object sender, EventArgs e)
        {
            if (isUpdate)
            {
                txtDepartment.Text = depDetail.DepartmentName.ToString();
            }
        }
    }
}
