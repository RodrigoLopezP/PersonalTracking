using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BLL;
using DAL;
using DAL.DTO;


namespace PersonalTracking
{
    public partial class FrmLogin : Form
    {
     
        public FrmLogin()
        {
            
            
            InitializeComponent();
          
        }

        private void txtUserNo_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtUserNo_KeyPress(object sender, KeyPressEventArgs e)
        {
             e.Handled = General.IsNumber(e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserNo.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("UserNo or password empty.");
            }
            else
            {
                List<Employee> empList = EmployeeBLL.GetEmployees(Convert.ToInt32(txtUserNo.Text), txtPassword.Text);
                if (empList.Count == 0)
                {
                    MessageBox.Show("Wrong credentials. Please check your information.");
                }
                else
                {
                    Employee employee = new Employee();
                    employee = empList.First();
                    UserStatic.EmployeeID = employee.ID;
                    UserStatic.UserNo = employee.UserNo;
                    UserStatic.isAdmin = Convert.ToBoolean(employee.isAdmin);
                    FrmMain frm = new FrmMain();
                    this.Hide();
                    frm.ShowDialog();
                }
            }
        }
    }
}
