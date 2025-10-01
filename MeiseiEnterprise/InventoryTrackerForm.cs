using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MeiseiEnterprise
{
    public partial class InventoryTrackerForm : Form
    {
        private DataGridView _inventoryGrid;
        private List<InventoryItem> _items;

        public InventoryTrackerForm()
        {
            InitializeComponent();
            Program.ApplyClassicTheme(this);
            LoadInventoryData();
            AuditLogger.LogAction("INVENTORY_MODULE_OPENED", "Inventory Tracker module initialized");
        }

        private void InitializeComponent()
        {
            this.Text = "Meisei Inventory Tracker - Office Equipment Database";
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(212, 208, 200);

            var titleLabel = new Label
            {
                Text = "CORPORATE INVENTORY MANAGEMENT SYSTEM",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, 15),
                Size = new Size(850, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var searchLabel = new Label
            {
                Text = "Search:",
                Location = new Point(20, 50),
                Size = new Size(60, 20)
            };

            var searchBox = new TextBox
            {
                Location = new Point(85, 48),
                Size = new Size(300, 20)
            };

            var searchButton = new Button
            {
                Text = "Search",
                Location = new Point(395, 47),
                Size = new Size(80, 23)
            };
            searchButton.Click += (s, e) => Search_Click(searchBox.Text);

            _inventoryGrid = new DataGridView
            {
                Location = new Point(20, 130),
                Size = new Size(850, 380),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var addButton = new Button
            {
                Text = "Add Item",
                Location = new Point(20, 85),
                Size = new Size(100, 30)
            };
            addButton.Click += AddItem_Click;

            var editButton = new Button
            {
                Text = "Edit Item",
                Location = new Point(130, 85),
                Size = new Size(100, 30)
            };
            editButton.Click += EditItem_Click;

            var deleteButton = new Button
            {
                Text = "Remove Item",
                Location = new Point(240, 85),
                Size = new Size(100, 30)
            };
            deleteButton.Click += DeleteItem_Click;

            var transferButton = new Button
            {
                Text = "Transfer Item",
                Location = new Point(350, 85),
                Size = new Size(100, 30)
            };
            transferButton.Click += TransferItem_Click;

            var viewDetailsButton = new Button
            {
                Text = "View Details",
                Location = new Point(460, 85),
                Size = new Size(100, 30)
            };
            viewDetailsButton.Click += ViewDetails_Click;

            var refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(570, 85),
                Size = new Size(100, 30)
            };
            refreshButton.Click += Refresh_Click;

            var statusLabel = new Label
            {
                Text = $"Total Items: {_items?.Count ?? 0} | Last Updated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                Location = new Point(20, 520),
                Size = new Size(850, 20),
                ForeColor = Color.Gray
            };

            this.Controls.AddRange(new Control[] 
            { 
                titleLabel, searchLabel, searchBox, searchButton, 
                addButton, editButton, deleteButton, transferButton, viewDetailsButton, refreshButton,
                _inventoryGrid, statusLabel 
            });
        }

        private void LoadInventoryData()
        {
            _items = new List<InventoryItem>
            {
                new InventoryItem { ID = "IT-001", Name = "Dell Latitude 5400", Category = "Computer", Location = "Floor 3 - Cubicle 3A-47", Status = "In Use", AssignedTo = "Smith, John", PurchaseDate = new DateTime(2018, 5, 15) },
                new InventoryItem { ID = "IT-002", Name = "HP LaserJet Pro M404n", Category = "Printer", Location = "Floor 3 - Print Room", Status = "In Use", AssignedTo = "Shared Resource", PurchaseDate = new DateTime(2019, 8, 22) },
                new InventoryItem { ID = "IT-003", Name = "Polycom Conference Phone", Category = "Communication", Location = "Conf Room A-301", Status = "In Use", AssignedTo = "Meeting Room", PurchaseDate = new DateTime(2020, 1, 10) },
                new InventoryItem { ID = "FN-001", Name = "Ergonomic Office Chair", Category = "Furniture", Location = "Floor 2 - Cubicle 2C-89", Status = "In Use", AssignedTo = "Johnson, Sarah", PurchaseDate = new DateTime(2017, 3, 5) },
                new InventoryItem { ID = "IT-004", Name = "Logitech Wireless Mouse M325", Category = "Peripheral", Location = "Storage Room B", Status = "Available", AssignedTo = "Unassigned", PurchaseDate = new DateTime(2021, 6, 18) },
                new InventoryItem { ID = "IT-005", Name = "Dell 24\" LED Monitor P2419H", Category = "Display", Location = "Floor 4 - Cubicle 4A-12", Status = "Repair Pending", AssignedTo = "Chen, Michael", PurchaseDate = new DateTime(2019, 11, 30) },
                new InventoryItem { ID = "IT-006", Name = "Lenovo ThinkPad T480", Category = "Computer", Location = "Floor 2 - Cubicle 2D-55", Status = "In Use", AssignedTo = "Williams, Alice", PurchaseDate = new DateTime(2019, 2, 8) },
                new InventoryItem { ID = "IT-007", Name = "Cisco IP Phone 7821", Category = "Communication", Location = "Floor 5 - Office 5B", Status = "In Use", AssignedTo = "Brown, Robert", PurchaseDate = new DateTime(2020, 9, 14) },
                new InventoryItem { ID = "IT-008", Name = "Canon imageCLASS MF445dw", Category = "Printer", Location = "Floor 2 - Copy Room", Status = "In Use", AssignedTo = "Shared Resource", PurchaseDate = new DateTime(2021, 3, 27) },
                new InventoryItem { ID = "IT-009", Name = "HP EliteDesk 800 G5", Category = "Computer", Location = "Floor 1 - Reception", Status = "In Use", AssignedTo = "Davis, Jennifer", PurchaseDate = new DateTime(2020, 7, 19) },
                new InventoryItem { ID = "IT-010", Name = "USB Keyboard - Standard", Category = "Peripheral", Location = "Storage Room B", Status = "Available", AssignedTo = "Unassigned", PurchaseDate = new DateTime(2018, 10, 5) },
                new InventoryItem { ID = "FN-002", Name = "Standing Desk Converter", Category = "Furniture", Location = "Floor 4 - Cubicle 4C-78", Status = "In Use", AssignedTo = "Miller, James", PurchaseDate = new DateTime(2021, 1, 22) },
                new InventoryItem { ID = "IT-011", Name = "Webcam Logitech C920", Category = "Peripheral", Location = "Conf Room B-205", Status = "In Use", AssignedTo = "Meeting Room", PurchaseDate = new DateTime(2020, 4, 11) },
                new InventoryItem { ID = "IT-012", Name = "Shredder - Heavy Duty", Category = "Office Equipment", Location = "Floor 3 - Mail Room", Status = "Maintenance", AssignedTo = "Facilities", PurchaseDate = new DateTime(2017, 8, 3) },
                new InventoryItem { ID = "FN-003", Name = "Filing Cabinet 4-Drawer", Category = "Furniture", Location = "Floor 2 - Storage", Status = "In Use", AssignedTo = "HR Department", PurchaseDate = new DateTime(2016, 5, 29) },
                new InventoryItem { ID = "IT-013", Name = "Scanner Epson ES-50", Category = "Peripheral", Location = "Floor 4 - Admin Office", Status = "In Use", AssignedTo = "Martinez, Lisa", PurchaseDate = new DateTime(2019, 12, 7) },
                new InventoryItem { ID = "IT-014", Name = "External Hard Drive 2TB", Category = "Storage", Location = "IT Department", Status = "In Use", AssignedTo = "IT Backup System", PurchaseDate = new DateTime(2021, 8, 16) },
                new InventoryItem { ID = "IT-015", Name = "Projector Epson PowerLite", Category = "Presentation", Location = "Conf Room C-410", Status = "In Use", AssignedTo = "Meeting Room", PurchaseDate = new DateTime(2018, 11, 25) },
                new InventoryItem { ID = "IT-016", Name = "Label Printer Dymo", Category = "Peripheral", Location = "Floor 1 - Mail Room", Status = "In Use", AssignedTo = "Mail Services", PurchaseDate = new DateTime(2020, 2, 14) },
                new InventoryItem { ID = "FN-004", Name = "Desk Lamp LED", Category = "Furniture", Location = "Floor 3 - Cubicle 3B-23", Status = "In Use", AssignedTo = "Anderson, Kevin", PurchaseDate = new DateTime(2019, 6, 8) },
                new InventoryItem { ID = "IT-017", Name = "Headset Plantronics", Category = "Peripheral", Location = "Storage Room B", Status = "Available", AssignedTo = "Unassigned", PurchaseDate = new DateTime(2021, 4, 3) },
                new InventoryItem { ID = "IT-018", Name = "Surge Protector 8-Outlet", Category = "Accessory", Location = "Floor 5 - Cubicle 5A-91", Status = "In Use", AssignedTo = "Taylor, Mark", PurchaseDate = new DateTime(2019, 9, 17) },
                new InventoryItem { ID = "IT-019", Name = "UPS Battery Backup", Category = "Power", Location = "Server Room", Status = "Active", AssignedTo = "IT Infrastructure", PurchaseDate = new DateTime(2020, 10, 28) },
                new InventoryItem { ID = "FN-005", Name = "Whiteboard 6ft x 4ft", Category = "Furniture", Location = "Conf Room D-312", Status = "In Use", AssignedTo = "Meeting Room", PurchaseDate = new DateTime(2018, 3, 12) },
                new InventoryItem { ID = "IT-020", Name = "Calculator Texas Instruments", Category = "Office Equipment", Location = "Floor 4 - Finance Dept", Status = "In Use", AssignedTo = "Accounting Pool", PurchaseDate = new DateTime(2017, 7, 21) },
            };

            _inventoryGrid.DataSource = _items;
        }

        private void Search_Click(string query)
        {
            AuditLogger.LogClick("Search Button");
            AuditLogger.LogAction("INVENTORY_SEARCH", $"Query: {query}");

            ConfirmationHelper.ShowProcessingDialog("Connecting to inventory database...", 1200);
            ConfirmationHelper.ShowProcessingDialog("Executing search query...", 1500);
            ConfirmationHelper.ShowProcessingDialog("Filtering results...", 1000);
            ConfirmationHelper.ShowProcessingDialog("Sorting data...", 800);
            ConfirmationHelper.ShowProcessingDialog("Applying permissions...", 900);

            MessageBox.Show($"Search completed.\n\nQuery: '{query}'\nResults: {_items.Count} items found\n\nShowing all items (search filter not applied for demonstration).",
                "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddItem_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Add Item Button");

            MessageBox.Show("Adding new inventory items requires approval from:\n\n" +
                           "1. Department Manager\n2. Inventory Control Manager\n\n" +
                           "Please proceed to approval process.",
                "Manager Approval Required - Step 1 of 7", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!ConfirmationHelper.RequireAdminApproval("Add new inventory item"))
                return;

            if (!ConfirmationHelper.RequireAdminApproval("Secondary manager approval for inventory addition"))
                return;

            if (!ConfirmationHelper.ConfirmAction("add this item to the inventory database", 3))
                return;

            MessageBox.Show("Item addition requires the following forms:\n\n" +
                           "• Form INV-100: New Item Request\n" +
                           "• Form INV-101: Asset Categorization\n" +
                           "• Form INV-102: Location Assignment\n" +
                           "• Form FIN-50: Budget Approval\n" +
                           "• Form IT-25: Asset Tag Request\n\n" +
                           "Please submit all forms to the Inventory Department.",
                "Additional Documentation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void EditItem_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Edit Item Button");

            if (_inventoryGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to edit.\n\nSelection is required to proceed.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmationHelper.ConfirmAction("edit this inventory item", 4))
                return;

            if (!ConfirmationHelper.RequireAdminApproval("Modify inventory record"))
                return;

            MessageBox.Show("Inventory modifications require:\n\n" +
                           "• Written justification\n" +
                           "• Manager signature\n" +
                           "• IT department notification\n" +
                           "• Audit trail documentation\n\n" +
                           "Processing time: 5-7 business days",
                "Edit Request Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteItem_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Remove Item Button");

            if (_inventoryGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("INVENTORY REMOVAL POLICY\n\n" +
                           "Items cannot be removed from the database.\n\n" +
                           "Items can only be marked as:\n" +
                           "• Disposed\n" +
                           "• Retired\n" +
                           "• Lost\n" +
                           "• Stolen\n\n" +
                           "All status changes require:\n" +
                           "• Police report (if stolen)\n" +
                           "• Disposal certificate (if disposed)\n" +
                           "• Manager approval\n" +
                           "• Asset recovery investigation\n\n" +
                           "Please contact Asset Management Department.",
                "Removal Not Permitted", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void TransferItem_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Transfer Item Button");

            if (_inventoryGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to transfer.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ConfirmationHelper.ConfirmAction("transfer this item to another location", 3))
                return;

            MessageBox.Show("Item transfer process:\n\n" +
                           "Step 1: Submit transfer request form (INV-200)\n" +
                           "Step 2: Obtain current location manager approval\n" +
                           "Step 3: Obtain destination location manager approval\n" +
                           "Step 4: Schedule transfer date (2 weeks notice required)\n" +
                           "Step 5: Coordinate with facilities department\n" +
                           "Step 6: Update insurance records\n" +
                           "Step 7: Complete transfer confirmation form\n\n" +
                           "Estimated processing time: 3-4 weeks",
                "Transfer Process Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ViewDetails_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("View Details Button");

            if (_inventoryGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to view details.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConfirmationHelper.ShowProcessingDialog("Loading item details...", 1500);

            var item = _inventoryGrid.SelectedRows[0].DataBoundItem as InventoryItem;
            if (item != null)
            {
                MessageBox.Show($"ITEM DETAILS\n\n" +
                               $"ID: {item.ID}\n" +
                               $"Name: {item.Name}\n" +
                               $"Category: {item.Category}\n" +
                               $"Location: {item.Location}\n" +
                               $"Status: {item.Status}\n" +
                               $"Assigned To: {item.AssignedTo}\n" +
                               $"Purchase Date: {item.PurchaseDate:yyyy-MM-dd}\n" +
                               $"Age: {(DateTime.Now - item.PurchaseDate).Days} days\n\n" +
                               $"Last Audited: {DateTime.Now.AddDays(-45):yyyy-MM-dd}\n" +
                               $"Next Audit Due: {DateTime.Now.AddDays(15):yyyy-MM-dd}\n" +
                               $"Depreciation: 35%\n" +
                               $"Condition: Fair\n" +
                               $"Insurance Status: Active\n" +
                               $"Warranty: Expired",
                    "Item Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Refresh_Click(object? sender, EventArgs e)
        {
            AuditLogger.LogClick("Refresh Button");
            
            ConfirmationHelper.ShowProcessingDialog("Connecting to corporate server...", 1200);
            ConfirmationHelper.ShowProcessingDialog("Synchronizing inventory data...", 1500);
            ConfirmationHelper.ShowProcessingDialog("Verifying checksums...", 800);
            ConfirmationHelper.ShowProcessingDialog("Updating local cache...", 1000);

            _inventoryGrid.DataSource = null;
            _inventoryGrid.DataSource = _items;

            MessageBox.Show($"Inventory database refreshed.\n\n" +
                           $"Total items: {_items.Count}\n" +
                           $"Last sync: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                           $"Server status: Online\n" +
                           $"Database version: 1.0.0.0",
                "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private class InventoryItem
        {
            public string ID { get; set; } = "";
            public string Name { get; set; } = "";
            public string Category { get; set; } = "";
            public string Location { get; set; } = "";
            public string Status { get; set; } = "";
            public string AssignedTo { get; set; } = "";
            public DateTime PurchaseDate { get; set; }
        }
    }
}
