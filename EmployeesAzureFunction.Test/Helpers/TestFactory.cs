using EmployeesAzureFunction.Common.Models;
using EmployeesAzureFunction.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EmployeesAzureFunction.Test.Helpers
{
    public class TestFactory
    {

        public static List<TimeEntity> GetTimesEntity()
        {


            return new List<TimeEntity>
            {
                     new TimeEntity
            {
                Date = DateTime.UtcNow,
                ETag = "*",
                IsConsolidated = false,
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                EmployeeId = 1,
                Type = 0
            }
        };
        }
        public static TimeEntity GetTimeEntity()
        {
            return
                 new TimeEntity
                 {
                     Date = DateTime.UtcNow,
                     ETag = "*",
                     IsConsolidated = false,
                     PartitionKey = "TIME",
                     RowKey = Guid.NewGuid().ToString(),
                     EmployeeId = 1,
                     Type = 0

                 };

        }

        public static List<ConsolidatedEntity> GetConsolidateEntity()
        {
            return
                 new List<ConsolidatedEntity>
                 {
                     new ConsolidatedEntity
                 {
                     Date = DateTime.UtcNow,
                     ETag = "*",
                     PartitionKey = "CONSOLIDATED",
                     RowKey = Guid.NewGuid().ToString(),
                     EmployeeId = 1,

                 }
                 };
        }


        public static DefaultHttpRequest CreateHttpRequest(Guid timeId, Time timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())

            {
                Body = GenerateStreamFromString(request),
                Path = $"/{timeId}"
            };

        }
        public static DefaultHttpRequest CreateHttpRequest(Guid timeId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())

            {
                Path = $"/{timeId}"
            };

        }
        public static DefaultHttpRequest CreateHttpRequest(Consolidated consolidatedRequest)
        {
            string request = JsonConvert.SerializeObject(consolidatedRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())

            {
                Body = GenerateStreamFromString(request),
            };

        }
        public static DefaultHttpRequest CreateHttpRequest(Time timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())

            {
                Body = GenerateStreamFromString(request),
            };

        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());

        }

        public static Time GetTimeRequest()
        {
            return new Time
            {
                Date = DateTime.UtcNow,
                EmployeeId = 1,
                Type = 0,
                IsConsolidated = false
            };
        }

        public static Consolidated GetConsolidatedRequest()
        {
            return new Consolidated
            {
                Date = DateTime.UtcNow,
                EmployeeId = 1,
                Minutes = 100
            };
        }
        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;

        }

        public static ILogger CreateLogger(LoggerTypes Type = LoggerTypes.Null)
        {

            ILogger logger;
            if (Type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }
            return logger;
        }


    }
}