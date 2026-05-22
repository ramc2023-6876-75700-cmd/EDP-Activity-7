using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CrispyTrackApp
{
    public partial class Form2 : Form
    {
        private DataGridView gridUsers;
        private TextBox txtSearch, txtId, txtName, txtEmail;
        private ComboBox cmbDept;
        private Button btnSearch, btnAdd, btnUpdate, btnDelete, btnRecover, btnClear, btnToggle;

        public Form2()
        {
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "CrispyTrack - User Management";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Form Title
            Label lblTitle = new Label { Text = "ACCOUNT MANAGEMENT", Font = new Font("Arial", 16, FontStyle.Bold), ForeColor = Color.DarkRed, Location = new Point(20, 20), Size = new Size(300, 30) };
            this.Controls.Add(lblTitle);

            // --- LEFT PANEL: INPUT FORM ---
            Label lblId = new Label { Text = "ID (Auto):", Location = new Point(20, 70), Size = new Size(100, 20) };
            txtId = new TextBox { Location = new Point(20, 90), Size = new Size(200, 25), ReadOnly = true, BackColor = Color.LightGray };
            
            Label lblName = new Label { Text = "Name:", Location = new Point(20, 120), Size = new Size(100, 20) };
            txtName = new TextBox { Location = new Point(20, 140), Size = new Size(200, 25) };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(20, 170), Size = new Size(100, 20) };
            txtEmail = new TextBox { Location = new Point(20, 190), Size = new Size(200, 25) };

            Label lblDept = new Label { Text = "Department:", Location = new Point(20, 220), Size = new Size(100, 20) };
            cmbDept = new ComboBox { Location = new Point(20, 240), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDept.Items.AddRange(new object[] { "1 - Kitchen", "2 - Counter", "3 - Delivery", "4 - Storage" });

            btnAdd = new Button { Text = "Add Account", Location = new Point(20, 290), Size = new Size(95, 35), BackColor = Color.ForestGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = new Button { Text = "Update", Location = new Point(125, 290), Size = new Size(95, 35), BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = new Button { Text = "Delete Member", Location = new Point(20, 380), Size = new Size(200, 35), BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;

            btnRecover = new Button { Text = "Recover Password", Location = new Point(20, 425), Size = new Size(200, 35), BackColor = Color.DarkSlateBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRecover.Click += BtnRecover_Click;

            Button btnReport = new Button { Text = "Reports Module", Location = new Point(650, 20), Size = new Size(120, 35), BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnReport.Click += BtnReport_Click;
            this.Controls.Add(btnReport);

            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRecover);

            btnClear = new Button { Text = "Clear Fields", Location = new Point(20, 335), Size = new Size(200, 30), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear.Click += (s, e) => ClearInputs();

            this.Controls.AddRange(new Control[] { lblId, txtId, lblName, txtName, lblEmail, txtEmail, lblDept, cmbDept, btnAdd, btnUpdate, btnClear });

            // --- RIGHT PANEL: SEARCH & GRID ---
            txtSearch = new TextBox { Location = new Point(250, 70), Size = new Size(300, 25) };
            btnSearch = new Button { Text = "Search", Location = new Point(560, 68), Size = new Size(80, 28), BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSearch.Click += (s, e) => LoadData(txtSearch.Text);

            btnToggle = new Button { Text = "Toggle Active/Inactive", Location = new Point(650, 68), Size = new Size(120, 28), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnToggle.Click += BtnToggle_Click;

            gridUsers = new DataGridView { Location = new Point(250, 110), Size = new Size(520, 420), AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White };
            gridUsers.CellClick += GridUsers_CellClick;

            this.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnToggle, gridUsers });

            this.FormClosed += (s, e) => Application.Exit(); // Kills the app when closing this window
        }

        private void LoadData(string searchTerm = "")
        {
            DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                if (conn == null) return;
                string query = "SELECT staff.ID, staff.Name, depts.Name AS Department, staff.email, staff.status, staff.DeptID FROM staff LEFT JOIN depts ON staff.DeptID = depts.ID";
                if (!string.IsNullOrEmpty(searchTerm)) query += " WHERE staff.Name LIKE @search";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                if (!string.IsNullOrEmpty(searchTerm)) cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                gridUsers.DataSource = dt;
                gridUsers.Columns["DeptID"].Visible = false; // Hide the raw ID column
            }
        }

        private void GridUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridUsers.Rows[e.RowIndex];
                txtId.Text = row.Cells["ID"].Value.ToString();
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                string deptId = row.Cells["DeptID"].Value.ToString();
                
                for (int i = 0; i < cmbDept.Items.Count; i++)
                {
                    if (cmbDept.Items[i].ToString().StartsWith(deptId)) cmbDept.SelectedIndex = i;
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || cmbDept.SelectedIndex == -1) { MessageBox.Show("Name and Dept required."); return; }
            
            DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                string deptId = cmbDept.SelectedItem.ToString().Split('-')[0].Trim();
                // Get next ID
                conn.Open();
                MySqlCommand idCmd = new MySqlCommand("SELECT COALESCE(MAX(ID), 0) + 1 FROM staff", conn);
                int nextId = Convert.ToInt32(idCmd.ExecuteScalar());

                string query = "INSERT INTO staff (ID, Name, DeptID, email, password, status) VALUES (@id, @name, @dept, @email, 'password123', 'Active')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", nextId);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@dept", deptId);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.ExecuteNonQuery();
            }
            LoadData(); ClearInputs();
            MessageBox.Show("Account Added!");
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) { MessageBox.Show("Select a user to update."); return; }

            DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string deptId = cmbDept.SelectedItem.ToString().Split('-')[0].Trim();
                string query = "UPDATE staff SET Name=@name, email=@email, DeptID=@dept WHERE ID=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@dept", deptId);
                cmd.Parameters.AddWithValue("@id", txtId.Text);
                cmd.ExecuteNonQuery();
            }
            LoadData(); ClearInputs();
            MessageBox.Show("Profile Updated!");
        }

        private void BtnToggle_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count == 0) { MessageBox.Show("Select a user from the grid to toggle."); return; }

            string id = gridUsers.SelectedRows[0].Cells["ID"].Value.ToString();
            string currentStatus = gridUsers.SelectedRows[0].Cells["status"].Value.ToString();
            string newStatus = currentStatus == "Active" ? "Inactive" : "Active";

            DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "UPDATE staff SET status=@status WHERE ID=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            LoadData();
        }

        private void ClearInputs()
        {
            txtId.Text = ""; txtName.Text = ""; txtEmail.Text = ""; cmbDept.SelectedIndex = -1;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) { MessageBox.Show("Please select a member from the list to delete."); return; }

             var confirmResult = MessageBox.Show("Are you sure you want to delete " + txtName.Text + "?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
             DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
            conn.Open();
            string query = "DELETE FROM staff WHERE ID=@id";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", txtId.Text);
            cmd.ExecuteNonQuery();
            }
             LoadData(); 
            ClearInputs();
            MessageBox.Show("Member successfully deleted.");
    }
}

        private void BtnRecover_Click(object sender, EventArgs e)
        {
         if (string.IsNullOrEmpty(txtEmail.Text)) 
         { 
            MessageBox.Show("Please select a member to recover their access key."); 
            return; 
         }
            MessageBox.Show("A password recovery link has been sent to: " + txtEmail.Text + 
                    "\n\nTemporary Access Key: password123", "Recovery System", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReport_Click(object sender, EventArgs e)
            {
             Form3 reportForm = new Form3();
             reportForm.ShowDialog(); 
        }   
    }
}