using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DAL;
using BLL;
using DAL.DTO;

namespace PersonalTracking
{
    public partial class FrmPermission : Form
    {
        TimeSpan permissionDay;
        public bool isUpdate = false;
        public PermissionDetailDTO detail = new PermissionDetailDTO();
        public FrmPermission()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmPermission_Load(object sender, EventArgs e)
        {
            txtUserNo.Text = UserStatic.EmployeeID.ToString();

            if (isUpdate)
            {
                txtUserNo.Text = detail.UserNo.ToString();
                dpStart.Value = detail.StartDate;
                dpEnd.Value = detail.EndDate;
                txtAmount.Text = detail.PermissionDayAmount.ToString();
                txtExplanation.Text = detail.Explanation;
            }

        }

        private void dpStart_ValueChanged(object sender, EventArgs e)
        {
            permissionDay = dpEnd.Value.Date - dpStart.Value.Date;
            txtAmount.Text = permissionDay.TotalDays.ToString();
        }

        private void dpEnd_ValueChanged(object sender, EventArgs e)
        {
            permissionDay = dpEnd.Value.Date - dpStart.Value.Date;
            txtAmount.Text = permissionDay.TotalDays.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt32(txtAmount.Text) <= 0)
            {
                MessageBox.Show("Day amount has to be more than 0");
            }
            else if (String.IsNullOrEmpty(txtAmount.Text))
            {
                MessageBox.Show("Day amount empty");
            }
            else if (string.IsNullOrEmpty(txtExplanation.Text))
            {
                MessageBox.Show("Explanation empty");
            }

            else
            {
                Permisions permission = new Permisions();

                if (!isUpdate)
                {
                    permission.Employe_id = Convert.ToInt32(txtUserNo.Text);
                    permission.PermisionStart = dpStart.Value.Date;
                    permission.PermisionEnd = dpEnd.Value.Date;
                    permission.permisionDay = Convert.ToInt32(txtAmount.Text);
                    permission.PermisionExplanation = txtExplanation.Text;
                    permission.PermisionState = 1;
                    PermissionBLL.AddPermission(permission);

                    MessageBox.Show("Permission added.");
                    txtAmount.Clear();
                    txtExplanation.Clear();
                    dpStart.ResetText();
                    dpEnd.ResetText();
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure about that?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        permission.ID = detail.PermissionID;
                        permission.PermisionStart = dpStart.Value.Date;
                        permission.PermisionEnd = dpEnd.Value.Date;
                        permission.PermisionExplanation = txtExplanation.Text;
                        permission.permisionDay = Convert.ToInt32(txtAmount.Text);
                        PermissionBLL.UpdatePermission(permission);

                        MessageBox.Show("Permission was updated");
                        this.Close();

                    }
                }
            }
        }

    }
}
