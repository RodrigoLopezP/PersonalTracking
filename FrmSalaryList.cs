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
using System.Linq;

namespace PersonalTracking
{
    public partial class FrmSalaryList : Form
    {
        SalaryDTO dto = new SalaryDTO();
        private bool combofull = false;
        SalaryDetailDTO detailSal = new SalaryDetailDTO();
        public FrmSalaryList()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmSalary frm = new FrmSalary();
            this.Hide();
            frm.ShowDialog();
            //Dopo che la finestra FrmSalary si è chiusa, FrmSalaryList riaggiorna tutti i dati della tabella

            this.Visible = true;
            FillAllData();
            CleanFilters();
        }

        private void FrmSalaryList_Load(object sender, EventArgs e)
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
            dataGridView1.Columns[8].HeaderText = "Month";
            dataGridView1.Columns[9].HeaderText = "Year";
            dataGridView1.Columns[10].Visible = false;//Salary ID
            dataGridView1.Columns[11].HeaderText = "Salary";
            dataGridView1.Columns[12].Visible = false;//OldAmmount
            dataGridView1.Columns[13].Visible = false; //MonthID

            if (!UserStatic.isAdmin)
            {
                pnlForAdmin.Visible = false;
                //btnNew.Hide();
                btnUpdate.Hide();
                btnDelete.Hide();
            }
        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combofull)
            {
                cmbPosition.DataSource = dto.Positions.Where(x =>
                    x.Department_id == Convert.ToInt32(cmbDepartment.SelectedValue)).ToList();
            }
        }

        private void FillAllData()
        {
            dto = SalaryBLL.GetAll();

            if (!UserStatic.isAdmin)
            {
                dto.Salaries = dto.Salaries.Where(x => x.EmployeeID == UserStatic.EmployeeID).ToList();
            }


            dataGridView1.DataSource = dto.Salaries;

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

            cmbMonth.DataSource = dto.Months;
            cmbMonth.DisplayMember = "MonthName";
            cmbMonth.ValueMember = "ID";
            cmbMonth.SelectedIndex = -1;
        }

        private void CleanFilters()
        {
            txtUserNo.Clear();
            txtName.Clear();
            txtSurname.Clear();
            txtYear.Clear();
            txtSalary.Clear();
            combofull = false;

            //resetta tutte le combo box
            cmbDepartment.DataSource = dto.Departments;
            cmbDepartment.SelectedIndex = -1;

            cmbPosition.DataSource = dto.Positions;
            cmbPosition.SelectedIndex = -1;

            cmbMonth.DataSource = dto.Months;
            cmbMonth.SelectedIndex = -1;
            combofull = true;

            rbMore.Checked = false;
            rbLess.Checked = false;
            rbEquals.Checked = true;
            dataGridView1.DataSource = dto.Salaries;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CleanFilters();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<SalaryDetailDTO> list = dto.Salaries;

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

            if (!String.IsNullOrEmpty(txtYear.Text))
            {
                list = list.Where(x => x.SalaryYear == Convert.ToInt32(txtYear.Text)).ToList();
            }

            if (cmbMonth.SelectedIndex != -1)
            {
                list = list.Where(x => x.MonthID == Convert.ToInt32(cmbMonth.SelectedValue)).ToList();
            }

            if (!String.IsNullOrEmpty(txtSalary.Text))
            {

                if (rbMore.Checked)
                {
                    list = list.Where(x => x.SalaryAmount < Convert.ToInt32(txtSalary.Text)).ToList();
                }
                else if(rbLess.Checked)
                {
                    list = list.Where(x => x.SalaryAmount > Convert.ToInt32(txtSalary.Text)).ToList();
                }
                else
                {
                    list = list.Where(x => x.SalaryAmount == Convert.ToInt32(txtSalary.Text)).ToList();
                }

            }



            dataGridView1.DataSource = list;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            detailSal.Name= dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            detailSal.Surname = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            detailSal.UserNo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
            detailSal.SalaryID= Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[10].Value);
            detailSal.EmployeeID= Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            detailSal.SalaryYear = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[9].Value);
            detailSal.MonthID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[13].Value);
            detailSal.SalaryAmount = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[11].Value);
            detailSal.OldSalary = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[11].Value);
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (detailSal.SalaryID == 0)
            {
                MessageBox.Show("Select a Salary");
            }
            else
            {
                FrmSalary frm = new FrmSalary();
                frm.isUpdate = true;
                frm.detailSalary = detailSal;

                this.Hide();
                frm.ShowDialog();
                //Dopo che la finestra FrmSalary si è chiusa, FrmSalaryList riaggiorna tutti i dati della tabella

                this.Visible = true;
                FillAllData();
                CleanFilters();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (detailSal.SalaryID == 0)
            {
                MessageBox.Show("No salary selected");
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SalaryBLL.DeletePermission(detailSal.SalaryID);
                    MessageBox.Show("Salary deleted");
                    FillAllData();
                    CleanFilters();
                }
            }
        }

        private void btn_ExportToExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel.ExcelExport(dataGridView1);

        }
    }
}
