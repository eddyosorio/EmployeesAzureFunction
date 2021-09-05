using EmployeesAzureFunction.Common.Classes;
using EmployeesAzureFunction.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EmployeesAzureFunction.Functions.Functions
{
    class ScheduleConsolidate
    {
        [FunctionName("ScheduleFunction")]
        public static async Task Run(
            [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
        [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
        [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidateTable, ILogger log)
        {
            log.LogInformation($"Concolidate  function executed at:  {DateTime.Now}");
            int count = 0;
            int countAdd = 0;

            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>()
                .Where(TableQuery.GenerateFilterConditionForBool("IsConsolidated", QueryComparisons.Equal
                 , false));
            TableQuerySegment<TimeEntity> times = await timeTable.ExecuteQuerySegmentedAsync(query, null);
            List<TimeEntity> orderEmployees = times
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
                        .Sum(t => t.Minutes) + difference;
                        orderEmployees[i].IsConsolidated = true;
                        orderEmployees[i + 1].IsConsolidated = true;
                        count++;
                    }
                    else
                    {
                        ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                        {
                            Date = orderEmployees[i].Date.Date,
                            ETag = "*",
                            Minutes = difference,
                            PartitionKey = "CONSOLIDATED",
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
            foreach (ConsolidatedEntity item in consolidatedEntitys)
            {
                List<ConsolidatedEntity> validConsolidated = consolidated.Where(t => t.EmployeeId.Equals(item.EmployeeId))
                .Where(t => t.Date.ToString("dd/MM/yyyy").Equals(item.Date.ToString("dd/MM/yyyy")))
                .OrderBy(t => t.EmployeeId)
                .ThenBy(t => t.Date).ToList();

                if (!validConsolidated.Any())
                {
                    TableOperation addOperationConsolidated = TableOperation.Insert(item);
                    await consolidateTable.ExecuteAsync(addOperationConsolidated);
                    countAdd++;
                }
                else
                {
                    validConsolidated[0].Minutes = validConsolidated[0].Minutes + item.Minutes;

                    TableOperation UpdateConsolidated = TableOperation.Replace(validConsolidated[0]);
                    await consolidateTable.ExecuteAsync(UpdateConsolidated);
                }


            }
            foreach (TimeEntity item in orderEmployees)
            {
                TableOperation UpdateTimeConsolidated = TableOperation.Replace(item);
                await timeTable.ExecuteAsync(UpdateTimeConsolidated);
            }

            string message = $"Consolidation sumary: Records added {countAdd}, record update :{count}";
            log.LogInformation(message);


            ;

        }
    }
}

