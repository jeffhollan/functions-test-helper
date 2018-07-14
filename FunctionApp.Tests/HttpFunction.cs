using FunctionTestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using Xunit;

namespace FunctionApp.Tests
{
    public class HttpFunction : FunctionTest
    {
        [Fact]
        public void HttpTrigger_ValidBody()
        {
            var result = HttpTrigger.Run(
                req: HttpTestHelpers.CreateHttpRequest("POST", uriString: "http://localhost", body: new { name = "Jeff"}), 
                log: log);

            var resultObject = (OkObjectResult)result;
            Assert.Equal("Hello, Jeff", resultObject.Value);
        }

        [Fact]
        public void HttpTrigger_ValidQuery()
        {
            var result = HttpTrigger.Run(
                req: HttpTestHelpers.CreateHttpRequest("POST", uriString: "http://localhost?name=Jeff"),
                log: log);

            var resultObject = (OkObjectResult)result;
            Assert.Equal("Hello, Jeff", resultObject.Value);
        }

        [Fact]
        public void HttpTrigger_EmptyBodyAndQuery()
        {
            var result = HttpTrigger.Run(
                req: HttpTestHelpers.CreateHttpRequest("POST", uriString: "http://localhost"),
                log: log);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
