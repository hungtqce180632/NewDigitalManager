using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace AppQuanLyV1
{
    public class DatabaseHelper
    {
        private string _connectionString = "Server=SMOKEYDUCK132\\SQLEXPRESS;Database=AppQuanLyMoi;User Id=sa;Password=123;";

        // Insert customer data from the file
        public void InsertCustomerDataFromFile(string filePath)
        {
            var lines = System.IO.File.ReadLines(filePath);
            foreach (var line in lines.Skip(1)) // Skip the header
            {
                var fields = line.Split(',');
                if (fields.Length >= 7)
                {
                    var customer = new Customer
                    {
                        Id = Convert.ToInt32(fields[0]),
                        Name = fields[1],
                        SubscriptionPackage = fields[2],
                        RegisterDay = DateTime.ParseExact(fields[3], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        SubscriptionExpiry = DateTime.ParseExact(fields[4], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        LastActivity = DateTime.ParseExact(fields[5], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Note = fields[6]
                    };

                    InsertCustomer(customer);
                }
            }
        }

        private void InsertCustomer(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Customers (FacebookLink, RegistrationDate, ExpirationDate, Package, AccountEmail) " +
                               "VALUES (@FacebookLink, @RegistrationDate, @ExpirationDate, @Package, @AccountEmail)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FacebookLink", customer.Name);
                command.Parameters.AddWithValue("@RegistrationDate", customer.RegisterDay);
                command.Parameters.AddWithValue("@ExpirationDate", customer.SubscriptionExpiry);
                command.Parameters.AddWithValue("@Package", customer.SubscriptionPackage);
                command.Parameters.AddWithValue("@AccountEmail", customer.Note);
                command.ExecuteNonQuery();
            }
        }

        // Method to get all customers
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // First, check if the ContinueSubscription column exists
                bool columnExists = false;
                string checkColumnQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customers' AND COLUMN_NAME = 'ContinueSubscription'";
                using (SqlCommand checkCommand = new SqlCommand(checkColumnQuery, connection))
                {
                    int columnCount = (int)checkCommand.ExecuteScalar();
                    columnExists = columnCount > 0;
                }

                // Prepare the query based on whether the column exists
                string query;
                if (columnExists)
                {
                    // Get ALL customers, both continuing and not continuing
                    query = "SELECT *, ISNULL(ContinueSubscription, 1) AS ContinueStatus FROM Customers";
                }
                else
                {
                    query = "SELECT *, 1 AS ContinueStatus FROM Customers";
                }
                
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        Id = Convert.ToInt32(reader["ID"]),
                        Name = reader["FacebookLink"].ToString(),
                        SubscriptionPackage = reader["Package"].ToString(),
                        RegisterDay = Convert.ToDateTime(reader["RegistrationDate"]),
                        SubscriptionExpiry = Convert.ToDateTime(reader["ExpirationDate"]),
                        Note = reader["AccountEmail"].ToString(),
                        ContinueSubscription = Convert.ToBoolean(reader["ContinueStatus"])
                    });
                }
            }

            return customers;
        }

        // Method to export customer data to CSV
        public void ExportCustomersToCsv(string filePath)
        {
            // Get all customers from the database
            var customers = GetAllCustomers();

            // Open the file to write data
            using (var writer = new StreamWriter(filePath))
            {
                // Write the header of the CSV file
                writer.WriteLine("Id,Name,SubscriptionPackage,RegisterDay,SubscriptionExpiry,LastActivity,Note");

                // Loop through each customer and write their data to the file
                foreach (var customer in customers)
                {
                    writer.WriteLine($"{customer.Id},{customer.Name},{customer.SubscriptionPackage},{customer.RegisterDay:dd/MM/yyyy},{customer.SubscriptionExpiry:dd/MM/yyyy},{DateTime.Now:dd/MM/yyyy},{customer.Note}");
                }
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Customers SET FacebookLink = @FacebookLink, Package = @Package, RegistrationDate = @RegistrationDate, ExpirationDate = @ExpirationDate, AccountEmail = @AccountEmail WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@FacebookLink", customer.Name);
                command.Parameters.AddWithValue("@Package", customer.SubscriptionPackage);
                command.Parameters.AddWithValue("@RegistrationDate", customer.RegisterDay);
                command.Parameters.AddWithValue("@ExpirationDate", customer.SubscriptionExpiry);
                command.Parameters.AddWithValue("@AccountEmail", customer.Note);
                command.Parameters.AddWithValue("@ID", customer.Id);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteCustomer(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Customers WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", customerId);
                command.ExecuteNonQuery();
            }
        }

        // Method to get expired customers
        public List<Customer> GetExpiredCustomers()
        {
            List<Customer> expiredCustomers = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // First, check if the ContinueSubscription column exists
                bool columnExists = false;
                string checkColumnQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customers' AND COLUMN_NAME = 'ContinueSubscription'";
                using (SqlCommand checkCommand = new SqlCommand(checkColumnQuery, connection))
                {
                    int columnCount = (int)checkCommand.ExecuteScalar();
                    columnExists = columnCount > 0;
                }

                // Prepare the query based on whether the column exists
                string query;
                if (columnExists)
                {
                    query = "SELECT *, ISNULL(ContinueSubscription, 1) AS ContinueStatus FROM Customers WHERE ExpirationDate <= GETDATE()";
                }
                else
                {
                    query = "SELECT *, 1 AS ContinueStatus FROM Customers WHERE ExpirationDate <= GETDATE()";
                }
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expiredCustomers.Add(new Customer
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                Name = reader["FacebookLink"].ToString(),
                                SubscriptionPackage = reader["Package"].ToString(),
                                RegisterDay = Convert.ToDateTime(reader["RegistrationDate"]),
                                SubscriptionExpiry = Convert.ToDateTime(reader["ExpirationDate"]),
                                Note = reader["AccountEmail"].ToString(),
                                ContinueSubscription = Convert.ToBoolean(reader["ContinueStatus"])
                            });
                        }
                    }
                }
            }

            return expiredCustomers;
        }
        
        // Renew customer subscription
        public void RenewCustomerSubscription(int customerId, string package, DateTime renewalDate)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Calculate new expiration date based on package
                        int months = 1;
                        if (package.StartsWith("goi"))
                        {
                            string numberPart = package.Substring(3);
                            if (int.TryParse(numberPart, out int packageMonths))
                            {
                                months = packageMonths;
                            }
                        }
                        
                        DateTime newExpirationDate = renewalDate.AddMonths(months);
                        
                        // Update customer with new package, registration date, expiration date, and reset ContinueSubscription to true
                        string query = "UPDATE Customers SET Package = @Package, RegistrationDate = @RegistrationDate, " +
                                      "ExpirationDate = @ExpirationDate, ContinueSubscription = 1 WHERE ID = @ID";
                        
                        SqlCommand command = new SqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@Package", package);
                        command.Parameters.AddWithValue("@RegistrationDate", renewalDate);
                        command.Parameters.AddWithValue("@ExpirationDate", newExpirationDate);
                        command.Parameters.AddWithValue("@ID", customerId);
                        command.ExecuteNonQuery();
                        
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Get all accounts from the database
        public List<Account> GetAllAccounts()
        {
            List<Account> accounts = new List<Account>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Ensure date columns exist
                EnsureAccountDateColumns(connection);
                
                string query = "SELECT Email, CustomerCount, StartDate, ExpireDate FROM Accounts";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var account = new Account
                    {
                        Email = reader["Email"].ToString(),
                        CustomerCount = Convert.ToInt32(reader["CustomerCount"])
                    };
                    
                    // Handle nullable date fields
                    if (reader["StartDate"] != DBNull.Value)
                        account.StartDate = Convert.ToDateTime(reader["StartDate"]);
                    else
                        account.StartDate = DateTime.MinValue;
                        
                    if (reader["ExpireDate"] != DBNull.Value)
                        account.ExpireDate = Convert.ToDateTime(reader["ExpireDate"]);
                    else
                        account.ExpireDate = DateTime.MinValue;
                        
                    accounts.Add(account);
                }
            }

            return accounts;
        }

        // Add a new account
        public void AddAccount(Account account)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Accounts (Email, CustomerCount) VALUES (@Email, @CustomerCount)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@CustomerCount", account.CustomerCount);
                command.ExecuteNonQuery();
            }
        }

        // Add a new account with dates
        public void AddAccountWithDates(Account account)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // First, check if StartDate and ExpireDate columns exist
                EnsureAccountDateColumns(connection);
                
                string query = "INSERT INTO Accounts (Email, CustomerCount, StartDate, ExpireDate) VALUES (@Email, @CustomerCount, @StartDate, @ExpireDate)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@CustomerCount", account.CustomerCount);
                command.Parameters.AddWithValue("@StartDate", account.StartDate);
                command.Parameters.AddWithValue("@ExpireDate", account.ExpireDate);
                command.ExecuteNonQuery();
            }
        }

        // Update an existing account
        public void UpdateAccount(Account account, string originalEmail)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Accounts SET Email = @Email, CustomerCount = @CustomerCount WHERE Email = @OriginalEmail";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@CustomerCount", account.CustomerCount);
                command.Parameters.AddWithValue("@OriginalEmail", originalEmail);
                command.ExecuteNonQuery();
            }
        }

        // Update an existing account with dates
        public void UpdateAccountWithDates(Account account, string originalEmail)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Ensure date columns exist
                EnsureAccountDateColumns(connection);
                
                string query = "UPDATE Accounts SET Email = @Email, CustomerCount = @CustomerCount, StartDate = @StartDate, ExpireDate = @ExpireDate WHERE Email = @OriginalEmail";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@CustomerCount", account.CustomerCount);
                
                if (account.StartDate != DateTime.MinValue)
                    command.Parameters.AddWithValue("@StartDate", account.StartDate);
                else
                    command.Parameters.AddWithValue("@StartDate", DBNull.Value);
                
                if (account.ExpireDate != DateTime.MinValue)
                    command.Parameters.AddWithValue("@ExpireDate", account.ExpireDate);
                else
                    command.Parameters.AddWithValue("@ExpireDate", DBNull.Value);
                
                command.Parameters.AddWithValue("@OriginalEmail", originalEmail);
                command.ExecuteNonQuery();
            }
        }

        // Delete an account
        public void DeleteAccount(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Accounts WHERE Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();
            }
        }

        // Export accounts data to CSV
        public void ExportAccountsToCsv(string filePath)
        {
            var accounts = GetAllAccounts();

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Email,CustomerCount");

                foreach (var account in accounts)
                {
                    writer.WriteLine($"{account.Email},{account.CustomerCount}");
                }
            }
        }

        // Get all email addresses from accounts
        public List<string> GetAllAccountEmails()
        {
            List<string> emails = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Email FROM Accounts";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    emails.Add(reader["Email"].ToString());
                }
            }

            return emails;
        }

        // Update customer count for an account (increment or decrement)
        public void UpdateAccountCustomerCount(string email, bool increment)
        {
            if (string.IsNullOrEmpty(email)) return;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Accounts SET CustomerCount = CustomerCount + @Change WHERE Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Change", increment ? 1 : -1);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();
            }
        }

        // Get customer count for an account
        public int GetAccountCustomerCount(string email)
        {
            int count = 0;

            if (string.IsNullOrEmpty(email)) return count;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT CustomerCount FROM Accounts WHERE Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                var result = command.ExecuteScalar();
                
                if (result != null && result != DBNull.Value)
                {
                    count = Convert.ToInt32(result);
                }
            }

            return count;
        }

        // Check if account exists
        public bool AccountExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Accounts WHERE Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // Ensure account table has date columns
        private void EnsureAccountDateColumns(SqlConnection connection)
        {
            // Check if StartDate column exists
            string checkStartDateQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Accounts' AND COLUMN_NAME = 'StartDate'";
            using (SqlCommand checkCommand = new SqlCommand(checkStartDateQuery, connection))
            {
                int columnCount = (int)checkCommand.ExecuteScalar();
                if (columnCount == 0)
                {
                    // Add StartDate column
                    string addQuery = "ALTER TABLE Accounts ADD StartDate DATETIME NULL";
                    using (SqlCommand addCommand = new SqlCommand(addQuery, connection))
                    {
                        addCommand.ExecuteNonQuery();
                    }
                }
            }
            
            // Check if ExpireDate column exists
            string checkExpireDateQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Accounts' AND COLUMN_NAME = 'ExpireDate'";
            using (SqlCommand checkCommand = new SqlCommand(checkExpireDateQuery, connection))
            {
                int columnCount = (int)checkCommand.ExecuteScalar();
                if (columnCount == 0)
                {
                    // Add ExpireDate column
                    string addQuery = "ALTER TABLE Accounts ADD ExpireDate DATETIME NULL";
                    using (SqlCommand addCommand = new SqlCommand(addQuery, connection))
                    {
                        addCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        // Update customer with account tracking
        public void UpdateCustomerWithAccountTracking(Customer customer, string originalEmail)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // If the email is changed, update account counts
                        if (!string.IsNullOrEmpty(originalEmail) && originalEmail != customer.Note)
                        {
                            // Decrement the count for the old account
                            if (AccountExists(originalEmail))
                            {
                                UpdateAccountCustomerCount(originalEmail, false);
                            }

                            // Increment the count for the new account
                            if (AccountExists(customer.Note))
                            {
                                UpdateAccountCustomerCount(customer.Note, true);
                            }
                        }
                        else if (string.IsNullOrEmpty(originalEmail) && !string.IsNullOrEmpty(customer.Note))
                        {
                            // New email assignment
                            if (AccountExists(customer.Note))
                            {
                                UpdateAccountCustomerCount(customer.Note, true);
                            }
                        }

                        // Update customer record
                        string query = "UPDATE Customers SET FacebookLink = @FacebookLink, Package = @Package, RegistrationDate = @RegistrationDate, ExpirationDate = @ExpirationDate, AccountEmail = @AccountEmail WHERE ID = @ID";
                        SqlCommand command = new SqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@FacebookLink", customer.Name);
                        command.Parameters.AddWithValue("@Package", customer.SubscriptionPackage);
                        command.Parameters.AddWithValue("@RegistrationDate", customer.RegisterDay);
                        command.Parameters.AddWithValue("@ExpirationDate", customer.SubscriptionExpiry);
                        command.Parameters.AddWithValue("@AccountEmail", customer.Note);
                        command.Parameters.AddWithValue("@ID", customer.Id);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Delete customer with account tracking
        public void DeleteCustomerWithAccountTracking(int customerId)
        {
            // First get the customer's account email
            string accountEmail = "";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT AccountEmail FROM Customers WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", customerId);
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    accountEmail = result.ToString();
                }
            }

            // Now delete the customer and update the account count
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete the customer
                        string query = "DELETE FROM Customers WHERE ID = @ID";
                        SqlCommand command = new SqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@ID", customerId);
                        command.ExecuteNonQuery();

                        // Update account count if needed
                        if (!string.IsNullOrEmpty(accountEmail) && AccountExists(accountEmail))
                        {
                            UpdateAccountCustomerCount(accountEmail, false);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Insert a new customer with account tracking
        public void InsertCustomerWithAccountTracking(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert the customer
                        string query = "INSERT INTO Customers (FacebookLink, RegistrationDate, ExpirationDate, Package, AccountEmail) " +
                                      "VALUES (@FacebookLink, @RegistrationDate, @ExpirationDate, @Package, @AccountEmail); SELECT SCOPE_IDENTITY();";
                        SqlCommand command = new SqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@FacebookLink", customer.Name);
                        command.Parameters.AddWithValue("@RegistrationDate", customer.RegisterDay);
                        command.Parameters.AddWithValue("@ExpirationDate", customer.SubscriptionExpiry);
                        command.Parameters.AddWithValue("@Package", customer.SubscriptionPackage);
                        command.Parameters.AddWithValue("@AccountEmail", customer.Note);
                        
                        // Get the new customer ID
                        decimal id = (decimal)command.ExecuteScalar();
                        customer.Id = Convert.ToInt32(id);

                        // Update account customer count if needed
                        if (!string.IsNullOrEmpty(customer.Note) && AccountExists(customer.Note))
                        {
                            UpdateAccountCustomerCount(customer.Note, true);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Mark customer as not continuing their subscription
        public void MarkCustomerAsNotContinuing(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // First, check if the ContinueSubscription column exists
                bool columnExists = false;
                string checkColumnQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customers' AND COLUMN_NAME = 'ContinueSubscription'";
                using (SqlCommand checkCommand = new SqlCommand(checkColumnQuery, connection))
                {
                    int columnCount = (int)checkCommand.ExecuteScalar();
                    columnExists = columnCount > 0;
                }

                // If column doesn't exist, add it
                if (!columnExists)
                {
                    string addColumnQuery = "ALTER TABLE Customers ADD ContinueSubscription BIT NOT NULL DEFAULT 1";
                    using (SqlCommand addColumnCommand = new SqlCommand(addColumnQuery, connection))
                    {
                        addColumnCommand.ExecuteNonQuery();
                    }
                }

                // Update the customer's ContinueSubscription status
                string query = "UPDATE Customers SET ContinueSubscription = 0 WHERE ID = @ID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", customerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Insert account data from the file
        public void InsertAccountDataFromFile(string filePath)
        {
            var lines = System.IO.File.ReadLines(filePath);
            foreach (var line in lines.Skip(1)) // Skip the header
            {
                var fields = line.Split(',');
                if (fields.Length >= 2)
                {
                    var account = new Account
                    {
                        Email = fields[0],
                        CustomerCount = Convert.ToInt32(fields[1])
                    };

                    InsertAccount(account);
                }
            }
        }

        private void InsertAccount(Account account)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Accounts (Email, CustomerCount) VALUES (@Email, @CustomerCount)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@CustomerCount", account.CustomerCount);
                command.ExecuteNonQuery();
            }
        }

        // Method to mark customer as not continuing and mark email as expired
        public void MarkCustomerAsNotContinuingAndClearEmail(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // First, get the customer's current email to update account counts
                string currentEmail = null;
                string checkEmailQuery = "SELECT AccountEmail FROM Customers WHERE ID = @ID";
                using (SqlCommand checkCommand = new SqlCommand(checkEmailQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@ID", customerId);
                    var result = checkCommand.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        currentEmail = result.ToString();
                    }
                }
                
                // Update customer to not continue and set AccountEmail to "Expired"
                string updateQuery = "UPDATE Customers SET ContinueSubscription = 0, AccountEmail = 'Expired' WHERE ID = @ID";
                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@ID", customerId);
                    updateCommand.ExecuteNonQuery();
                }
                
                // If the customer had an account associated, update customer count
                if (!string.IsNullOrEmpty(currentEmail) && currentEmail != "Expired" && AccountExists(currentEmail))
                {
                    UpdateAccountCustomerCount(currentEmail, false);
                }
            }
        }

        // Fix existing records that are marked as not continuing but still have an email
        public void FixExistingNotContinuingEmails()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Get list of customers marked as not continuing but with emails
                Dictionary<int, string> customersToFix = new Dictionary<int, string>();
                string findQuery = "SELECT ID, AccountEmail FROM Customers WHERE ContinueSubscription = 0 AND AccountEmail != 'Expired' AND AccountEmail IS NOT NULL AND AccountEmail != ''";
                using (SqlCommand findCommand = new SqlCommand(findQuery, connection))
                {
                    using (SqlDataReader reader = findCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["ID"]);
                            string email = reader["AccountEmail"].ToString();
                            customersToFix.Add(id, email);
                        }
                    }
                }
                
                // Update each customer's email to "Expired" and adjust account counts
                foreach (var customer in customersToFix)
                {
                    // Update to "Expired"
                    string updateQuery = "UPDATE Customers SET AccountEmail = 'Expired' WHERE ID = @ID";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@ID", customer.Key);
                        updateCommand.ExecuteNonQuery();
                    }
                    
                    // Decrement account count
                    if (!string.IsNullOrEmpty(customer.Value) && AccountExists(customer.Value))
                    {
                        UpdateAccountCustomerCount(customer.Value, false);
                    }
                }
            }
        }

        // Get customer by ID
        public Customer GetCustomerById(int customerId)
        {
            Customer customer = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT *, ISNULL(ContinueSubscription, 1) AS ContinueStatus FROM Customers WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", customerId);
                
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            Name = reader["FacebookLink"].ToString(),
                            SubscriptionPackage = reader["Package"].ToString(),
                            RegisterDay = Convert.ToDateTime(reader["RegistrationDate"]),
                            SubscriptionExpiry = Convert.ToDateTime(reader["ExpirationDate"]),
                            Note = reader["AccountEmail"]?.ToString(),
                            ContinueSubscription = Convert.ToBoolean(reader["ContinueStatus"])
                        };
                    }
                }
            }
            
            return customer;
        }
    }
}
