using EmployeesAzureFunction.Functions.Functions;
using EmployeesAzureFunction.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EmployeesAzureFunction.Test.Test
{
   public  class ScheduleConsolidateTest
    {
        [Fact]
        public void ScheduledConsolidate_Should_Log_Message()
        {
            // Arrange
            MockCloudTableTime mocktime = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidate mockConsolidate = new MockCloudTableConsolidate(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));

            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            ScheduleConsolidate.Run(null, mocktime, mockConsolidate, logger);
            string message = logger.Logs[0];

            // Assert
            Assert.Contains("Consolidate  function", message);

        }
    }
}
