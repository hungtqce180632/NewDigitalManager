using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AppQuanLyV1
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubscriptionPackage { get; set; }
        public DateTime RegisterDay { get; set; }
        public DateTime SubscriptionExpiry { get; set; }
        public DateTime LastActivity { get; set; }
        public string Note { get; set; }
        public bool ContinueSubscription { get; set; } = true; // Default to true (wanting to continue)
        
        // These properties are for UI display only
        public string Status { get; set; } = string.Empty;
        public Brush StatusColor { get; set; } = Brushes.Black;
    }
}
