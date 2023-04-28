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
    public partial class FrmSalary : Form
    {
        //Creazione oggetto contenente tabelle che verranno usate in questo frm
        SalaryDTO dto = new SalaryDTO();
        private bool combofull;
        public SalaryDetailDTO detailSalary = new SalaryDetailDTO();
        public bool isUpdate = false;
        int oldSalary = 0;
        public FrmSalary()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSalary_Load(object sender, EventArgs e)
        {
            dto = SalaryBLL.GetAll();
            if (!isUpdate)
            {
                dataGridView1.DataSource = dto.Employees;

                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "User No";
                dataGridView1.Columns[2].HeaderText = "Name";
                dataGridView1.Columns[3].HeaderText = "Surname";
                dataGridView1.Columns[4].Visible = false;
                dataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[8].Visible = false;
                dataGridView1.Columns[9].Visible = false;
                dataGridView1.Columns[10].Visible = false;
                dataGridView1.Columns[11].Visible = false;
                dataGridView1.Columns[12].Visible = false;
                dataGridView1.Columns[13].Visible = false;

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
            }

            cmbMonth.DataSource = dto.Months;
            cmbMonth.DisplayMember = "MonthName";
            cmbMonth.ValueMember = "ID";
            cmbMonth.SelectedIndex = -1;

            if (isUpdate)
            {
                panel1.Hide();
                txtUserNo.Text = detailSalary.UserNo.ToString();
                txtName.Text = detailSalary.Name.ToString();
                txtSalary.Text = detailSalary.SalaryAmount.ToString();
                txtSurname.Text = detailSalary.Surname;
                txtYear.Text = detailSalary.SalaryYear.ToString();
                cmbMonth.SelectedValue = detailSalary.MonthID;
            }
        }

        Salary salary = new Salary();
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtUserNo.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtSurname.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtYear.Text = DateTime.Today.Year.ToString();

            txtSalary.Text = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();

            ///di default la finestra punta alla prima riga della tabella,
            ///quindi si salva di già il valore nell'oggetto salary nel caso
            ///si aggiungesse il salario senza cliccare sulla tabella degli employee a destra
            salary.Employe_id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

            oldSalary= Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[8].Value);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtSalary.Text))
                MessageBox.Show("Salary empty");
            else if (String.IsNullOrEmpty(txtYear.Text))
                MessageBox.Show("Year empty");
            else if(cmbMonth.SelectedIndex==-1)
                MessageBox.Show("Month empty");
            else if (String.IsNullOrEmpty(txtUserNo.Text))
                MessageBox.Show("User empty");
            else
            {
                bool control = false;
                if (!isUpdate)
                {
                    salary.Year = Convert.ToInt32(txtYear.Text);
                    salary.Month_id = Convert.ToInt32(cmbMonth.SelectedValue);
                    salary.Amount = Convert.ToInt32(txtSalary.Text);
                    if (salary.Amount > oldSalary)
                    {
                        control = true;
                    }
                    SalaryBLL.AddSalary(salary, control);
                    MessageBox.Show("Salary was added.");

                    cmbDepartment.SelectedIndex = -1;
                    txtYear.Clear();
                    txtSalary.Clear();
                    cmbMonth.SelectedIndex = -1;

                    salary = new Salary();
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Salary salaryToUpdate = new Salary();
                        salaryToUpdate.ID = detailSalary.SalaryID;
                        salaryToUpdate.Employe_id = detailSalary.EmployeeID;
                        salaryToUpdate.Year = Convert.ToInt32(txtYear.Text);
                        salaryToUpdate.Month_id = Convert.ToInt32(cmbMonth.SelectedValue);
                        salaryToUpdate.Amount = Convert.ToInt32(txtSalary.Text);
                        if (salaryToUpdate.Amount > detailSalary.OldSalary)
                        {
                            control = true;
                        }

                        SalaryBLL.UpdateSalary(salaryToUpdate,control);

                        MessageBox.Show("Salary was updated");
                        this.Close();
                    }
                }
            }

        }
    }
}
