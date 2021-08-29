using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesAzureFunction.Common.Models
{
    public class Consolidated
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public int Minutes { get; set; }
    }
}
