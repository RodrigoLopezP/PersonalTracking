using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DAL.DTO;
using BLL;
using System.Linq;
using DAL;

namespace PersonalTracking
{
    public partial class FrmTask : Form
    {
        TaskDTO dto = new TaskDTO();    //oggetto il quale contiene taskdetail e altre liste che serviranno al Form Task
        private bool combofull=false;   // var di appoggio per capire il fill dinamico delle posizione a seconda del dipartimento selezionato
        Tasks taskToInsert = new Tasks(); //creo nuovo oggetto Tasks per fare insert dal gestionale al db, tramite linq
        public TaskDetailDTO detail = new TaskDetailDTO(); // usata per l'update di una task
        public bool isUpdate = false;

        public FrmTask()
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

        private void FrmTask_Load(object sender, EventArgs e)
        {
            label9.Visible = false;
            cmbTaskState.Visible = false;
            dto = TaskBLL.GetAll();

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

            combofull = true;

            cmbTaskState.DataSource = dto.TasksStates;
            cmbTaskState.DisplayMember = "StateName";
            cmbTaskState.ValueMember = "ID";
            cmbTaskState.SelectedIndex = -1;

            if (isUpdate)
            {
                label9.Visible = true;
                cmbTaskState.Visible = true;
                cmbTaskState.DataSource = dto.TasksStates;
                cmbTaskState.DisplayMember = "StateName";
                cmbTaskState.ValueMember = "ID";
                cmbTaskState.SelectedIndex = -1;
                txtName.Text = detail.Name;
                txtUserNo.Text = detail.UserNo.ToString();
                txtSurname.Text = detail.Surname;
                txtTitle.Text = detail.Title;
                txtContent.Text = detail.Content;
                cmbTaskState.SelectedValue = detail.taskStateID;
            }
        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combofull)
            {
                cmbPosition.DataSource = dto.Positions.Where(x => 
                    x.Department_id == Convert.ToInt32(cmbDepartment.SelectedValue)).ToList();

                List<EmployeeDetailDTO> list = dto.Employees;
                dataGridView1.DataSource = list.Where(x =>
                      x.DepartmentID == Convert.ToInt32(cmbDepartment.SelectedValue)).ToList();


            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combofull)
            {
                List<EmployeeDetailDTO> list = dto.Employees;
                dataGridView1.DataSource = list.Where(x =>
                      x.PositionID == Convert.ToInt32(cmbPosition.SelectedValue)
                      //&&
                      //x.DepartmentID==Convert.ToInt32(cmbDepartment.SelectedValue)
                      ).ToList();
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (cmbTaskState.SelectedIndex == -1)
            //    MessageBox.Show("Select task state");
            if (String.IsNullOrEmpty(txtTitle.Text))
                MessageBox.Show("Task title empty");
            else if (String.IsNullOrEmpty(txtContent.Text))
                MessageBox.Show("Task content empty");
            else
            {
                if (!isUpdate)
                {
                    taskToInsert.taskTitle = txtTitle.Text;
                    taskToInsert.taskContent = txtContent.Text;
                    taskToInsert.taskState = 1;
                    taskToInsert.tasStartDate = DateTime.Today;
                    //task_employee_id inserito nel cmbPosition_SelectedIndexChanged
                    TaskBLL.AddTask(taskToInsert);

                    MessageBox.Show("New task added.");

                    ///Sono state commentate le righe di codice sotto perché
                    ///se i fields vengono svuotati  dopo una insert task
                    ///è possibile inserire una task senza nessuna persona assegnata,
                    ///se non viene clickato nessun employee (employee id 0)nella grid a destra della finestra, creando un bug
                    //txtUserNo.Clear();
                    //txtName.Clear();
                    //txtSurname.Clear();

                    txtContent.Clear();
                    txtTitle.Clear();

                    taskToInsert = new Tasks();
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure about that?", "Warning!!", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Tasks taskToUpdate = new Tasks();
                        if(Convert.ToInt32(txtUserNo.Text) != detail.UserNo) // se si è cliccato la grid dei dipendenti che si trova nel lato destro del form, prende l'id dal taskToInsert.employe_id
                        {
                            taskToUpdate.Employe_id = taskToInsert.Employe_id;
                        }
                        else
                        {
                            taskToUpdate.Employe_id = detail.EmployeeID;
                        }
                        
                        taskToUpdate.taskTitle = txtTitle.Text;
                        taskToUpdate.taskContent = txtContent.Text;
                        taskToUpdate.taskState = Convert.ToInt32(cmbTaskState.SelectedValue);
                        taskToUpdate.ID = detail.TaskID;
                        TaskBLL.UpdateTask(taskToUpdate);

                        MessageBox.Show("Permission was updated");
                        this.Close();
                    }
                }

            }


        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtUserNo.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtSurname.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            taskToInsert.Employe_id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
        }
    }
}
