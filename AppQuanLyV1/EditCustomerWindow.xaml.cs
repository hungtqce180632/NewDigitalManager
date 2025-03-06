using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AppQuanLyV1
{
    public partial class EditCustomerWindow : Window
    {
        private Customer _customer;
        private DatabaseHelper _dbHelper;
        public bool ChangesSaved { get; private set; } = false;
        private string _originalEmail;

        public EditCustomerWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _dbHelper = new DatabaseHelper();
            _originalEmail = customer.Note;
            
            // Populate fields with customer data
            CustomerNameTextBox.Text = customer.Name;
            
            // Set the package ComboBox
            string packageValue = customer.SubscriptionPackage;
            foreach (ComboBoxItem item in CustomerPackageComboBox.Items)
            {
                if (item.Content.ToString() == packageValue)
                {
                    CustomerPackageComboBox.SelectedItem = item;
                    break;
                }
            }
            
            // Load account emails from the database
            var accountEmails = _dbHelper.GetAllAccountEmails();
            CustomerEmailComboBox.Items.Clear();
            foreach (string email in accountEmails)
            {
                CustomerEmailComboBox.Items.Add(email);
            }
            CustomerEmailComboBox.Text = customer.Note;
            
            // Set dates
            CustomerRegistrationDatePicker.SelectedDate = customer.RegisterDay;
            CustomerExpirationDatePicker.SelectedDate = customer.SubscriptionExpiry;
            
            // If no package is selected, select one based on the calculated duration
            if (CustomerPackageComboBox.SelectedItem == null)
            {
                CalculatePackageFromDates();
            }
        }

        private void CalculatePackageFromDates()
        {
            if (CustomerRegistrationDatePicker.SelectedDate.HasValue && CustomerExpirationDatePicker.SelectedDate.HasValue)
            {
                var startDate = CustomerRegistrationDatePicker.SelectedDate.Value;
                var endDate = CustomerExpirationDatePicker.SelectedDate.Value;
                var monthsDifference = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
                
                string packageToSelect = "goi1";
                if (monthsDifference >= 12)
                    packageToSelect = "goi12";
                else if (monthsDifference >= 6)
                    packageToSelect = "goi6";
                else if (monthsDifference >= 3)
                    packageToSelect = "goi3";
                
                foreach (ComboBoxItem item in CustomerPackageComboBox.Items)
                {
                    if (item.Content.ToString() == packageToSelect)
                    {
                        CustomerPackageComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void CalculateExpirationDate()
        {
            if (CustomerRegistrationDatePicker.SelectedDate.HasValue && CustomerPackageComboBox.SelectedItem != null)
            {
                var startDate = CustomerRegistrationDatePicker.SelectedDate.Value;
                var packageName = ((ComboBoxItem)CustomerPackageComboBox.SelectedItem).Content.ToString();
                
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
                CustomerExpirationDatePicker.SelectedDate = expirationDate;
            }
        }

        private void CustomerPackageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateExpirationDate();
        }

        private void CustomerRegistrationDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateExpirationDate();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update customer with new values
            _customer.Name = CustomerNameTextBox.Text;
            
            // Get the package from the ComboBox
            if (CustomerPackageComboBox.SelectedItem != null)
            {
                _customer.SubscriptionPackage = ((ComboBoxItem)CustomerPackageComboBox.SelectedItem).Content.ToString();
            }
            
            _customer.Note = CustomerEmailComboBox.Text;
            _customer.RegisterDay = CustomerRegistrationDatePicker.SelectedDate ?? _customer.RegisterDay;
            _customer.SubscriptionExpiry = CustomerExpirationDatePicker.SelectedDate ?? _customer.SubscriptionExpiry;

            // Save to database with account tracking
            _dbHelper.UpdateCustomerWithAccountTracking(_customer, _originalEmail);
            ChangesSaved = true;
            
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
