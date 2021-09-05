using EmployeesAzureFunction.Common.Classes;
using EmployeesAzureFunction.Common.Models;
using EmployeesAzureFunction.Common.Responses;
using EmployeesAzureFunction.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace EmployeesAzureFunction.Functions.Functions
{
    internal class ConsolidatedApi
    {
        [FunctionName(nameof(ConsolidateProcess))]
        public static async Task<IActionResult> ConsolidateProcess(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "consolidate")] HttpRequest req,
    [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
    [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidateTable,
     ILogger log)
        {
            log.LogInformation("Received a new consolidation");
            int count=0;
            int countAdd = 0;

            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>();
            TableQuerySegment<TimeEntity> times = await timeTable.ExecuteQuerySegmentedAsync(query, null);
            List<TimeEntity> orderEmployees = times.Where(t => !t.IsConsolidated)
                .OrderBy(t => t.EmployeeId)
                .ThenBy(t => t.Date).ToList();

            List<ConsolidatedEntity> consolidatedEntitys = new List<ConsolidatedEntity>();

            for (int i = 0; i < orderEmployees.Count; i++)
            {
                if (i + 1 >= orderEmployees.Count)
                {
                    break;
                }
                if (orderEmployees[i].EmployeeId.Equals(orderEmployees[i + 1].EmployeeId))
                {
                    SustractTime sustract = new SustractTime();
                    int difference = sustract.TimeTwoDates(orderEmployees[i].Date, orderEmployees[i + 1].Date);
                    List<ConsolidatedEntity> validId = consolidatedEntitys.Where(t => t.EmployeeId.Equals(orderEmployees[i].EmployeeId)).ToList();
                    if (validId.Any())
                    {
                        validId[0].Minutes = consolidatedEntitys.Where(t => t.EmployeeId.Equals(orderEmployees[i].EmployeeId))
                        .Sum(t => t.Minutes)+ difference;
                        orderEmployees[i].IsConsolidated = true;
                        orderEmployees[i+1].IsConsolidated = true;
                        count++;
                    }
                    else
                    {
                        ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                        {
                            Date = DateTime.Now,
                            ETag = "*",
                            Minutes = difference,
                            PartitionKey = "TIME",
                            RowKey = Guid.NewGuid().ToString(),
                            EmployeeId = orderEmployees[i].EmployeeId,

                        };
                        orderEmployees[i].IsConsolidated = true;
                        orderEmployees[i + 1].IsConsolidated = true;
                        consolidatedEntitys.Add(consolidatedEntity);
                        count++;
                    }
                    i++;
                }

            }

            TableQuery<ConsolidatedEntity> queryConsolidated = new TableQuery<ConsolidatedEntity>();
            TableQuerySegment<ConsolidatedEntity> consolidated = await consolidateTable.ExecuteQuerySegmentedAsync(queryConsolidated, null);

            foreach (var item in consolidatedEntitys)
            {
               var cosin= consolidated.Where(t => t.EmployeeId.Equals(item.EmployeeId))
                .OrderBy(t => t.EmployeeId)
                .ThenBy(t => t.Date).ToList();

                if (!cosin.Any())
                {
                    TableOperation addOperationConsolidated = TableOperation.Insert(item);
                    await consolidateTable.ExecuteAsync(addOperationConsolidated);
                    countAdd++;
                }
                else
                {
                    cosin[0].Minutes = item.Minutes;

                    TableOperation UpdateConsolidated = TableOperation.Replace(cosin[0]);
                    await timeTable.ExecuteAsync(UpdateConsolidated);
                }


            }
            foreach (var item in orderEmployees)
            {
                TableOperation UpdateTimeConsolidated = TableOperation.Replace(item);
                await timeTable.ExecuteAsync(UpdateTimeConsolidated);
            }





            string message = $"Consolidation sumary: Records added {countAdd}, record update :{count}";
            log.LogInformation(message);



            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = null

            });
            ;
        }
    }
}
