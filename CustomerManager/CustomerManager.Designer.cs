using System.Windows.Forms;

namespace CustomerManager
{
    partial class CustomerManagerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Text = "Customer Manager CRUD Application";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // Create tab control for organization
            TabControl tabControl = new TabControl();
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;

            // Tab 1: CRUD Operations
            TabPage crudTab = new TabPage("Manage Customers");
            crudTab.BackColor = System.Drawing.SystemColors.Control;

            // Label for ID
            Label lblId = new Label() { Text = "Customer ID:", Left = 20, Top = 20, Width = 100 };
            TextBox txtId = new TextBox() { Left = 130, Top = 20, Width = 150, ReadOnly = true };
            this.txtId = txtId;

            // Label for Name
            Label lblName = new Label() { Text = "Name:", Left = 20, Top = 50, Width = 100 };
            TextBox txtName = new TextBox() { Left = 130, Top = 50, Width = 150 };
            this.txtName = txtName;

            // Label for Email
            Label lblEmail = new Label() { Text = "Email:", Left = 20, Top = 80, Width = 100 };
            TextBox txtEmail = new TextBox() { Left = 130, Top = 80, Width = 150 };
            this.txtEmail = txtEmail;

            // Label for Phone
            Label lblPhone = new Label() { Text = "Phone:", Left = 20, Top = 110, Width = 100 };
            TextBox txtPhone = new TextBox() { Left = 130, Top = 110, Width = 150 };
            this.txtPhone = txtPhone;

            // Label for Address
            Label lblAddress = new Label() { Text = "Address:", Left = 20, Top = 140, Width = 100 };
            TextBox txtAddress = new TextBox() { Left = 130, Top = 140, Width = 150, Multiline = true, Height = 60 };
            this.txtAddress = txtAddress;

            // Buttons
            Button btnAdd = new Button() { Text = "Add", Left = 20, Top = 220, Width = 80 };
            btnAdd.Click += BtnAdd_Click;
            this.btnAdd = btnAdd;

            Button btnUpdate = new Button() { Text = "Update", Left = 110, Top = 220, Width = 80 };
            btnUpdate.Click += BtnUpdate_Click;
            this.btnUpdate = btnUpdate;

            Button btnDelete = new Button() { Text = "Delete", Left = 200, Top = 220, Width = 80 };
            btnDelete.Click += BtnDelete_Click;
            this.btnDelete = btnDelete;

            Button btnClear = new Button() { Text = "Clear", Left = 290, Top = 220, Width = 80 };
            btnClear.Click += BtnClear_Click;
            this.btnClear = btnClear;

            // DataGridView for displaying data
            DataGridView dgvCustomers = new DataGridView()
            {
                Left = 20,
                Top = 270,
                Width = 860,
                Height = 310,
                AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
            };
            dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;
            this.dgvCustomers = dgvCustomers;

            // Add controls to CRUD tab
            crudTab.Controls.Add(lblId);
            crudTab.Controls.Add(txtId);
            crudTab.Controls.Add(lblName);
            crudTab.Controls.Add(txtName);
            crudTab.Controls.Add(lblEmail);
            crudTab.Controls.Add(txtEmail);
            crudTab.Controls.Add(lblPhone);
            crudTab.Controls.Add(txtPhone);
            crudTab.Controls.Add(lblAddress);
            crudTab.Controls.Add(txtAddress);
            crudTab.Controls.Add(btnAdd);
            crudTab.Controls.Add(btnUpdate);
            crudTab.Controls.Add(btnDelete);
            crudTab.Controls.Add(btnClear);
            crudTab.Controls.Add(dgvCustomers);

            // Tab 2: About & Update Info
            TabPage aboutTab = new TabPage("About & Updates");
            aboutTab.BackColor = System.Drawing.SystemColors.Control;

            Label lblAppName = new Label() { Text = "Customer Manager v1.0.0", Left = 20, Top = 20, Width = 400, Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold) };
            Label lblDescription = new Label() { Text = "A simple CRUD application for managing customer information", Left = 20, Top = 60, Width = 400 };
            Label lblCurrentVersion = new Label() { Text = "Current Version: 1.0.0", Left = 20, Top = 100, Width = 400 };
            Label lblLastUpdate = new Label() { Text = "Last Updated: " + System.DateTime.Now.ToString(), Left = 20, Top = 140, Width = 400 };

            Button btnCheckUpdates = new Button() { Text = "Check for Updates", Left = 20, Top = 180, Width = 150 };
            btnCheckUpdates.Click += BtnCheckUpdates_Click;
            this.btnCheckUpdates = btnCheckUpdates;

            Label lblUpdateStatus = new Label() { Text = "", Left = 20, Top = 220, Width = 400 };
            this.lblUpdateStatus = lblUpdateStatus;

            aboutTab.Controls.Add(lblAppName);
            aboutTab.Controls.Add(lblDescription);
            aboutTab.Controls.Add(lblCurrentVersion);
            aboutTab.Controls.Add(lblLastUpdate);
            aboutTab.Controls.Add(btnCheckUpdates);
            aboutTab.Controls.Add(lblUpdateStatus);

            // Add tabs to tab control
            tabControl.TabPages.Add(crudTab);
            tabControl.TabPages.Add(aboutTab);

            // Add tab control to form
            this.Controls.Add(tabControl);
        }

        // Control declarations
        private TextBox txtId;
        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Button btnCheckUpdates;
        private DataGridView dgvCustomers;
        private Label lblUpdateStatus;
    }
}