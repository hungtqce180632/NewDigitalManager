using System;

namespace AppQuanLyV1
{
    public class FinancialData
    {
        // Package pricing constants
        public const decimal PACKAGE1_PRICE = 71000m;
        public const decimal PACKAGE3_PRICE = 195000m;
        public const decimal PACKAGE6_PRICE = 360000m;
        public const decimal PACKAGE12_PRICE = 660000m;
        public const decimal ACCOUNT_COST = 199000m;

        // Properties for financial summary
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalProfit { get; set; }
        public int CustomerCount { get; set; }
        public int AccountCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Detailed breakdown
        public int Package1Count { get; set; }
        public int Package3Count { get; set; }
        public int Package6Count { get; set; }
        public int Package12Count { get; set; }

        // Income breakdown
        public decimal Package1Income { get; set; }
        public decimal Package3Income { get; set; }
        public decimal Package6Income { get; set; }
        public decimal Package12Income { get; set; }

        // Helper method to get price based on package name
        public static decimal GetPriceForPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return 0;

            switch (packageName.ToLower())
            {
                case "goi1": return PACKAGE1_PRICE;
                case "goi3": return PACKAGE3_PRICE;
                case "goi6": return PACKAGE6_PRICE;
                case "goi12": return PACKAGE12_PRICE;
                default: return 0;
            }
        }
    }
}
