using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using EmployeesAzureFunction.Common.Models;
using EmployeesAzureFunction.Common.Responses;
using EmployeesAzureFunction.Functions.Entities;

namespace EmployeesAzureFunction.Functions.Functions
{
    public static class TimeApi
    {
        [FunctionName(nameof(CreateTime))]
        public static async Task<IActionResult> CreateTime(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "time")] HttpRequest req,
         [Table("time", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
         ILogger log)
        {
            log.LogInformation("Receivedd a new time");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Time time = JsonConvert.DeserializeObject<Time>(requestBody);

            if (string.IsNullOrEmpty(time?.Date.ToString())|| 
                string.IsNullOrEmpty(time?.Type.ToString())|| 
                string.IsNullOrEmpty(time?.EmployeeId.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have all parameters"
                });
            }

            TimeEntity timeEntity = new TimeEntity
            {
                Date = time.Date,
                ETag = "*",
                IsConsolidated = false,
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                EmployeeId = time.EmployeeId,
                Type = time.Type

            };

            TableOperation addOperation = TableOperation.Insert(timeEntity);
            await todoTable.ExecuteAsync(addOperation);

            string message = "New todo stored in table";
            log.LogInformation(message);



            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeEntity

            });
            ;
        }


    }
}
