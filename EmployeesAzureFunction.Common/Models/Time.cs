using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesAzureFunction.Common.Models
{
    public class Time
    {
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public int Type { get; set; }
        public bool IsConsolidated{ get; set; }

    }
}
