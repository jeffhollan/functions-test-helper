using FunctionTestHelper;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FunctionApp.Tests.Integration
{
    [Collection("Function collection")]
    public class StorageEndToEndTests : EndToEndTestsBase<TestFixture>
    {
        private readonly ITestOutputHelper output;
        public StorageEndToEndTests(TestFixture fixture, ITestOutputHelper output) : base(fixture)
        {
            this.output = output;
        }

        [Fact]
        public async Task BlobTrigger_TriggerFires()
        {
            var container = Fixture.BlobClient.GetContainerReference("test");
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference("testBlob");

            await blob.UploadTextAsync("Test blob file");

            await WaitForTraceAsync("BlobTrigger", log => log.FormattedMessage.Contains("testBlob"));

            // clean up
            await blob.DeleteIfExistsAsync();

            output.WriteLine(Fixture.Host.GetLog());
        }
    }
}
