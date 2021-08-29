﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesAzureFunction.Functions.Entities
{
    class ConsolidatedEntity:TableEntity
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public int Minutes { get; set; }
    }
}
