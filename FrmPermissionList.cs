using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DAL;
using DAL.DTO;
using BLL;
using System.Linq;

namespace PersonalTracking
{
    public partial class FrmPermissionList : Form
    {

        PermissionDetailDTO detailPer = new PermissionDetailDTO();
        PermissionDTO dto = new PermissionDTO();
        private bool combofull;

        public FrmPermissionList()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmPermission frm = new FrmPermission();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            FillAllData();
            CleanFilters();

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (detailPer.PermissionID == 0)
            {
                MessageBox.Show("Select a permission");
            }
            else if (detailPer.State==PermissionStates.Approved || detailPer.State == PermissionStates.Disapproved)
            {
                MessageBox.Show("Can't update if it's already approved / disapproved ");
            }
            else
            {
                FrmPermission frm = new FrmPermission();
                frm.isUpdate = true;
                frm.detail = detailPer;

                this.Hide();
                frm.ShowDialog();

                this.Visible = true;
                FillAllData();
                CleanFilters();
            }
        }

        private void FillAllData()
        {
            dto = PermissionBLL.GetAll();
            if (!UserStatic.isAdmin)
            {
                dto.Permissions = dto.Permissions.Where(x => x.EmployeeID == UserStatic.EmployeeID).ToList();
            }

            dataGridView1.DataSource = dto.Permissions;

            combofull = false;

            cmbDepartment.DataSource = dto.Departments;
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "ID";
            cmbDepartment.SelectedIndex = -1;

            cmbPosition.DataSource = dto.Positions;
            cmbPosition.DisplayMember = "PositionName";
            cmbPosition.ValueMember = "ID";
            cmbPosition.SelectedIndex = -1;

            if (dto.Departments.Count > 0)
            {
                combofull = true;
            }

            cmbState.DataSource = dto.States;
            cmbState.DisplayMember = "StateName";
            cmbState.ValueMember = "ID";
            cmbState.SelectedIndex = -1;
        }

        private void FrmPermissionList_Load(object sender, EventArgs e)
        {

            FillAllData();


            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "User No";
            dataGridView1.Columns[2].HeaderText = "Name";
            dataGridView1.Columns[3].HeaderText = "Surname";
            dataGridView1.Columns[4].Visible = false; //DepartmentName
            dataGridView1.Columns[5].Visible = false; //PositionName
            dataGridView1.Columns[6].Visible = false; //DepartmentID
            dataGridView1.Columns[7].Visible = false; //PositionID
            dataGridView1.Columns[8].HeaderText = "Start Date";
            dataGridView1.Columns[9].HeaderText = "End Date";
            dataGridView1.Columns[10].HeaderText = "Day Amount";
            dataGridView1.Columns[11].HeaderText = "State";
            dataGridView1.Columns[12].Visible = false; // Old Amount
            dataGridView1.Columns[13].HeaderText = "Explanation";
            dataGridView1.Columns[14].Visible= false; // id Permission

            if (!UserStatic.isAdmin)
            {
                btnApprove.Visible = false;
                btnDisapprove.Hide();
                pnlForAdmin.Hide();
            }


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<PermissionDetailDTO> list = dto.Permissions;

            if (txtUserNo.Text.Trim() != "")
            {
                list = list.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            }
            if (txtName.Text.Trim() != "")
            {
                list = list.Where(x => x.Name.ToLower().Contains(txtName.Text.ToLower())).ToList(); // ToLower() in entrambi i valori per confrontarli in modo IgnoreCase
            }
            if (txtSurname.Text.Trim() != "")
            {
                list = list.Where(x => x.Surname.ToLower().Contains(txtSurname.Text.ToLower())).ToList();  // ToLower() in entrambi i valori per confrontarli in modo IgnoreCase
            }
            if (cmbDepartment.SelectedIndex != -1)
            {
                list = list.Where(x => x.DepartmentID == Convert.ToInt32(cmbDepartment.SelectedValue)).ToList();
            }
            if (cmbPosition.SelectedIndex != -1)
            {
                list = list.Where(x => x.PositionID == Convert.ToInt32(cmbPosition.SelectedValue)).ToList();
            }

            if (rbStartDate.Checked)
            {
                list = list.Where(x => x.StartDate < Convert.ToDateTime(dpEnd.Value)
                    && x.StartDate > Convert.ToDateTime(dpStart.Value)).ToList();
            }

            else if (rbEndDate.Checked)
            {
                list = list.Where(x => x.EndDate < Convert.ToDateTime(dpEnd.Value)
                    && x.EndDate > Convert.ToDateTime(dpStart.Value)).ToList();
            }
            if (cmbState.SelectedIndex != -1)
            {
                list = list.Where(x => x.State == Convert.ToInt32(cmbState.SelectedValue)).ToList();
            }

            if (txtDayAmount.Text.Trim() != "")
            {
                list = list.Where(x => x.PermissionDayAmount == Convert.ToInt32(txtDayAmount.Text)).ToList();
            }
            dataGridView1.DataSource = list;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CleanFilters();
        }

        private void CleanFilters()
        {
            txtUserNo.Clear();
            txtName.Clear();
            txtSurname.Clear();
            combofull = false;

            //resetta tutte le combo box
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.DataSource = dto.Positions;
            cmbPosition.SelectedIndex = -1;
            combofull = true;

            rbStartDate.Checked = false;
            rbEndDate.Checked = false;
            cmbState.SelectedIndex = -1;
            txtDayAmount.Clear();
            dataGridView1.DataSource = dto.Permissions;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            detailPer.PermissionID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[14].Value);
            detailPer.StartDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[8].Value);
            detailPer.EndDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[9].Value);
            detailPer.Explanation = dataGridView1.Rows[e.RowIndex].Cells[13].Value.ToString();

            detailPer.UserNo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
            detailPer.State = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[12].Value);
            detailPer.PermissionDayAmount = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[10].Value);

            detailPer.StateName= dataGridView1.Rows[e.RowIndex].Cells[11].Value.ToString();

        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            PermissionBLL.UpdatePermission(detailPer.PermissionID, PermissionStates.Approved);

            MessageBox.Show("Approved");
            FillAllData();
            CleanFilters();
        }

        private void btnDisapprove_Click(object sender, EventArgs e)
        {
            PermissionBLL.UpdatePermission(detailPer.PermissionID, PermissionStates.Disapproved);

            MessageBox.Show("Disapproved");
            FillAllData();
            CleanFilters();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (detailPer.PermissionID == 0)
            {
                MessageBox.Show("No permission selected");
            }
            else
            {
                if(detailPer.StateName=="Approved"|| detailPer.StateName == "Disapproved")
                {
                    MessageBox.Show("Can't delete permissions already approved or disapproved");
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        PermissionBLL.DeletePermission(detailPer.PermissionID);
                        MessageBox.Show("Permission deleted");
                        FillAllData();
                        CleanFilters();
                    }
                }
            }

        }

        private void btn_ExportToExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel.ExcelExport(dataGridView1);

        }
    }
}
