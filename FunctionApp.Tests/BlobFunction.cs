using FunctionTestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace FunctionApp.Tests
{
    public class BlobFunction : FunctionTest
    {
        [Fact]
        public async Task BlobFunction_ValidStreamAndName()
        {
            Stream s = new MemoryStream();
            using(StreamWriter sw = new StreamWriter(s))
            {
                await sw.WriteLineAsync("This is a test");
                BlobTrigger.Run(s, "testBlob", log);
            }
        }
    }
}
