using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesAzureFunction.Common.Classes
{
    public class SustractTime
    {
        public int TimeTwoDates (DateTime initialDate,DateTime finalDate)
        {
            TimeSpan result = finalDate - initialDate;
            return (int)result.TotalMinutes;
        }
    }
}
