using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.IO;

namespace AppQuanLyV1
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private Customer _selectedCustomer;
        private Account _selectedAccount;
        private string _originalAccountEmail; // To store the original email for updates
        private List<Customer> _allCustomers; // To store the full list of customers
        private List<Account> _allAccounts; // To store the full list of accounts
        private DateTime _financialStartDate;
        private DateTime _financialEndDate;
        private bool _isUIInitialized = false;

        public MainWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            
            // Configure the customers data grid to be unclickable
            CustomersDataGrid.IsReadOnly = true;
            CustomersDataGrid.SelectionMode = DataGridSelectionMode.Single;
            CustomersDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            CustomersDataGrid.CanUserResizeRows = false;
            
            // Initialize filter combo box if it exists in the XAML
            if (FilterComboBox != null)
            {
                FilterComboBox.Items.Add("All");
                FilterComboBox.Items.Add("Active");
                FilterComboBox.Items.Add("Expired");
                FilterComboBox.Items.Add("Do Not Continue"); // Add the new filter option
                FilterComboBox.SelectedIndex = 0;
            }
            
            // Initialize account filter combo box
            if (AccountFilterComboBox != null)
            {
                AccountFilterComboBox.SelectedIndex = 0;
            }
            
            // Set default financial period (current month)
            _financialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _financialEndDate = _financialStartDate.AddMonths(1).AddDays(-1);
            
            // Fix existing data to ensure consistency with "Expired" marking
            _dbHelper.FixExistingNotContinuingEmails();
            
            // Use event to ensure UI is fully loaded before calculating financials
            this.Loaded += (s, e) => 
            {
                // Initialize date pickers for financial dashboard
                if (FromDatePicker != null) FromDatePicker.SelectedDate = _financialStartDate;
                if (ToDatePicker != null) ToDatePicker.SelectedDate = _financialEndDate;
                
                _isUIInitialized = true;
                LoadCustomers();
                LoadAccounts();
                CalculateFinancials(); // Only calculate after UI is loaded
            };
        }

        // Load customer data into DataGrid
        private void LoadCustomers()
        {
            _allCustomers = _dbHelper.GetAllCustomers();
            var accounts = _dbHelper.GetAllAccounts();
            
            // Mark customers with expired accounts
            foreach (var customer in _allCustomers)
            {
                if (!string.IsNullOrEmpty(customer.Note))
                {
                    var associatedAccount = accounts.FirstOrDefault(acc => acc.Email == customer.Note);
                    if (associatedAccount != null && associatedAccount.IsExpired)
                    {
                        customer.Status = "Account Expired";
                        customer.StatusColor = Brushes.Red;
                    }
                }
                
                // Check if customer subscription is expired
                if (customer.SubscriptionExpiry < DateTime.Today)
                {
                    customer.Status = "Subscription Expired";
                    customer.StatusColor = Brushes.Red;
                }
                
                // Mark the Not Continuing customers 
                if (!customer.ContinueSubscription)
                {
                    customer.Status = "Not Continuing";
                    customer.StatusColor = Brushes.Red;
                }
            }
            
            // Apply filter to show properly filtered list
            ApplySearchAndFilter();
            LoadExpiredCustomers();
        }

        // Apply search and filter to the customer list
        private void ApplySearchAndFilter()
        {
            if (_allCustomers == null) return;

            IEnumerable<Customer> filteredList = _allCustomers;

            // Apply filter if FilterComboBox exists and is set
            if (FilterComboBox != null && FilterComboBox.SelectedItem != null)
            {
                string filter = FilterComboBox.SelectedItem.ToString();
                switch (filter)
                {
                    case "Active":
                        filteredList = filteredList.Where(c => c.SubscriptionExpiry >= DateTime.Today && c.ContinueSubscription);
                        break;
                    case "Expired":
                        filteredList = filteredList.Where(c => c.SubscriptionExpiry < DateTime.Today);
                        break;
                    case "Do Not Continue":
                        filteredList = filteredList.Where(c => !c.ContinueSubscription);
                        break;
                    case "All":
                        // Explicitly show all customers without any filtering
                        // This ensures "Do Not Continue" customers are included
                        filteredList = _allCustomers;
                        break;
                    default:
                        // Default case - show all customers
                        filteredList = _allCustomers;
                        break;
                }
            }

            // Apply search text if SearchTextBox exists and has content
            if (SearchTextBox != null && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                filteredList = filteredList.Where(c => 
                    c.Name.ToLower().Contains(searchText) || 
                    (c.Note != null && c.Note.ToLower().Contains(searchText)) ||
                    (c.SubscriptionPackage != null && c.SubscriptionPackage.ToLower().Contains(searchText)));
            }

            // Update the data grid and show count information
            var filteredCount = filteredList.Count();
            var totalCount = _allCustomers.Count;
            var continuingCount = _allCustomers.Count(c => c.ContinueSubscription);
            var notContinuingCount = _allCustomers.Count(c => !c.ContinueSubscription);
            
            // Update the data grid
            CustomersDataGrid.ItemsSource = filteredList.ToList();
            
            // Update status bar if it exists
            if (AccountStatusTextBlock != null)
            {
                AccountStatusTextBlock.Text = $"Showing {filteredCount} of {totalCount} customers" +
                    $" (Continuing: {continuingCount}, Not Continuing: {notContinuingCount})";
            }
        }

        // Handle search text changed
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchAndFilter();
        }

        // Handle filter selection changed
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySearchAndFilter();
        }

        // Disable selection changed event to make the list "unclickable"
        private void CustomersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear selection to make it appear unclickable
            CustomersDataGrid.UnselectAll();
            e.Handled = true;
        }

        // This method is for selecting a customer when you need to (for edit/delete operations)
        // To be called from buttons, not from direct grid clicks
        private void SelectCustomer(Customer customer)
        {
            _selectedCustomer = customer;
        }
        
        // Load expired customers into the ListView
        private void LoadExpiredCustomers()
        {
            ExpiredCustomersListView.Items.Clear();
            var expiredCustomers = _dbHelper.GetExpiredCustomers();
            
            foreach (var customer in expiredCustomers)
            {
                var item = new ExpiredCustomerItem 
                { 
                    CustomerId = customer.Id,
                    DisplayText = $"{customer.Name} - Expired on {customer.SubscriptionExpiry:dd/MM/yyyy}",
                    ReminderText = $"Chào bạn {customer.Name}, gói ChatGPT Plus của bạn đã hết hạn vào {customer.SubscriptionExpiry:dd/MM/yyyy}. Bạn có muốn gia hạn không ha (0,0?!)",
                    IsContinuing = customer.ContinueSubscription
                };

                // Set appearance based on continuation status
                if (!customer.ContinueSubscription)
                {
                    item.DisplayText += " (Not Continuing)";
                    item.TextColor = Brushes.Gray;
                    item.MoneyPayVisibility = Visibility.Collapsed;
                    item.DoNotContinueVisibility = Visibility.Collapsed;
                    item.RenewVisibility = Visibility.Visible; // Always show Renew button
                }
                else
                {
                    item.TextColor = Brushes.Red;
                    item.MoneyPayVisibility = Visibility.Visible;
                    item.DoNotContinueVisibility = Visibility.Visible;
                    item.RenewVisibility = Visibility.Visible;
                }

                ExpiredCustomersListView.Items.Add(item);
            }
        }

        // Handle the Money-pay-text button click
        private void MoneyPayTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int customerId)
            {
                // Find the customer item in the ListView
                foreach (ExpiredCustomerItem item in ExpiredCustomersListView.Items)
                {
                    if (item.CustomerId == customerId)
                    {
                        // Copy reminder text to clipboard
                        Clipboard.SetText(item.ReminderText);
                        
                        // Show confirmation
                        MessageBox.Show("Payment reminder text copied to clipboard!", "Text Copied", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                }
            }
        }

        // Handle the "Do Not Continue" button click
        private void DoNotContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int customerId)
            {
                var result = MessageBox.Show(
                    "Are you sure this customer does not want to continue their subscription?\nThis will mark their account as \"Expired\".",
                    "Confirm Discontinuation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Get the customer first to see if we need to update account counts
                    var customer = _dbHelper.GetCustomerById(customerId);
                    string originalEmail = customer?.Note; // Store original email for account update
                    
                    // Mark the customer as not continuing AND set their email to "Expired"
                    _dbHelper.MarkCustomerAsNotContinuingAndClearEmail(customerId);
                    
                    // Always reload both customers and accounts
                    LoadCustomers();
                    LoadAccounts();
                    
                    MessageBox.Show(
                        "Customer marked as not continuing subscription and account marked as expired.",
                        "Status Updated",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        // Handle the Renew button click
        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int customerId)
            {
                // Find the customer in the database
                Customer customerToRenew = null;
                var expiredCustomers = _dbHelper.GetExpiredCustomers();
                foreach (var customer in expiredCustomers)
                {
                    if (customer.Id == customerId)
                    {
                        customerToRenew = customer;
                        break;
                    }
                }

                if (customerToRenew != null)
                {
                    // Open the renewal window
                    var renewWindow = new RenewSubscriptionWindow(customerToRenew);
                    renewWindow.Owner = this;
                    renewWindow.ShowDialog();
                    
                    // Refresh data if renewal was completed
                    if (renewWindow.RenewalCompleted)
                    {
                        LoadCustomers();
                        LoadAccounts();
                        MessageBox.Show($"Subscription renewed for {customerToRenew.Name}.", 
                                      "Renewal Complete", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Information);
                    }
                }
            }
        }

        // Load accounts data into AccountsDataGrid
        private void LoadAccounts()
        {
            _allAccounts = _dbHelper.GetAllAccounts();
            
            // Update the customer counts for each account based on continuing customers
            var continuingCustomers = _dbHelper.GetAllCustomers()
                .Where(c => c.ContinueSubscription && c.Note != "Expired") // Don't count "Expired" entries
                .ToList();
                
            foreach (var account in _allAccounts)
            {
                // For each account, count how many continuing customers use it
                // Skip counting for "Expired" accounts
                if (account.Email == "Expired")
                {
                    account.CustomerCount = 0;
                }
                else
                {
                    account.CustomerCount = continuingCustomers.Count(c => c.Note == account.Email);
                }
            }
            
            ApplyAccountFilter();
            ClearAccountFields();
        }

        // Apply filter to accounts list
        private void ApplyAccountFilter()
        {
            if (_allAccounts == null) return;

            var filteredAccounts = _allAccounts;

            // First, get all customers who have not been marked as "Do Not Continue"
            var continuingCustomers = _dbHelper.GetAllCustomers()
                .Where(c => c.ContinueSubscription && c.Note != "Expired") // Only consider customers who wish to continue and not marked as expired
                .ToList();

            // Apply filter if AccountFilterComboBox exists and AccountFilterComboBox.SelectedItem is ComboBoxItem selectedItem
            if (AccountFilterComboBox != null && AccountFilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string filter = selectedItem.Content.ToString();
                var today = DateTime.Today;
                
                switch (filter)
                {
                    case "Active":
                        // Get only accounts that:
                        // 1. Have an expiration date in the future
                        // 2. Have at least one CONTINUING customer
                        // 3. Have customers with active subscriptions
                        // 4. Exclude accounts marked with "Expired" emails
                        
                        // Get accounts that have active CONTINUING customers
                        var activeCustomers = continuingCustomers
                            .Where(c => c.SubscriptionExpiry >= today) // Active customers
                            .Select(c => c.Note) // Get account emails
                            .Where(email => !string.IsNullOrEmpty(email) && email != "Expired") // Remove empty emails and "Expired"
                            .Distinct() // Get unique emails
                            .ToList();
                        
                        // Filter accounts that are active themselves AND have active continuing customers
                        filteredAccounts = filteredAccounts.Where(a => 
                            (a.ExpireDate > today || a.ExpireDate == DateTime.MinValue) && 
                            activeCustomers.Contains(a.Email) &&
                            a.Email != "Expired" // Exclude "Expired" accounts
                        ).ToList();
                        
                        // Update status message
                        int totalAccounts = _allAccounts.Count;
                        int nonExpiredAccounts = _allAccounts.Count(a => a.Email != "Expired");
                        AccountStatusTextBlock.Text = $"Showing {filteredAccounts.Count} active accounts out of {nonExpiredAccounts} valid accounts";
                        break;
                    
                    default: // "All" case
                        // For "All", show both active and inactive, but display counts
                        // Count active accounts as those with continuing customers and valid expiration
                        var accountsWithContinuingCustomers = continuingCustomers
                            .Select(c => c.Note) // Get account emails
                            .Where(email => !string.IsNullOrEmpty(email) && email != "Expired") // Remove empty emails and "Expired"
                            .Distinct() // Get unique emails
                            .ToList();
                        
                        var activeAccounts = _allAccounts
                            .Where(a => 
                                (a.ExpireDate > today || a.ExpireDate == DateTime.MinValue) && 
                                accountsWithContinuingCustomers.Contains(a.Email) &&
                                a.Email != "Expired"
                            ).Count();
                        
                        var expiredAccounts = _allAccounts.Count(a => a.Email == "Expired");
                        var inactiveAccounts = _allAccounts.Count - activeAccounts - expiredAccounts;
                        
                        AccountStatusTextBlock.Text = $"Showing all {_allAccounts.Count} accounts ({activeAccounts} active, {inactiveAccounts} inactive, {expiredAccounts} expired)";
                        break;
                }
            }

            // Recalculate customer counts based on continuing customers only
            foreach (var account in filteredAccounts)
            {
                if (account.Email == "Expired")
                {
                    account.CustomerCount = 0; // Expired accounts have no customers
                    continue;
                }
                
                var continuingCustomerCount = continuingCustomers
                    .Count(c => c.Note == account.Email);
                
                account.CustomerCount = continuingCustomerCount;
            }

            AccountsDataGrid.ItemsSource = filteredAccounts;
        }

        // Handle account filter selection changed
        private void AccountFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAccountFilter();
        }

        private void ClearAccountFields()
        {
            AccountEmailTextBox.Text = "";
            AccountCustomerCountTextBox.Text = "";
            _selectedAccount = null;
            _originalAccountEmail = null;
            
            // Leave the status message as it is since it shows the filter information
            if (string.IsNullOrEmpty(AccountStatusTextBlock.Text) || 
                !AccountStatusTextBlock.Text.StartsWith("Showing"))
            {
                AccountStatusTextBlock.Text = "Ready";
            }
        }

        // Update account panel when account is selected
        private void AccountsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountsDataGrid.SelectedItem is Account selectedAccount)
            {
                _selectedAccount = selectedAccount;
                _originalAccountEmail = selectedAccount.Email;
                
                // Populate the fields
                AccountEmailTextBox.Text = selectedAccount.Email;
                AccountCustomerCountTextBox.Text = selectedAccount.CustomerCount.ToString();
                AccountStartDatePicker.SelectedDate = selectedAccount.StartDate != DateTime.MinValue ? selectedAccount.StartDate : (DateTime?)null;
                AccountExpirationDatePicker.SelectedDate = selectedAccount.ExpireDate != DateTime.MinValue ? selectedAccount.ExpireDate : (DateTime?)null;
                AccountStatusTextBlock.Text = $"Selected: {selectedAccount.Email}";
            }
        }

        // Open the customer editing window when the Edit button is clicked
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer != null)
            {
                // Create and show the edit window
                var editWindow = new EditCustomerWindow(_selectedCustomer);
                editWindow.Owner = this;
                editWindow.ShowDialog();

                // Refresh the customer list if changes were saved
                if (editWindow.ChangesSaved)
                {
                    LoadCustomers();
                    LoadAccounts(); // Also refresh accounts to reflect any changes to customer counts
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to edit.");
            }
        }

        // Save the edited customer details
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer != null)
            {
                // Update the customer details
                _selectedCustomer.Name = EditCustomerName.Text;
                _selectedCustomer.SubscriptionPackage = EditCustomerPackage.Text;
                _selectedCustomer.Note = EditCustomerEmail.Text;
                _selectedCustomer.RegisterDay = EditCustomerRegistrationDate.SelectedDate ?? _selectedCustomer.RegisterDay;
                _selectedCustomer.SubscriptionExpiry = EditCustomerExpirationDate.SelectedDate ?? _selectedCustomer.SubscriptionExpiry;

                // Save the updated customer details in the database
                _dbHelper.UpdateCustomer(_selectedCustomer);

                // Reload the customers list to reflect the changes
                LoadCustomers();

                // Hide the Edit tab after saving changes
                EditCustomerTab.Visibility = Visibility.Collapsed;
            }
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AccountEmailTextBox.Text))
                {
                    MessageBox.Show("Please enter an email address.");
                    return;
                }

                if (!int.TryParse(AccountCustomerCountTextBox.Text, out int customerCount))
                {
                    MessageBox.Show("Please enter a valid number for customer count.");
                    return;
                }

                var newAccount = new Account
                {
                    Email = AccountEmailTextBox.Text,
                    CustomerCount = customerCount,
                    StartDate = AccountStartDatePicker.SelectedDate ?? DateTime.MinValue,
                    ExpireDate = AccountExpirationDatePicker.SelectedDate ?? DateTime.MinValue
                };

                _dbHelper.AddAccountWithDates(newAccount);
                LoadAccounts();
                // Also reload customers to update the status
                LoadCustomers();
                AccountStatusTextBlock.Text = $"Account {newAccount.Email} added successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding account: {ex.Message}");
                AccountStatusTextBlock.Text = "Error adding account.";
            }
        }

        private void UpdateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedAccount == null || string.IsNullOrEmpty(_originalAccountEmail))
                {
                    MessageBox.Show("Please select an account to update.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(AccountEmailTextBox.Text))
                {
                    MessageBox.Show("Please enter an email address.");
                    return;
                }

                if (!int.TryParse(AccountCustomerCountTextBox.Text, out int customerCount))
                {
                    MessageBox.Show("Please enter a valid number for customer count.");
                    return;
                }

                var updatedAccount = new Account
                {
                    Email = AccountEmailTextBox.Text,
                    CustomerCount = customerCount,
                    StartDate = AccountStartDatePicker.SelectedDate ?? DateTime.MinValue,
                    ExpireDate = AccountExpirationDatePicker.SelectedDate ?? DateTime.MinValue
                };

                _dbHelper.UpdateAccountWithDates(updatedAccount, _originalAccountEmail);
                LoadAccounts();
                // Also reload customers to update the status of customers with this account
                LoadCustomers();
                AccountStatusTextBlock.Text = $"Account {updatedAccount.Email} updated successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating account: {ex.Message}");
                AccountStatusTextBlock.Text = "Error updating account.";
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedAccount == null || string.IsNullOrEmpty(_originalAccountEmail))
                {
                    MessageBox.Show("Please select an account to delete.");
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete the account {_originalAccountEmail}?", 
                                            "Confirm Deletion", 
                                            MessageBoxButton.YesNo, 
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _dbHelper.DeleteAccount(_originalAccountEmail);
                    LoadAccounts();
                    AccountStatusTextBlock.Text = $"Account {_originalAccountEmail} deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting account: {ex.Message}");
                AccountStatusTextBlock.Text = "Error deleting account.";
            }
        }

        // Delete a customer (with account tracking)
        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {_selectedCustomer.Name}?", 
                                            "Confirm Delete", 
                                            MessageBoxButton.YesNo, 
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _dbHelper.DeleteCustomerWithAccountTracking(_selectedCustomer.Id);
                    LoadCustomers();
                    LoadAccounts(); // Refresh accounts to reflect updated counts
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete");
            }
        }

        // Export customer data to CSV
        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            string exportFilePath = @"C:\path\to\exported_customers.csv";  // Specify the correct path
            try
            {
                _dbHelper.ExportCustomersToCsv(exportFilePath);
                MessageBox.Show("Customers data exported successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Add new customer
        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new empty customer object
            var newCustomer = new Customer
            {
                Name = "",
                SubscriptionPackage = "goi1",
                RegisterDay = DateTime.Today,
                SubscriptionExpiry = DateTime.Today.AddMonths(1),
                LastActivity = DateTime.Today,
                Note = ""
            };

            // Create and show the edit window with the new customer
            var editWindow = new EditCustomerWindow(newCustomer);
            editWindow.Owner = this;
            editWindow.Title = "Add New Customer";
            editWindow.ShowDialog();

            if (editWindow.ChangesSaved)
            {
                // Check if a customer with the same name already exists
                var existingCustomers = _dbHelper.GetAllCustomers();
                bool isDuplicate = existingCustomers.Any(c => c.Name.Equals(newCustomer.Name, StringComparison.OrdinalIgnoreCase));
                
                if (isDuplicate)
                {
                    MessageBox.Show($"A customer with the name '{newCustomer.Name}' already exists. Please use a different name.",
                        "Duplicate Customer Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Save the new customer to the database
                _dbHelper.InsertCustomerWithAccountTracking(newCustomer);
                // Refresh the customers list
                LoadCustomers();
                LoadAccounts();
            }
        }

        // Delete selected customer
        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Customer customer)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {customer.Name}?", 
                                           "Confirm Delete", 
                                           MessageBoxButton.YesNo, 
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _dbHelper.DeleteCustomerWithAccountTracking(customer.Id);
                    LoadCustomers();
                    LoadAccounts(); // Refresh accounts to reflect updated counts
                }
            }
        }

        // Handle edit button click from DataGrid
        private void EditCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Customer customer)
            {
                _selectedCustomer = customer;
                
                // Create and show the edit window
                var editWindow = new EditCustomerWindow(_selectedCustomer);
                editWindow.Owner = this;
                editWindow.ShowDialog();

                // Refresh the customer list if changes were saved
                if (editWindow.ChangesSaved)
                {
                    LoadCustomers();
                    LoadAccounts(); // Also refresh accounts to reflect any changes to customer counts
                }
            }
        }

        #region Financial Dashboard Methods
        
        // Calculate financials when period selection changes
        private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Only proceed if UI is initialized and combo box has a selection
            if (!_isUIInitialized || PeriodComboBox?.SelectedItem == null) 
                return;
            
            var selectedItem = ((ComboBoxItem)PeriodComboBox.SelectedItem).Content.ToString();
            
            switch (selectedItem)
            {
                case "Current Month":
                    // Current month
                    _financialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    _financialEndDate = _financialStartDate.AddMonths(1).AddDays(-1);
                    if (FromDatePicker != null) FromDatePicker.Visibility = Visibility.Collapsed;
                    if (ToDatePicker != null) ToDatePicker.Visibility = Visibility.Collapsed;
                    if (FromDateLabel != null) FromDateLabel.Visibility = Visibility.Collapsed;
                    if (ToDateLabel != null) ToDateLabel.Visibility = Visibility.Collapsed;
                    break;
                    
                case "Last Month":
                    // Previous month
                    _financialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                    _financialEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
                    if (FromDatePicker != null) FromDatePicker.Visibility = Visibility.Collapsed;
                    if (ToDatePicker != null) ToDatePicker.Visibility = Visibility.Collapsed;
                    if (FromDateLabel != null) FromDateLabel.Visibility = Visibility.Collapsed;
                    if (ToDateLabel != null) ToDateLabel.Visibility = Visibility.Collapsed;
                    break;
                    
                case "Last 3 Months":
                    // Last 3 months
                    _financialStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-3);
                    _financialEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
                    if (FromDatePicker != null) FromDatePicker.Visibility = Visibility.Collapsed;
                    if (ToDatePicker != null) ToDatePicker.Visibility = Visibility.Collapsed;
                    if (FromDateLabel != null) FromDateLabel.Visibility = Visibility.Collapsed;
                    if (ToDateLabel != null) ToDateLabel.Visibility = Visibility.Collapsed;
                    break;
                    
                case "Custom Period":
                    // Show date pickers for custom period
                    if (FromDatePicker != null) FromDatePicker.Visibility = Visibility.Visible;
                    if (ToDatePicker != null) ToDatePicker.Visibility = Visibility.Visible;
                    if (FromDateLabel != null) FromDateLabel.Visibility = Visibility.Visible;
                    if (ToDateLabel != null) ToDateLabel.Visibility = Visibility.Visible;
                    break;
            }
            
            // Update date pickers
            if (FromDatePicker != null) FromDatePicker.SelectedDate = _financialStartDate;
            if (ToDatePicker != null) ToDatePicker.SelectedDate = _financialEndDate;
            
            CalculateFinancials();
        }
        
        // Handle date picker changes
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isUIInitialized)
                return;
                
            if (FromDatePicker?.SelectedDate != null && ToDatePicker?.SelectedDate != null)
            {
                _financialStartDate = FromDatePicker.SelectedDate.Value;
                _financialEndDate = ToDatePicker.SelectedDate.Value;
                CalculateFinancials();
            }
        }
        
        // Calculate financial data
        private void CalculateFinancials()
        {
            // Skip calculation if UI is not initialized
            if (!_isUIInitialized)
                return;
                
            try
            {
                // Get all customers and accounts for the selected period
                var allCustomers = _dbHelper.GetAllCustomers();
                var allAccounts = _dbHelper.GetAllAccounts();
                
                // Filter customers by the selected period
                // We consider customers whose registration date or expiry date falls within the period
                var customersInPeriod = allCustomers.Where(c => 
                    (c.RegisterDay >= _financialStartDate && c.RegisterDay <= _financialEndDate) ||
                    (c.SubscriptionExpiry >= _financialStartDate && c.SubscriptionExpiry <= _financialEndDate) ||
                    (c.RegisterDay <= _financialStartDate && c.SubscriptionExpiry >= _financialEndDate)
                ).ToList();
                
                // Count packages by type
                int package1Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi1");
                int package3Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi3");
                int package6Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi6");
                int package12Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi12");
                
                // Calculate income for each package type
                decimal package1Income = package1Count * FinancialData.PACKAGE1_PRICE;
                decimal package3Income = package3Count * FinancialData.PACKAGE3_PRICE;
                decimal package6Income = package6Count * FinancialData.PACKAGE6_PRICE;
                decimal package12Income = package12Count * FinancialData.PACKAGE12_PRICE;
                
                // Calculate total income
                decimal totalIncome = package1Income + package3Income + package6Income + package12Income;
                
                // Count active accounts in the period
                var accountsInPeriod = allAccounts.Where(a =>
                    (a.StartDate >= _financialStartDate && a.StartDate <= _financialEndDate) ||
                    (a.ExpireDate >= _financialStartDate && a.ExpireDate <= _financialEndDate) ||
                    (a.StartDate <= _financialStartDate && a.ExpireDate >= _financialEndDate)
                ).ToList();
                
                int accountCount = accountsInPeriod.Count;
                
                // Calculate total expenses
                decimal accountExpenses = accountCount * FinancialData.ACCOUNT_COST;
                decimal totalExpenses = accountExpenses;
                
                // Calculate profit
                decimal totalProfit = totalIncome - totalExpenses;
                
                // Update UI
                UpdateFinancialUI(
                    totalIncome, 
                    totalExpenses, 
                    totalProfit, 
                    customersInPeriod.Count, 
                    accountCount, 
                    package1Count, package3Count, package6Count, package12Count,
                    package1Income, package3Income, package6Income, package12Income
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating financials: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Update financial UI elements
        private void UpdateFinancialUI(
            decimal totalIncome, 
            decimal totalExpenses, 
            decimal totalProfit, 
            int customerCount, 
            int accountCount, 
            int package1Count, int package3Count, int package6Count, int package12Count,
            decimal package1Income, decimal package3Income, decimal package6Income, decimal package12Income)
        {
            // Format currency values
            string currencyFormat = "{0:#,##0} VND";
            
            // Skip UI updates if UI elements aren't available yet
            if (!_isUIInitialized)
                return;
                
            // Update summary with null checks
            if (TotalIncomeTextBlock != null) 
                TotalIncomeTextBlock.Text = string.Format(currencyFormat, totalIncome);
                
            if (TotalExpensesTextBlock != null) 
                TotalExpensesTextBlock.Text = string.Format(currencyFormat, totalExpenses);
                
            if (TotalProfitTextBlock != null)
            {
                TotalProfitTextBlock.Text = string.Format(currencyFormat, totalProfit);
                // Set profit color based on value
                TotalProfitTextBlock.Foreground = totalProfit >= 0 ? Brushes.Green : Brushes.Red;
            }
            
            // Update income breakdown with null checks
            if (Package1CountTextBlock != null) Package1CountTextBlock.Text = package1Count.ToString();
            if (Package3CountTextBlock != null) Package3CountTextBlock.Text = package3Count.ToString();
            if (Package6CountTextBlock != null) Package6CountTextBlock.Text = package6Count.ToString();
            if (Package12CountTextBlock != null) Package12CountTextBlock.Text = package12Count.ToString();
            if (TotalCustomersTextBlock != null) TotalCustomersTextBlock.Text = customerCount.ToString();
            
            if (Package1IncomeTextBlock != null) Package1IncomeTextBlock.Text = string.Format(currencyFormat, package1Income);
            if (Package3IncomeTextBlock != null) Package3IncomeTextBlock.Text = string.Format(currencyFormat, package3Income);
            if (Package6IncomeTextBlock != null) Package6IncomeTextBlock.Text = string.Format(currencyFormat, package6Income);
            if (Package12IncomeTextBlock != null) Package12IncomeTextBlock.Text = string.Format(currencyFormat, package12Income);
            if (TotalIncomeBreakdownTextBlock != null) TotalIncomeBreakdownTextBlock.Text = string.Format(currencyFormat, totalIncome);
            
            // Update expense breakdown with null checks
            if (AccountCountTextBlock != null) AccountCountTextBlock.Text = accountCount.ToString();
            if (AccountExpenseTextBlock != null) AccountExpenseTextBlock.Text = string.Format(currencyFormat, accountCount * FinancialData.ACCOUNT_COST);
            if (TotalExpensesBreakdownTextBlock != null) TotalExpensesBreakdownTextBlock.Text = string.Format(currencyFormat, totalExpenses);
        }
        
        // Handler for Recalculate button
        private void CalculateFinancials_Click(object sender, RoutedEventArgs e)
        {
            CalculateFinancials();
        }
        
        // Export financial report to CSV
        private void ExportFinancialReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a save file dialog
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    DefaultExt = "csv",
                    FileName = $"FinancialReport_{_financialStartDate:yyyy-MM-dd}_to_{_financialEndDate:yyyy-MM-dd}"
                };
                
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Recalculate financials to ensure data is up to date
                    CalculateFinancials();
                    
                    // Get all customers and accounts for the selected period
                    var allCustomers = _dbHelper.GetAllCustomers();
                    var allAccounts = _dbHelper.GetAllAccounts();
                    
                    // Filter customers by the selected period
                    var customersInPeriod = allCustomers.Where(c => 
                        (c.RegisterDay >= _financialStartDate && c.RegisterDay <= _financialEndDate) ||
                        (c.SubscriptionExpiry >= _financialStartDate && c.SubscriptionExpiry <= _financialEndDate) ||
                        (c.RegisterDay <= _financialStartDate && c.SubscriptionExpiry >= _financialEndDate)
                    ).ToList();
                    
                    // Count packages by type
                    int package1Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi1");
                    int package3Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi3");
                    int package6Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi6");
                    int package12Count = customersInPeriod.Count(c => c.SubscriptionPackage?.ToLower() == "goi12");
                    
                    // Calculate income for each package type
                    decimal package1Income = package1Count * FinancialData.PACKAGE1_PRICE;
                    decimal package3Income = package3Count * FinancialData.PACKAGE3_PRICE;
                    decimal package6Income = package6Count * FinancialData.PACKAGE6_PRICE;
                    decimal package12Income = package12Count * FinancialData.PACKAGE12_PRICE;
                    
                    // Calculate total income
                    decimal totalIncome = package1Income + package3Income + package6Income + package12Income;
                    
                    // Count active accounts in the period
                    var accountsInPeriod = allAccounts.Where(a =>
                        (a.StartDate >= _financialStartDate && a.StartDate <= _financialEndDate) ||
                        (a.ExpireDate >= _financialStartDate && a.ExpireDate <= _financialEndDate) ||
                        (a.StartDate <= _financialStartDate && a.ExpireDate >= _financialEndDate)
                    ).ToList();
                    
                    int accountCount = accountsInPeriod.Count;
                    
                    // Calculate total expenses
                    decimal accountExpenses = accountCount * FinancialData.ACCOUNT_COST;
                    decimal totalExpenses = accountExpenses;
                    
                    // Calculate profit
                    decimal totalProfit = totalIncome - totalExpenses;
                    
                    // Write to CSV file
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Write report header
                        writer.WriteLine("Financial Report");
                        writer.WriteLine($"Period: {_financialStartDate:dd/MM/yyyy} - {_financialEndDate:dd/MM/yyyy}");
                        writer.WriteLine();
                        
                        // Write summary
                        writer.WriteLine("Financial Summary");
                        writer.WriteLine($"Total Income,{totalIncome}");
                        writer.WriteLine($"Total Expenses,{totalExpenses}");
                        writer.WriteLine($"Total Profit,{totalProfit}");
                        writer.WriteLine();
                        
                        // Write income breakdown
                        writer.WriteLine("Income Breakdown");
                        writer.WriteLine("Package,Count,Price Per Unit,Total");
                        writer.WriteLine($"Package 1 Month,{package1Count},{FinancialData.PACKAGE1_PRICE},{package1Income}");
                        writer.WriteLine($"Package 3 Months,{package3Count},{FinancialData.PACKAGE3_PRICE},{package3Income}");
                        writer.WriteLine($"Package 6 Months,{package6Count},{FinancialData.PACKAGE6_PRICE},{package6Income}");
                        writer.WriteLine($"Package 12 Months,{package12Count},{FinancialData.PACKAGE12_PRICE},{package12Income}");
                        writer.WriteLine($"Total,{customersInPeriod.Count},,{totalIncome}");
                        writer.WriteLine();
                        
                        // Write expense breakdown
                        writer.WriteLine("Expense Breakdown");
                        writer.WriteLine("Item,Count,Cost Per Unit,Total");
                        writer.WriteLine($"Accounts,{accountCount},{FinancialData.ACCOUNT_COST},{accountExpenses}");
                        writer.WriteLine($"Total,,,{totalExpenses}");
                        writer.WriteLine();
                        
                        // Write customer details
                        writer.WriteLine("Customer Details");
                        writer.WriteLine("ID,Name,Package,Registration Date,Expiry Date,Account Email,Price");
                        foreach (var customer in customersInPeriod)
                        {
                            decimal price = FinancialData.GetPriceForPackage(customer.SubscriptionPackage);
                            writer.WriteLine($"{customer.Id},{customer.Name},{customer.SubscriptionPackage},{customer.RegisterDay:dd/MM/yyyy},{customer.SubscriptionExpiry:dd/MM/yyyy},{customer.Note},{price}");
                        }
                    }
                    
                    MessageBox.Show($"Financial report exported to {saveFileDialog.FileName}", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting financial report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        #endregion
    }

    // Helper class to store expired customer data for the ListView
    public class ExpiredCustomerItem
    {
        public int CustomerId { get; set; }
        public string DisplayText { get; set; }
        public string ReminderText { get; set; }
        public bool IsContinuing { get; set; } = true;
        public Brush TextColor { get; set; } = Brushes.Red;
        public Visibility ContinueButtonVisibility { get; set; } = Visibility.Visible;
        public Visibility MoneyPayVisibility { get; set; } = Visibility.Visible;
        public Visibility DoNotContinueVisibility { get; set; } = Visibility.Visible;
        public Visibility RenewVisibility { get; set; } = Visibility.Visible;
    }
}
