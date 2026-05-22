using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace CrispyTrackApp
{
    // Activity 5 Requirement: MySQL connection must be a public class
    public class DatabaseConnection
    {
        // Port 3307 to match your XAMPP configuration
        private string connectionString = "Server=127.0.0.1;Port=3306;Database=fried_chicken;Uid=root;Pwd=;";
        public MySqlConnection conn;

        public MySqlConnection GetConnection()
        {
            try
            {
                conn = new MySqlConnection(connectionString);
                return conn;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}