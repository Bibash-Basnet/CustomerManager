using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CustomerManager
{
    public partial class CustomerManagerForm : Form
    {
        private const string DATABASE_NAME = "customers.db";
        private const string CONNECTION_STRING = "Data Source=customers.db;Version=3;";
        private const string VERSION_FILE_URL = "https://raw.githubusercontent.com/yourusername/CustomerManager/main/version.xml";
        private const string UPDATE_FILE_URL = "https://github.com/yourusername/CustomerManager/releases/download/v{0}/CustomerManager.exe";

        public CustomerManagerForm()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadCustomers();
        }

        // ==================== DATABASE OPERATIONS ====================

        /// <summary>
        /// Initialize SQLite database with customers table
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                // Create database if it doesn't exist
                using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Customers (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Email TEXT,
                            Phone TEXT,
                            Address TEXT,
                            CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                            ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";

                    using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Database initialized successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== CRUD OPERATIONS ====================

        /// <summary>
        /// Load all customers from database and display in DataGridView
        /// </summary>
        private void LoadCustomers()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    string query = "SELECT Id, Name, Email, Phone, Address FROM Customers ORDER BY Id DESC";

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvCustomers.DataSource = dt;

                        // Format columns
                        dgvCustomers.Columns["Id"].Width = 50;
                        dgvCustomers.Columns["Name"].Width = 150;
                        dgvCustomers.Columns["Email"].Width = 200;
                        dgvCustomers.Columns["Phone"].Width = 120;
                        dgvCustomers.Columns["Address"].Width = 300;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Add new customer to database
        /// </summary>
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Please fill in all required fields (Name is mandatory).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO Customers (Name, Email, Phone, Address)
                        VALUES (@name, @email, @phone, @address)";

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        command.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        command.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        command.Parameters.AddWithValue("@address", txtAddress.Text.Trim());

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Update selected customer
        /// </summary>
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Please select a customer to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs())
            {
                MessageBox.Show("Please fill in all required fields (Name is mandatory).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    string updateQuery = @"
                        UPDATE Customers 
                        SET Name = @name, Email = @email, Phone = @phone, Address = @address, ModifiedDate = CURRENT_TIMESTAMP
                        WHERE Id = @id";

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", int.Parse(txtId.Text));
                        command.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        command.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        command.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        command.Parameters.AddWithValue("@address", txtAddress.Text.Trim());

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Delete selected customer
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Please select a customer to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
                    {
                        connection.Open();

                        string deleteQuery = "DELETE FROM Customers WHERE Id = @id";

                        using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id", int.Parse(txtId.Text));
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadCustomers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // ==================== HELPER METHODS ====================

        private void ClearFields()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            dgvCustomers.ClearSelection();
        }

        private bool ValidateInputs()
        {
            return !string.IsNullOrWhiteSpace(txtName.Text);
        }

        /// <summary>
        /// When user selects a row in DataGridView, populate the form fields
        /// </summary>
        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvCustomers.SelectedRows[0];

                txtId.Text = row.Cells["Id"].Value?.ToString() ?? "";
                txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                txtAddress.Text = row.Cells["Address"].Value?.ToString() ?? "";
            }
        }

        // ==================== AUTO-UPDATE FUNCTIONALITY ====================

        /// <summary>
        /// Check for available updates from GitHub
        /// </summary>
        private void BtnCheckUpdates_Click(object sender, EventArgs e)
        {
            btnCheckUpdates.Enabled = false;
            lblUpdateStatus.Text = "Checking for updates...";
            lblUpdateStatus.ForeColor = System.Drawing.Color.Blue;

            try
            {
                string currentVersion = GetApplicationVersion();
                string latestVersion = GetLatestVersionFromGitHub();

                if (string.IsNullOrEmpty(latestVersion))
                {
                    lblUpdateStatus.Text = "Could not check for updates. Please check your internet connection.";
                    lblUpdateStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                if (IsNewerVersion(latestVersion, currentVersion))
                {
                    lblUpdateStatus.Text = $"Update available! Version {latestVersion} is ready to download.";
                    lblUpdateStatus.ForeColor = System.Drawing.Color.Green;

                    DialogResult result = MessageBox.Show(
                        $"A new version ({latestVersion}) is available. Current version: {currentVersion}\n\nDo you want to download and install it now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        DownloadAndInstallUpdate(latestVersion);
                    }
                }
                else
                {
                    lblUpdateStatus.Text = $"You are running the latest version ({currentVersion}).";
                    lblUpdateStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblUpdateStatus.Text = $"Error checking updates: {ex.Message}";
                lblUpdateStatus.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                btnCheckUpdates.Enabled = true;
            }
        }

        /// <summary>
        /// Get current application version from assembly
        /// </summary>
        private string GetApplicationVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        /// <summary>
        /// Fetch latest version from GitHub version.xml file
        /// </summary>
        private string GetLatestVersionFromGitHub()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (WebClient webClient = new WebClient())
                {
                    string xmlContent = webClient.DownloadString(VERSION_FILE_URL);
                    XDocument doc = XDocument.Parse(xmlContent);

                    XElement versionElement = doc.Root?.Element("Version");
                    return versionElement?.Value?.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching version info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Compare version strings to determine if update is available
        /// </summary>
        private bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            try
            {
                Version latest = new Version(latestVersion);
                Version current = new Version(currentVersion);
                return latest > current;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Download and install the update
        /// </summary>
        private void DownloadAndInstallUpdate(string version)
        {
            try
            {
                string updateUrl = string.Format(UPDATE_FILE_URL, version);
                string tempPath = Path.Combine(Path.GetTempPath(), "CustomerManager_Setup.exe");

                lblUpdateStatus.Text = "Downloading update...";
                Application.DoEvents();

                // Download the update
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(updateUrl, tempPath);
                }

                lblUpdateStatus.Text = "Download complete! Installing...";
                Application.DoEvents();

                // Execute the update
                System.Diagnostics.Process.Start(tempPath);

                // Close current application
                this.Close();
                Application.Exit();
            }
            catch (Exception ex)
            {
                lblUpdateStatus.Text = $"Error downloading update: {ex.Message}";
                lblUpdateStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Error: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}