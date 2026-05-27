using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CustomerManager
{
    public partial class CustomerManagerForm : Form
    {
        private const string DATABASE_NAME = "customers.db";
        private const string CONNECTION_STRING = "Data Source=customers.db;Version=3;";
        private const string VERSION_FILE_URL = "https://raw.githubusercontent.com/Bibash-Basnet/CustomerManager/main/version.xml";
        private const string UPDATE_FILE_URL = "https://github.com/Bibash-Basnet/CustomerManager/releases/download/v{0}/CustomerManager-Setup-v{0}.exe";

        public CustomerManagerForm()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadCustomers();
            CheckForUpdatesSilent();
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

        private void CheckForUpdatesSilent()
        {
            backgroundWorker.RunWorkerAsync();
        }

        private void BtnCheckUpdates_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Update check is already in progress.", "Please Wait", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(0, "Checking for updates...");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string currentVersion = GetApplicationVersion();
            string latestVersion = null;

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Proxy = WebRequest.GetSystemWebProxy();
                    webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    string xmlContent = webClient.DownloadString(VERSION_FILE_URL);
                    XDocument doc = XDocument.Parse(xmlContent);
                    latestVersion = doc.Root?.Element("Version")?.Value?.Trim();
                }
            }
            catch
            {
                worker.ReportProgress(0, "Could not check for updates. Check your internet connection.");
                e.Result = null;
                return;
            }

            if (string.IsNullOrEmpty(latestVersion))
            {
                worker.ReportProgress(0, "Could not check for updates.");
                e.Result = null;
                return;
            }

            if (!IsNewerVersion(latestVersion, currentVersion))
            {
                worker.ReportProgress(0, $"You have the latest version ({currentVersion}).");
                e.Result = null;
                return;
            }

            worker.ReportProgress(50, $"Update v{latestVersion} available (current: v{currentVersion})");
            e.Result = new UpdateInfo { LatestVersion = latestVersion, CurrentVersion = currentVersion };
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblUpdateStatus.Text = e.UserState?.ToString() ?? "";
            lblUpdateStatus.ForeColor = System.Drawing.Color.Blue;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnCheckUpdates.Enabled = true;

            if (e.Result is UpdateInfo info)
            {
                DialogResult result = MessageBox.Show(
                    $"A new version (v{info.LatestVersion}) is available.\nCurrent version: v{info.CurrentVersion}\n\nDownload and install now?",
                    "Update Available",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    DownloadAndInstallUpdate(info.LatestVersion);
                }
            }
        }

        private string GetApplicationVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        private bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            try
            {
                return new Version(latestVersion) > new Version(currentVersion);
            }
            catch
            {
                return false;
            }
        }

        private void DownloadAndInstallUpdate(string version)
        {
            btnCheckUpdates.Enabled = false;
            updateProgress.Visible = true;
            updateProgress.Value = 0;

            try
            {
                string updateUrl = string.Format(UPDATE_FILE_URL, version);
                string tempPath = Path.Combine(Path.GetTempPath(), "CustomerManager_Setup.exe");

                using (WebClient webClient = new WebClient())
                {
                    webClient.Proxy = WebRequest.GetSystemWebProxy();
                    webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        updateProgress.Value = e.ProgressPercentage;
                        lblUpdateStatus.Text = $"Downloading update... {e.ProgressPercentage}% ({FormatBytes(e.BytesReceived)} / {FormatBytes(e.TotalBytesToReceive)})";
                    };

                    lblUpdateStatus.ForeColor = System.Drawing.Color.Blue;
                    webClient.DownloadFileAsync(new Uri(updateUrl), tempPath);

                    while (webClient.IsBusy)
                    {
                        Application.DoEvents();
                        Thread.Sleep(50);
                    }
                }

                lblUpdateStatus.Text = "Download complete! Launching installer...";
                lblUpdateStatus.ForeColor = System.Drawing.Color.Green;
                Application.DoEvents();
                Thread.Sleep(500);

                Process.Start(tempPath);
                this.Close();
                Application.Exit();
            }
            catch (Exception ex)
            {
                lblUpdateStatus.Text = $"Download failed: {ex.Message}";
                lblUpdateStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Update download failed: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                updateProgress.Visible = false;
            }
            finally
            {
                if (this.Visible)
                {
                    btnCheckUpdates.Enabled = true;
                }
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < suffixes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {suffixes[order]}";
        }

        private class UpdateInfo
        {
            public string LatestVersion { get; set; }
            public string CurrentVersion { get; set; }
        }
    }
}