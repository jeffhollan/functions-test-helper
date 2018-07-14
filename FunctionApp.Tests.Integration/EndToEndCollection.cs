using FunctionTestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FunctionApp.Tests.Integration
{
    [CollectionDefinition("Function collection")]
    public class FunctionCollection : ICollectionFixture<TestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        
    }

    public class TestFixture : EndToEndTestFixture
    {
        public TestFixture() :
            base(@"../../../../FunctionApp/bin/Debug/netstandard2.0", "CSharp")
        {
        }

        // If desired you can specify which functions load up in this fixture
        // protected override IEnumerable<string> GetActiveFunctions() => new[] { "BlobTrigger", "EventTrigger", "HttpTrigger" };
    }
}
