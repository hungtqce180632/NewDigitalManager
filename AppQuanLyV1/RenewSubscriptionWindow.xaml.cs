using System;
using System.Windows;
using System.Windows.Controls;

namespace AppQuanLyV1
{
    public partial class RenewSubscriptionWindow : Window
    {
        private Customer _customer;
        private DatabaseHelper _dbHelper;
        public bool RenewalCompleted { get; private set; } = false;
        
        public RenewSubscriptionWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _dbHelper = new DatabaseHelper();
            
            // Initialize UI with customer details
            CustomerNameTextBlock.Text = customer.Name;
            
            // Set default values
            StartDatePicker.SelectedDate = DateTime.Today;
            PackageComboBox.SelectedIndex = 0; // default to goi1
            
            // Calculate initial expiration date based on default package
            UpdateExpirationDate();
        }
        
        private void UpdateExpirationDate()
        {
            if (StartDatePicker.SelectedDate.HasValue && PackageComboBox.SelectedItem != null)
            {
                var startDate = StartDatePicker.SelectedDate.Value;
                var packageName = ((ComboBoxItem)PackageComboBox.SelectedItem).Content.ToString();
                
                // Extract the number from the package name
                int months = 1; // Default to 1 month
                if (packageName.StartsWith("goi"))
                {
                    string numberPart = packageName.Substring(3); // Remove "goi" prefix
                    if (int.TryParse(numberPart, out int packageMonths))
                    {
                        months = packageMonths;
                    }
                }
                
                // Calculate the expiration date
                var expirationDate = startDate.AddMonths(months);
                ExpirationDateTextBlock.Text = expirationDate.ToString("dd/MM/yyyy");
            }
        }
        
        private void PackageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateExpirationDate();
        }
        
        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateExpirationDate();
        }
        
        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue && PackageComboBox.SelectedItem != null)
            {
                try
                {
                    string selectedPackage = ((ComboBoxItem)PackageComboBox.SelectedItem).Content.ToString();
                    DateTime startDate = StartDatePicker.SelectedDate.Value;
                    
                    // Renew the customer's subscription
                    _dbHelper.RenewCustomerSubscription(_customer.Id, selectedPackage, startDate);
                    
                    // Show additional message if this was a "Not Continuing" customer
                    if (!_customer.ContinueSubscription)
                    {
                        MessageBox.Show("This customer was previously marked as 'Not Continuing'. " +
                                        "Their status has been reset to continue subscription.", 
                                        "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    
                    RenewalCompleted = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renewing subscription: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
