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
    public partial class FrmTaskList : Form
    {
        TaskDTO dto = new TaskDTO();
        private bool combofull = false;
        TaskDetailDTO detailTask = new TaskDetailDTO();

        public FrmTaskList()
        {
            InitializeComponent();
        }

        private void txtUserNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.IsNumber(e);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmTaskList_Load(object sender, EventArgs e)
        {
            FillAllData();

            dataGridView1.Columns[0].HeaderText = "Task Title";
            dataGridView1.Columns[1].HeaderText = "User No";
            dataGridView1.Columns[2].HeaderText = "Name";
            dataGridView1.Columns[3].HeaderText = "Surname";
            dataGridView1.Columns[4].HeaderText = "Start Date";
            dataGridView1.Columns[5].HeaderText = "Delivery Date";
            dataGridView1.Columns[6].HeaderText = "Task State";
            dataGridView1.Columns[7].Visible = false; //Department name
            dataGridView1.Columns[8].Visible = false; //Position name
            dataGridView1.Columns[9].Visible = false; //Department ID
            dataGridView1.Columns[10].Visible = false;//Position ID
            dataGridView1.Columns[11].Visible = false;//Task ID
            dataGridView1.Columns[12].Visible = false;//Employee ID
            dataGridView1.Columns[13].Visible = false;//Content
            dataGridView1.Columns[14].Visible = false;//task State ID

            if (!UserStatic.isAdmin)
            {
                btnNew.Visible = false;
                btnUpdate.Visible = false;
                btnDelete.Visible = false;
                pnlForAdmin.Visible = false;
                btnApprove.Text = "Delivery";
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmTask frm = new FrmTask();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            FillAllData();
            CleanFilters();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            
            FrmTask frm = new FrmTask();
            frm.detail = detailTask;
            frm.isUpdate = true;
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combofull)
            {
                cmbPosition.DataSource = dto.Positions.Where(x =>
                    x.Department_id == Convert.ToInt32(cmbDepartment.SelectedValue)).ToList();

            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<TaskDetailDTO> list = dto.Tasks;

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
                list = list.Where(x => x.TasStartDate > Convert.ToDateTime(dpStart.Value) 
                    && x.TasStartDate < Convert.ToDateTime(dpEnd.Value)).ToList();
            }
            if (rbDeliveryDate.Checked)
            {
                list = list.Where(x => x.TaskDeliveryDate > Convert.ToDateTime(dpStart.Value)
                    && x.TaskDeliveryDate < Convert.ToDateTime(dpEnd.Value)).ToList();
            }
            if(cmbTaskState.SelectedIndex!=-1)
            {
                list = list.Where(x => x.taskStateID == Convert.ToInt32(cmbTaskState.SelectedValue)).ToList();
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

            rbDeliveryDate.Checked = false;
            rbStartDate.Checked = false;
            cmbTaskState.SelectedIndex = -1;
            dataGridView1.DataSource = dto.Tasks;
        }

        private void FillAllData()
        {
            dto = TaskBLL.GetAll();

            if (!UserStatic.isAdmin)
            {
                dto.Tasks = dto.Tasks.Where(x => x.EmployeeID == UserStatic.EmployeeID).ToList();
            }

            dataGridView1.DataSource = dto.Tasks;

            combofull = false;

            cmbDepartment.DataSource = dto.Departments;
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "ID";
            cmbDepartment.SelectedIndex = -1;

            cmbPosition.DataSource = dto.Positions;
            cmbPosition.DisplayMember = "PositionName";
            cmbPosition.ValueMember = "ID";
            cmbPosition.SelectedIndex = -1;

            combofull = true;

            cmbTaskState.DataSource = dto.TasksStates;
            cmbTaskState.DisplayMember = "StateName";
            cmbTaskState.ValueMember = "ID";
            cmbTaskState.SelectedIndex = -1;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            detailTask.Title= dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            detailTask.UserNo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
            detailTask.Name = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            detailTask.Surname = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            detailTask.TasStartDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value);
            detailTask.TaskDeliveryDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[5].Value);
            detailTask.TaskStateName= dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            detailTask.TaskID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[11].Value);
            detailTask.EmployeeID= Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[12].Value);   
            detailTask.Content= dataGridView1.Rows[e.RowIndex].Cells[13].Value.ToString();
            detailTask.taskStateID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[14].Value);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (detailTask.TaskID == 0)
            {
                MessageBox.Show("Please select a task.");
            }
            else
            {
                FrmTask frm = new FrmTask();

                frm.isUpdate = true;
                frm.detail = detailTask;

                this.Hide();
                frm.ShowDialog();
                this.Visible = true;
                FillAllData();
                CleanFilters();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (detailTask.TaskID == 0)
            {
                MessageBox.Show("No task selected");
            }
            else
            {
                if (detailTask.taskStateID == 2 || detailTask.taskStateID == 3) // if delivered or approved
                {
                    MessageBox.Show("Cant delete task delivered or approved");
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        TaskBLL.DeleteTask(detailTask.TaskID);
                        MessageBox.Show("Task deleted");
                        FillAllData();
                        CleanFilters();
                    }
                }
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if(UserStatic.isAdmin && detailTask.taskStateID==TaskStates.OnEmployee&& detailTask.EmployeeID != UserStatic.EmployeeID)
            {
                MessageBox.Show("Before approve a task, an employee have to delivery task");
            }
            else if (UserStatic.isAdmin && detailTask.taskStateID == TaskStates.Approved)
            {
                MessageBox.Show("This task is already approved");
            }
            else if (!UserStatic.isAdmin && detailTask.taskStateID == TaskStates.Approved)
            {
                MessageBox.Show("This task is already approved");
            }
            else if (!UserStatic.isAdmin && detailTask.taskStateID == TaskStates.Delivered)
            {
                MessageBox.Show("This task is already delivered");
            }
            else
            {
                TaskBLL.ApproveTask(detailTask.TaskID, UserStatic.isAdmin);
                MessageBox.Show("Task updated");
                FillAllData();
                CleanFilters();

            }

        }

        private void btn_ExportToExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel.ExcelExport(dataGridView1);

        }
    }
}
