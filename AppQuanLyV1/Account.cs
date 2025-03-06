using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppQuanLyV1
{
    public class Account
    {
        public string Email { get; set; }
        public int CustomerCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsExpired => DateTime.Now > ExpireDate;
    }
}
