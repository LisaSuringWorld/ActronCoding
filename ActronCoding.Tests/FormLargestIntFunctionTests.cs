using NUnit.Framework;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Internal;

namespace ActronCoding.Tests
{
    public class FormLargestIntFunctionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RunFunction_ReturnsOkResult()
        {
            // Arrange
            var input = new InputModel { Input = new int[] { 10, 223, 78, 90, 99 } };
            var requestBody = JsonConvert.SerializeObject(input);
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody))
            };
            var logger = TestFactory.CreateLogger();

            // Act
            var response = FormLargestIntFunction.Run(request, logger);

            // Log response content for debugging
            /*var responseContent = response != null ? response.Value?.ToString() : "Response is null";
            logger.LogInformation("Response content: " + responseContent);*/

            //var result = (OkObjectResult)response;
            var result = response as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    
            // Parse the response content
            var responseContent = JsonConvert.DeserializeObject<ResponseModel>(result.Value.ToString());

            Assert.IsNotNull(responseContent);
            Assert.AreEqual("99907822310", responseContent.output);
        }

        [Test]
        public void RunFunction_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var input = new InputModel { Input = new int[] { } }; // Invalid input with empty array
            var requestBody = JsonConvert.SerializeObject(input);
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody))
            };
            var logger = TestFactory.CreateLogger();

            // Act
            var response = FormLargestIntFunction.Run(request, logger);
            var result = response as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid input array [non positive integers or empty array]", result.Value);
        }
    }

   
    public class ResponseModel
    {
        public string output { get; set; }
    }

    // Helper class to create a logger
    public static class TestFactory
    {
        public static ILogger CreateLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            return loggerFactory.CreateLogger("TestLogger");
        }
    }
}