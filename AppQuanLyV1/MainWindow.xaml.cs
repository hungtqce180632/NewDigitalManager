using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppQuanLyV1
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private Customer _selectedCustomer;
        private Account _selectedAccount;
        private string _originalAccountEmail; // To store the original email for updates

        public MainWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            LoadCustomers();
            LoadAccounts();
        }

        // Load customer data into DataGrid
        private void LoadCustomers()
        {
            var customers = _dbHelper.GetAllCustomers();
            var accounts = _dbHelper.GetAllAccounts();
            
            // Mark customers with expired accounts
            foreach (var customer in customers)
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
            }
            
            CustomersDataGrid.ItemsSource = customers;
            LoadExpiredCustomers();
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
                    "Are you sure this customer does not want to continue their subscription?",
                    "Confirm Discontinuation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Mark the customer as not continuing
                    _dbHelper.MarkCustomerAsNotContinuing(customerId);
                    // Reload the expired customers list
                    LoadExpiredCustomers();
                    
                    MessageBox.Show(
                        "Customer marked as not continuing subscription.",
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
            var accounts = _dbHelper.GetAllAccounts();
            AccountsDataGrid.ItemsSource = accounts;
            ClearAccountFields();
        }

        private void ClearAccountFields()
        {
            AccountEmailTextBox.Text = "";
            AccountCustomerCountTextBox.Text = "";
            _selectedAccount = null;
            _originalAccountEmail = null;
            AccountStatusTextBlock.Text = "Ready";
        }

        // Handle the selection of a customer in the DataGrid
        private void CustomersDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customer selectedCustomer)
            {
                // You can populate additional UI elements or perform actions here
                // For example, updating the Edit tab with the selected customer's information
                _selectedCustomer = selectedCustomer;
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
