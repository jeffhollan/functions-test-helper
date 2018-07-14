using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FunctionTestHelper
{
    public abstract class FunctionTest
    {
        public TestLogger log = new TestLogger("Test");
    }
    
}
