using FunctionTestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


namespace FunctionApp.Tests
{
    public class EventHubFunction : FunctionTest
    {
        [Fact]
        public void EventHubFunction_ValidArray()
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
            EventHubTrigger.Run(events.ToArray(), log: log);
        }
    }
}
