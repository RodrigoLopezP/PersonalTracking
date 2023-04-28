using DAL;
using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DAL.DTO;

namespace PersonalTracking
{
    public partial class FrmPosition : Form
    {
        List<Department> departmentlist = new List<Department>();
        public PositionDTO posDetail = new PositionDTO();
        public bool isUpdate = false;
        public FrmPosition()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FrmPosition_Load(object sender, EventArgs e)
        {
            departmentlist = DepartmentBLL.GetDepartments();
            cmbDepartment.DataSource = departmentlist;

            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "ID";
            cmbDepartment.SelectedIndex = -1;

            if (isUpdate)
            {
                txtPosition.Text = posDetail.PositionName;
                cmbDepartment.SelectedValue = posDetail.OldDepartmentID;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtPosition.Text.Trim() == "")
                MessageBox.Show("Please fill position name");
            else if(cmbDepartment.SelectedIndex==-1)
                MessageBox.Show("Please select a department");
            else
            {
                if (!isUpdate)
                {
                    Position position = new Position();
                    position.PositionName = txtPosition.Text;
                    position.Department_id = Convert.ToInt32(cmbDepartment.SelectedValue);


                    PositionBBL.AddPosition(position);


                    MessageBox.Show("Position was added");
                    txtPosition.Clear();

                    cmbDepartment.SelectedIndex = -1;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Position positionToUpdate = new Position();
                        positionToUpdate.ID = posDetail.ID;
                        positionToUpdate.PositionName = txtPosition.Text;
                        positionToUpdate.Department_id = Convert.ToInt32(cmbDepartment.SelectedValue);
                        bool control = false;
                        if (positionToUpdate.Department_id!=posDetail.OldDepartmentID)
                        {
                            control = true;
                        }
                        PositionBBL.UpdatePosition(positionToUpdate, control);

                        MessageBox.Show("Position was updated");
                        this.Close();
                    }
                }
            }

        }
    }
}
