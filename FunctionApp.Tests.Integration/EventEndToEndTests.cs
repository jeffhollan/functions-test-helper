using FunctionTestHelper;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FunctionApp.Tests.Integration
{
    [Collection("Function collection")]
    public class EventEndToEndTests : EndToEndTestsBase<TestFixture>
    {
        private readonly ITestOutputHelper output;
        public EventEndToEndTests(TestFixture fixture, ITestOutputHelper output) : base(fixture)
        {
            this.output = output;
        }

        [Fact]
        public async Task EventHub_TriggerFires()
        {
            List<EventData> events = new List<EventData>();
            string[] ids = new string[3];
            for (int i = 0; i < 3; i++)
            {
                ids[i] = Guid.NewGuid().ToString();
                JObject jo = new JObject
                {
                    { "value", ids[i] }
                };
                var evt = new EventData(Encoding.UTF8.GetBytes(jo.ToString(Formatting.None)));
                evt.Properties.Add("TestIndex", i);
                events.Add(evt);
            }

            string connectionString = Environment.GetEnvironmentVariable("EventHubsConnectionString");
            EventHubsConnectionStringBuilder builder = new EventHubsConnectionStringBuilder(connectionString);

            if (string.IsNullOrWhiteSpace(builder.EntityPath))
            {
                string eventHubPath = "test";
                builder.EntityPath = eventHubPath;
            }

            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(builder.ToString());
            try
            {
                await eventHubClient.SendAsync(events);

                await WaitForTraceAsync("EventHubTrigger", log => log.FormattedMessage.Contains(ids[2]));
                output.WriteLine(Fixture.Host.GetLog());
            }
            catch(Exception ex)
            {
                output.WriteLine(Fixture.Host.GetLog());
                throw ex;
            }
        }
    }
}
