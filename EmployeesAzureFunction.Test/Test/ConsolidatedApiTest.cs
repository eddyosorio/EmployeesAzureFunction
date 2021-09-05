using EmployeesAzureFunction.Common.Models;
using EmployeesAzureFunction.Functions.Entities;
using EmployeesAzureFunction.Functions.Functions;
using EmployeesAzureFunction.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EmployeesAzureFunction.Test.Test
{
   public class ConsolidatedApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void GetConsolidatedByID_Should_Return_200()
        {
            //Arrenge
            MockCloudTableConsolidate mockTodos = new MockCloudTableConsolidate(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Consolidated consolidatedRequest = TestFactory.GetConsolidatedRequest();
             DefaultHttpRequest request = TestFactory.CreateHttpRequest(consolidatedRequest);

            string date = "2021/09/03";
            //Act
            IActionResult response = await ConsolidatedApi.GetConsolidated(request, mockTodos,date,logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
