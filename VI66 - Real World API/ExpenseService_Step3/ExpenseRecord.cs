using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseService_Step3
{
    public class ExpenseRecord
    {
        public string Employee { get; set; }
        public DateTime TransDate { get; set; }
        public string Description { get; set; }
        public string WBS1 { get; set; }
        public string WBS2 { get; set; }
        public string WBS3 { get; set; }
        public string Account { get; set; }
        public decimal Amount { get; set; }
        public bool Billable { get; set; }
        public int Period { get; set; }
        public string Company { get; set; }
    }
}
