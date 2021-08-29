using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesAzureFunction.Functions.Entities
{
    class TimeEntity :TableEntity
    {
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public int Type { get; set; }
        public bool IsConsolidated { get; set; }
    }
}
