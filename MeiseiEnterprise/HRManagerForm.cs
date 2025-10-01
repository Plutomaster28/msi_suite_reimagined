using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MeiseiEnterprise
{
    public partial class HRManagerForm : Form
    {
        private DataGridView _employeeGrid;
        private TabControl _tabControl;
        private List<Employee> _employees;

        public HRManagerForm()
        {
            InitializeComponent();
            Program.ApplyClassicTheme(this);
            LoadSampleData();
            AuditLogger.LogAction("HR_MANAGER_OPENED", "HR Manager module initialized");
        }

        private void InitializeComponent()
        {
            this.Text = "Meisei HR Manager - Employee Database";
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(212, 208, 200);

            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(212, 208, 200)
            };

            var employeeTab = new TabPage("Employee Database");
            var attendanceTab = new TabPage("Attendance Tracker");
            var performanceTab = new TabPage("Performance Reports");

            CreateEmployeeDatabaseTab(employeeTab);
            CreateAttendanceTab(attendanceTab);
            CreatePerformanceTab(performanceTab);

            _tabControl.TabPages.Add(employeeTab);
            _tabControl.TabPages.Add(attendanceTab);
            _tabControl.TabPages.Add(performanceTab);

            this.Controls.Add(_tabControl);
        }

        private void CreateEmployeeDatabaseTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(212, 208, 200) };

            _employeeGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(212, 208, 200)
            };

            var addButton = new Button
            {
                Text = "Add Employee",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.System
            };
            addButton.Click += (s, e) => AddEmployee();

            var editButton = new Button
            {
                Text = "Edit Employee",
                Location = new Point(140, 10),
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.System
            };
            editButton.Click += (s, e) => EditEmployee();

            var deleteButton = new Button
            {
                Text = "Delete Employee",
                Location = new Point(270, 10),
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.System
            };
            deleteButton.Click += (s, e) => DeleteEmployee();

            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(deleteButton);

            panel.Controls.Add(_employeeGrid);
            panel.Controls.Add(buttonPanel);
            tab.Controls.Add(panel);
        }

        private void CreateAttendanceTab(TabPage tab)
        {
            var label = new Label
            {
                Text = "Attendance Tracking Module\n\n" +
                       "Employee Check-in/Check-out Times\n" +
                       "Vacation Days Remaining\n" +
                       "Sick Leave Records\n\n" +
                       "[Requires 3 confirmations to view detailed attendance]",
                Font = new Font("Arial", 10),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(212, 208, 200)
            };

            var viewButton = new Button
            {
                Text = "View Attendance Details",
                Size = new Size(200, 40),
                Location = new Point(350, 250),
                FlatStyle = FlatStyle.System
            };
            viewButton.Click += (s, e) =>
            {
                if (ConfirmationHelper.ConfirmAction("view attendance details", 3))
                {
                    MessageBox.Show("Attendance data loaded.\n\nAverage attendance: 97.3%\nLate arrivals this month: 12",
                        "Attendance Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            tab.Controls.Add(label);
            tab.Controls.Add(viewButton);
        }

        private void CreatePerformanceTab(TabPage tab)
        {
            var label = new Label
            {
                Text = "Performance Review System\n\n" +
                       "Quarterly Performance Metrics\n" +
                       "Goal Tracking\n" +
                       "Manager Feedback\n\n" +
                       "[Graphs available after 4 confirmation dialogs]",
                Font = new Font("Arial", 10),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(212, 208, 200)
            };

            tab.Controls.Add(label);
        }

        private void LoadSampleData()
        {
            _employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "John Smith", Department = "IT", Position = "Developer", Salary = 75000 },
                new Employee { Id = 2, Name = "Jane Doe", Department = "HR", Position = "Manager", Salary = 85000 },
                new Employee { Id = 3, Name = "Bob Johnson", Department = "Sales", Position = "Representative", Salary = 60000 },
                new Employee { Id = 4, Name = "Alice Williams", Department = "IT", Position = "Senior Developer", Salary = 95000 },
                new Employee { Id = 5, Name = "Charlie Brown", Department = "Marketing", Position = "Coordinator", Salary = 55000 }
            };

            _employeeGrid.DataSource = new BindingSource { DataSource = _employees };
        }

        private void AddEmployee()
        {
            AuditLogger.LogClick("Add Employee Button");
            if (!ConfirmationHelper.ConfirmAction("add a new employee", 5))
                return;

            MessageBox.Show("Adding employee requires:\n1. Manager approval\n2. HR approval\n3. IT access setup\n4. Payroll configuration\n5. Security clearance",
                "Add Employee", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditEmployee()
        {
            AuditLogger.LogClick("Edit Employee Button");
            if (_employeeGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmationHelper.ConfirmAction("edit employee information", 6))
                return;

            MessageBox.Show("Employee data updated.\n\nAll changes have been logged to the audit system for compliance purposes.",
                "Edit Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteEmployee()
        {
            AuditLogger.LogClick("Delete Employee Button");
            if (_employeeGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmationHelper.ConfirmAction("delete this employee", 8))
                return;

            MessageBox.Show("Employee deletion requires:\n1. Exit interview\n2. Equipment return\n3. Access revocation\n4. Final paycheck processing\n5. Documentation archival",
                "Delete Employee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Department { get; set; } = "";
            public string Position { get; set; } = "";
            public decimal Salary { get; set; }
        }
    }
}
