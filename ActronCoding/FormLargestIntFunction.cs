using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ActronCoding
{
    public class FormLargestIntFunction
    {
        private readonly ILogger<FormLargestIntFunction> _logger;

        public FormLargestIntFunction(ILogger<FormLargestIntFunction> log)
        {
            _logger = log;
        }

        [FunctionName("FormLargestInt")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Actron Coding Challenge" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]

        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(InputModel), Required = true, Description = "Input array of integers")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(OutputModel), Description = "Largest value")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Invalid input array [non positive integers or empty array]")]

        //public IActionResult Run(
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "formLargestInt")] HttpRequest req,
            ILogger log)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //string requestBody = new StreamReader(req.Body).ReadToEnd();
            var input = JsonConvert.DeserializeObject<InputModel>(requestBody);

            if (input == null || input.Input == null || input.Input.Length == 0 || input.Input.Any(x => x <= 0))
            {
                return new BadRequestObjectResult("Invalid input array [non positive integers or empty array]");
            }

            // Sort the input array to form the largest integer
            Array.Sort(input.Input, (a, b) =>
            {
                string order1 = a.ToString() + b.ToString();
                string order2 = b.ToString() + a.ToString();
                return order2.CompareTo(order1);
            });

            var largestInt = string.Join("", input.Input);

            // Return JSON response
            /*var jsonResponse = JsonConvert.SerializeObject(new { output = largestInt });
            return new JsonResult(jsonResponse)
            {
                //Value = jsonResponse,
                ContentType = "application/json",
                StatusCode = 200
            };*/

            // Return JSON response
            return new JsonResult(new OutputModel { output = largestInt })
            {
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
            /*dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);*/


        }
    }

    /*public static class FormLargestIntFunction
    {
        [FunctionName("FormLargestInt")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "formLargestInt")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Read the request body
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var input = JsonConvert.DeserializeObject<InputModel>(requestBody);

            if (input == null || input.Input == null || input.Input.Length == 0 || input.Input.Any(x => x <= 0))
            {
                return new BadRequestObjectResult("Invalid input array [non positive integers or empty array]");
            }

            // Sort the input array to form the largest integer
            Array.Sort(input.Input, (a, b) =>
            {
                string order1 = a.ToString() + b.ToString();
                string order2 = b.ToString() + a.ToString();
                return order2.CompareTo(order1);
            });

            var largestInt = string.Join("", input.Input);
            //return new OkObjectResult(new { output = largestInt });
            //return new JsonResult(new { output = largestInt })
            //{
            //    StatusCode = 200,
            //    ContentType = "application/json"
            //};

            // Return JSON response
            var jsonResponse = JsonConvert.SerializeObject(new { output = largestInt });
            return new JsonResult(jsonResponse)
            {
                //Value = jsonResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }*/

    public class InputModel
    {
        public int[] Input { get; set; }
    }

    public class OutputModel
    {
        public string output { get; set; }
    }
}

