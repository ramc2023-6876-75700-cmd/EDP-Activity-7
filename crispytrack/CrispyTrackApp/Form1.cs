using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CrispyTrackApp
{
    public partial class Form1 : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;

        public Form1()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "CrispyTrack - Login";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            lblTitle = new Label { Text = "CRISPY TRACK", Font = new Font("Arial", 16, FontStyle.Bold), ForeColor = Color.DarkRed, Location = new Point(100, 30), Size = new Size(200, 30), TextAlign = ContentAlignment.MiddleCenter };
            
            Label lblUser = new Label { Text = "Username:", Location = new Point(50, 90), Size = new Size(100, 20) };
            txtUsername = new TextBox { Location = new Point(50, 110), Size = new Size(280, 30) };

            Label lblPass = new Label { Text = "Access Key:", Location = new Point(50, 150), Size = new Size(100, 20) };
            txtPassword = new TextBox { Location = new Point(50, 170), Size = new Size(280, 30), UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "SIGN IN", Location = new Point(50, 220), Size = new Size(280, 40), BackColor = Color.DarkRed, ForeColor = Color.White, Font = new Font("Arial", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnLogin.Click += BtnLogin_Click; 

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            DatabaseConnection db = new DatabaseConnection();
            MySqlConnection conn = db.GetConnection();

            if (conn != null)
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Name, status FROM staff WHERE Name = @user AND password = @pass LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string status = reader["status"].ToString();
                            if (status == "Active")
                            {
                                MessageBox.Show("Welcome back, " + reader["Name"].ToString() + "!", "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Hide(); 
                                new Form2().Show();
                            }
                            else
                            {
                                MessageBox.Show("Account is inactive. Contact Admin.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}