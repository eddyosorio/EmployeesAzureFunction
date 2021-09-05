using EmployeesAzureFunction.Common.Models;
using EmployeesAzureFunction.Functions.Entities;
using EmployeesAzureFunction.Functions.Functions;
using EmployeesAzureFunction.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EmployeesAzureFunction.Test.Test
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();
        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTime mockTodos = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRequest);
            //Act
            IActionResult response = await TimeApi.CreateTime(request, mockTodos, logger);
            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void Update_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTime mockTodos = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time todoRequest = TestFactory.GetTimeRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequest);
            //Act
            IActionResult response = await TimeApi.UpdateTime(request, mockTodos, todoId.ToString(), logger);
            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public async void Delete_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTime mockTodos = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            TimeEntity timeEntity = TestFactory.GetTimeEntity();

            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId);
            //Act
            IActionResult response = await TimeApi.DeleteTime(request, timeEntity, mockTodos, todoId.ToString(), logger);
            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

       
        public async void GetById_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTime mockTodos = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            TimeEntity timeEntity = TestFactory.GetTimeEntity();
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId);
            //Act
        //    IActionResult response = await TimeApi.GetTimeById(request, timeEntity, timeId.ToString(), logger);
           
            //Assert

           // OkObjectResult result = (OkObjectResult)response;
           // Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllTimes_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTime mockTodos = new MockCloudTableTime(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
        
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId);
            //Act
            IActionResult response = await TimeApi.GetAllTimes(request, mockTodos,  logger);

            //Assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
