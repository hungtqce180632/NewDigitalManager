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

        // Method to get all customers
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Customers";
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
                        Note = reader["AccountEmail"].ToString()
                        // Bỏ qua LastActivity vì không có trong cơ sở dữ liệu
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
                    writer.WriteLine($"{customer.Id},{customer.Name},{customer.SubscriptionPackage},{customer.RegisterDay:dd/MM/yyyy},{customer.SubscriptionExpiry:dd/MM/yyyy},{customer.LastActivity:dd/MM/yyyy},{customer.Note}");
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

                // Cập nhật thông tin khách hàng
                command.Parameters.AddWithValue("@FacebookLink", customer.Name);
                command.Parameters.AddWithValue("@Package", customer.SubscriptionPackage);
                command.Parameters.AddWithValue("@RegistrationDate", customer.RegisterDay);
                command.Parameters.AddWithValue("@ExpirationDate", customer.SubscriptionExpiry);
                command.Parameters.AddWithValue("@AccountEmail", customer.Note);
                command.Parameters.AddWithValue("@ID", customer.Id);

                // Thực hiện cập nhật
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

                // Thực hiện xóa khách hàng
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
                string query = "SELECT * FROM Customers WHERE ExpirationDate <= GETDATE()"; // Get customers whose subscription has expired
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    expiredCustomers.Add(new Customer
                    {
                        Id = Convert.ToInt32(reader["ID"]),
                        Name = reader["FacebookLink"].ToString(),
                        SubscriptionPackage = reader["Package"].ToString(),
                        RegisterDay = Convert.ToDateTime(reader["RegistrationDate"]),
                        SubscriptionExpiry = Convert.ToDateTime(reader["ExpirationDate"]),
                        // Bỏ qua LastActivity vì không có trong cơ sở dữ liệu
                        Note = reader["AccountEmail"].ToString()
                    });
                }
            }

            return expiredCustomers;
        }

    }
}
