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
            var result = (OkObjectResult)response;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
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
            var result = (BadRequestObjectResult)response;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }
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