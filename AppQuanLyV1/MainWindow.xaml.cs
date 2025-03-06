using System;
using System.Windows;
using System.Windows.Controls;

namespace AppQuanLyV1
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private Customer _selectedCustomer;

        public MainWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            LoadCustomers();
        }

        // Load customer data into DataGrid
        private void LoadCustomers()
        {
            var customers = _dbHelper.GetAllCustomers();
            CustomersDataGrid.ItemsSource = customers;

            var expiredCustomers = _dbHelper.GetExpiredCustomers();
            foreach (var customer in expiredCustomers)
            {
                var listItem = new ListBoxItem
                {
                    Content = $"{customer.Name} - Expired on {customer.SubscriptionExpiry:dd/MM/yyyy}",
                    Foreground = System.Windows.Media.Brushes.Red
                };
                ExpiredCustomersList.Items.Add(listItem);
            }
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

        // Open the customer editing tab when the Edit button is clicked
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer != null)
            {
                // Show the Edit Customer tab and populate fields with the selected customer’s data
                EditCustomerTab.Visibility = Visibility.Visible;
                EditCustomerName.Text = _selectedCustomer.Name;
                EditCustomerPackage.Text = _selectedCustomer.SubscriptionPackage;
                EditCustomerEmail.Text = _selectedCustomer.Note;
                EditCustomerRegistrationDate.SelectedDate = _selectedCustomer.RegisterDay;
                EditCustomerExpirationDate.SelectedDate = _selectedCustomer.SubscriptionExpiry;
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
    }
}
