using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using DAL;
using DAL.DTO;
using System.IO;
namespace PersonalTracking
{
    public partial class FrmEmployee : Form
    {
        EmployeeDTO dto = new EmployeeDTO();
        bool combofull = false;
        string fileName = "";
        bool isUnique = false;
        public EmployeeDetailDTO detailEmpl = new EmployeeDetailDTO();
        public bool isUpdate=false;
        public FrmEmployee()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtUserNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUserNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.IsNumber(e);
        }

        private void txtSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.IsNumber(e);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmEmployee_Load(object sender, EventArgs e)
        {
            dto = EmployeeBLL.GetAll();
            cmbDepartment.DataSource = dto.Departments;
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "ID";

            cmbPosition.DataSource = dto.Positions;
            cmbPosition.DisplayMember = "PositionName";
            cmbPosition.ValueMember = "ID";

            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            
            combofull = true;

            if (isUpdate)
            {
                txtUserNo.Text = detailEmpl.UserNo.ToString();
                txtPassword.Text = detailEmpl.Password.ToString();

                txtName.Text = detailEmpl.Name;
                txtSurname.Text = detailEmpl.Surname;
                fileName = Application.StartupPath + "\\images\\"+detailEmpl.ImagePath;
                txtImgPath.Text = fileName;
                pictureBox1.ImageLocation = fileName;
                txtSalary.Text = detailEmpl.Salary.ToString();
                cmbDepartment.SelectedValue = detailEmpl.DepartmentID;
                cmbPosition.SelectedValue = detailEmpl.PositionID;
                dateTimePicker1.Value = (DateTime)detailEmpl.BirthDay;
                txtaddress.Text = detailEmpl.Address;
                chAdmin.Checked = detailEmpl.isAdmin;

                if (!UserStatic.isAdmin)
                {
                    chAdmin.Enabled = false;
                    txtUserNo.Enabled = false;
                    txtSalary.Enabled = false;
                    cmbDepartment.Enabled = false;
                    cmbPosition.Enabled = false;
                    btnCheck.Visible = false;
                }
            }

        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combofull)
            {
                //Quando viene selezionato un elemento della cmb Department, la cmb delle Posizione
                //viene aggiornata, facendo vedere solo le positions riguardante quel Department
                int departmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                cmbPosition.DataSource = dto.Positions.Where(x => x.Department_id == departmentID).ToList();
            }

          
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = "";
;                pictureBox1.Load(openFileDialog1.FileName);
                txtImgPath.Text = openFileDialog1.FileName;

                string Unique = Guid.NewGuid().ToString();
                fileName += Unique + openFileDialog1.SafeFileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUserNo.Text.Trim() == "")
                MessageBox.Show("User no. is Empty");
            else if(txtPassword.Text.Trim()=="")
                MessageBox.Show("Password is Empty");
            else if (txtName.Text.Trim() == "")
                MessageBox.Show("Name is Empty");
            else if (txtSalary.Text.Trim() == "")
                MessageBox.Show("Salary is Empty");
            else if (txtSurname.Text.Trim() == "")
                MessageBox.Show("Surname is Empty");
            else if (cmbDepartment.SelectedIndex == -1)
                MessageBox.Show("Department is Empty");
            else if (cmbPosition.SelectedIndex == -1)
                MessageBox.Show("Position is Empty");
            else
            {
                if (!isUpdate)
                {
                    if (!EmployeeBLL.isUnique(Convert.ToInt32(txtUserNo.Text)))
                    {
                        MessageBox.Show("This user is used by another employee");
                    }
                    else
                    {
                        Employee employee = new Employee();
                        employee.UserNo = Convert.ToInt32(txtUserNo.Text);
                        employee.Password = txtPassword.Text;
                        employee.isAdmin = chAdmin.Checked;
                        employee.Name = txtName.Text;
                        employee.SurName = txtSurname.Text;
                        employee.Salary = Convert.ToInt32(txtSalary.Text);
                        employee.Department_id = Convert.ToInt32(cmbDepartment.SelectedValue);
                        employee.Position_id = Convert.ToInt32(cmbPosition.SelectedValue);
                        employee.Adress = txtaddress.Text;
                        employee.Birthday = dateTimePicker1.Value;
                        employee.imgPath = fileName;
                        EmployeeBLL.AddEmployee(employee);
                        File.Copy(txtImgPath.Text, @"images\\" + fileName);
                        MessageBox.Show("Employee added");
                        txtUserNo.Clear();
                        txtPassword.Clear();
                        chAdmin.Checked = false;
                        txtName.Clear();
                        txtSurname.Clear();
                        txtSalary.Clear();
                        txtaddress.Clear();
                        txtImgPath.Clear();
                        pictureBox1.Image = null;
                        combofull = false;
                        cmbDepartment.SelectedIndex = -1;
                        cmbPosition.DataSource = dto.Positions;
                        cmbPosition.SelectedIndex = -1;
                        txtaddress.Clear();
                        combofull = true;
                        dateTimePicker1.Value = DateTime.Today;

                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Employee empToUpdate = new Employee();

                        if (txtImgPath.Text!=fileName)
                        {
                            if (File.Exists(@"images\\" + detailEmpl.ImagePath))
                            {
                                File.Delete(@"images\\" + detailEmpl.ImagePath);
                            }

                            File.Copy(txtImgPath.Text, @"images\\" + fileName);
                            empToUpdate.imgPath = fileName;
                        }
                        else
                        {
                            empToUpdate.imgPath = detailEmpl.ImagePath;
                        }
                        empToUpdate.ID = detailEmpl.EmployeeID;
                        empToUpdate.UserNo = Convert.ToInt32(txtUserNo.Text);
                        empToUpdate.Password = txtPassword.Text;
                        empToUpdate.Name = txtName.Text;
                        empToUpdate.SurName = txtSurname.Text;
                        empToUpdate.isAdmin = chAdmin.Checked;
                        empToUpdate.Salary = Convert.ToInt32(txtSalary.Text);
                        empToUpdate.Department_id = Convert.ToInt32(cmbDepartment.SelectedValue);
                        empToUpdate.Position_id = Convert.ToInt32(cmbPosition.SelectedValue);
                        empToUpdate.Birthday = dateTimePicker1.Value.Date;
                        empToUpdate.Adress = txtaddress.Text;

                        EmployeeBLL.UpdateEmployee(empToUpdate);

                        MessageBox.Show("Employee was updated");
                        this.Close();

                    }
                }
            }


        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (txtUserNo.Text.Trim() == "")
                MessageBox.Show("User no is Empty");
            else
            {
                isUnique = EmployeeBLL.isUnique(Convert.ToInt32(txtUserNo.Text));
                if (!isUnique)
                    MessageBox.Show("This user is used by another employee");
                else
                    MessageBox.Show("This user can be use");
            }
        }

    }
}
