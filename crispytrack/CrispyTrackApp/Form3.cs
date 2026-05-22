using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel; 

namespace CrispyTrackApp
{
    public partial class Form3 : Form
    {
        private ComboBox cmbTransactions;
        private DataGridView gridReport;
        private Button btnExport, btnBack;
        private Label lblTitle;
        private DataTable currentData; 
        
        // Hardcoding the exact path from your computer to guarantee it finds the image!
        private string logoPath = @"D:\xampp\htdocs\crispytrack\CrispyTrackApp\logo.jpg";

        public Form3()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "CrispyTrack - Report Generation Module";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            lblTitle = new Label { Text = "DATA EXPORT MODULE", Font = new Font("Arial", 16, FontStyle.Bold), ForeColor = Color.DarkRed, Location = new Point(20, 20), Size = new Size(300, 30) };
            
            Label lblSelect = new Label { Text = "Select Transaction Report:", Location = new Point(20, 70), Size = new Size(200, 20), Font = new Font("Arial", 10, FontStyle.Bold) };
            
            cmbTransactions = new ComboBox { Location = new Point(20, 95), Size = new Size(300, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTransactions.Items.AddRange(new string[] { "1. Sales Transactions", "2. Daily Activity Logs", "3. Department Staffing Summary" });
            cmbTransactions.SelectedIndexChanged += CmbTransactions_SelectedIndexChanged;

            btnExport = new Button { Text = "EXPORT TO MS EXCEL", Location = new Point(340, 90), Size = new Size(200, 35), BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Arial", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnExport.Click += BtnExport_Click;

            gridReport = new DataGridView { Location = new Point(20, 140), Size = new Size(740, 390), AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White };

            // --- NEW: Display the Logo in the App ---
            if (File.Exists(logoPath))
            {
                PictureBox pbLogo = new PictureBox {
                    ImageLocation = logoPath,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(620, 20),
                    Size = new Size(120, 100)
                };
                this.Controls.Add(pbLogo);
            }

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSelect);
            this.Controls.Add(cmbTransactions);
            this.Controls.Add(btnExport);
            this.Controls.Add(gridReport);
        }

        private void CmbTransactions_SelectedIndexChanged(object sender, EventArgs e)
        {
            DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                if (conn == null) return;
                string query = "";

                if (cmbTransactions.SelectedIndex == 0)
                {
                    query = "SELECT o.ID as OrderID, s.Name as Cashier, m.Item, o.Status, o.OrderDate FROM orders o JOIN staff s ON o.StaffID = s.ID JOIN menu m ON o.MenuID = m.ID";
                }
                else if (cmbTransactions.SelectedIndex == 1)
                {
                    query = "SELECT l.ID as LogID, s.Name as Staff, l.Action, l.LogTime FROM logs l JOIN staff s ON l.StaffID = s.ID";
                }
                else if (cmbTransactions.SelectedIndex == 2)
                {
                    query = "SELECT d.Name as Department, ds.StaffCount FROM dept_summary ds JOIN depts d ON ds.DeptID = d.ID";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                currentData = new DataTable();
                adapter.Fill(currentData);
                gridReport.DataSource = currentData;
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (currentData == null || currentData.Rows.Count == 0)
            {
                MessageBox.Show("Please select a transaction to generate a report.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Generating MS Excel Report... Please wait, this might take a few seconds!", "Processing", MessageBoxButtons.OK, MessageBoxIcon.Information);

            try
            {
                Excel.Application excelApp = new Excel.Application();
                if (excelApp == null)
                {
                    MessageBox.Show("Excel is not installed properly on this computer!");
                    return;
                }

                excelApp.Visible = true; 
                Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                
                // --- SHEET 1 WITH HEADER & SIGNATURE ---
                Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];
                sheet1.Name = "Report Data";

                Excel.Range titleCell = (Excel.Range)sheet1.Cells[1, 1];
                titleCell.Value2 = "CRISPY TRACK INC.";
                titleCell.Font.Bold = true;
                titleCell.Font.Size = 16;

                Excel.Range subtitleCell = (Excel.Range)sheet1.Cells[2, 1];
                subtitleCell.Value2 = "Official System Generated Report";
                
                // --- NEW: Inject the Real Logo into Excel! ---
                if (File.Exists(logoPath))
                {
                    Excel.Pictures pictures = (Excel.Pictures)sheet1.Pictures(Type.Missing);
                    Excel.Picture pic = pictures.Insert(logoPath);
                    // Position it at Column D (Cell 1, 4)
                    pic.Left = (double)((Excel.Range)sheet1.Cells[1, 4]).Left; 
                    pic.Top = (double)((Excel.Range)sheet1.Cells[1, 4]).Top;   
                    pic.Width = 100;
                    pic.Height = 100;
                }
                else
                {
                    // Fallback just in case the image gets moved
                    Excel.Range logoRange = sheet1.Range["D1", "E3"];
                    logoRange.Merge();
                    logoRange.Value2 = "[ LOGO FILE NOT FOUND ]";
                    logoRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    logoRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    logoRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                }

                // Print Column Headers
                for (int i = 0; i < currentData.Columns.Count; i++)
                {
                    Excel.Range headerCell = (Excel.Range)sheet1.Cells[5, i + 1];
                    headerCell.Value2 = currentData.Columns[i].ColumnName;
                    headerCell.Font.Bold = true;
                    headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                }

                // Print the Data Rows
                int rowIndex = 6;
                for (int i = 0; i < currentData.Rows.Count; i++)
                {
                    for (int j = 0; j < currentData.Columns.Count; j++)
                    {
                        Excel.Range dataCell = (Excel.Range)sheet1.Cells[rowIndex, j + 1];
                        dataCell.Value2 = currentData.Rows[i][j].ToString();
                    }
                    rowIndex++;
                }

                // Signature Placeholder
                rowIndex += 3; 
                Excel.Range sig1 = (Excel.Range)sheet1.Cells[rowIndex, 1];
                sig1.Value2 = "Prepared and Signed By:";
                
                Excel.Range sig2 = (Excel.Range)sheet1.Cells[rowIndex + 2, 1];
                sig2.Value2 = "_______________________";
                
                Excel.Range sig3 = (Excel.Range)sheet1.Cells[rowIndex + 3, 1];
                sig3.Value2 = "System Administrator";

                sheet1.Columns.AutoFit(); 

                // --- SHEET 2 WITH GRAPH ---
                Excel.Worksheet sheet2 = (Excel.Worksheet)workbook.Sheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                sheet2.Name = "Data Visualization";

                Excel.ChartObjects xlCharts = (Excel.ChartObjects)sheet2.ChartObjects(Type.Missing);
                Excel.ChartObject myChart = xlCharts.Add(10, 10, 500, 300);
                Excel.Chart chart = myChart.Chart;

                Excel.Range chartRange = sheet1.Range[sheet1.Cells[5, 1], sheet1.Cells[rowIndex - 4, currentData.Columns.Count]];
                chart.SetSourceData(chartRange);
                chart.ChartType = Excel.XlChartType.xlColumnClustered; 
                chart.HasTitle = true;
                chart.ChartTitle.Text = cmbTransactions.SelectedItem.ToString() + " Graph";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Excel report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}