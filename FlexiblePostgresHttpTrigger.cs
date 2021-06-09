using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;

namespace My.Function
{
    public static class FlexiblePostgresHttpTrigger
    {
        [FunctionName("FlexiblePostgresHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            DateTime dateTime = QueryDateFromDb();

            string responseMessage = $"DateTime from db: {dateTime.ToUniversalTime()}";

            return new OkObjectResult(responseMessage);
        }

        private static DateTime QueryDateFromDb()
        {
            DateTime dateTime;
            string connString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_RESOURCECONNECTOR_TESTDOTNETFUNCSECRETCONN_CONNECTIONSTRING");
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT NOW()", conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    dateTime = reader.GetDateTime(0);
                    reader.Close();
                }
            }
            return dateTime;
        }
    }
}
