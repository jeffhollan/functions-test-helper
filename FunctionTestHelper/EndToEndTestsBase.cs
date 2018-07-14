// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FunctionTestHelper
{
    public abstract class EndToEndTestsBase<TTestFixture> :
        IClassFixture<TTestFixture> where TTestFixture : EndToEndTestFixture, new()
    {
        private INameResolver _nameResolver = new DefaultNameResolver();
        private static readonly ScriptSettingsManager SettingsManager = ScriptSettingsManager.Instance;

        public EndToEndTestsBase(TTestFixture fixture)
        {
            Fixture = fixture;
        }

        protected TTestFixture Fixture { get; private set; }
        protected async Task<JObject> GetFunctionTestResult(string functionName)
        {
            string logEntry = null;

            await TestHelpers.Await(() =>
           {
               // search the logs for token "TestResult:" and parse the following JSON
               var logs = Fixture.Host.GetLogMessages(LogCategories.CreateFunctionUserCategory(functionName));
               if (logs != null)
               {
                   logEntry = logs.Select(p => p.FormattedMessage).SingleOrDefault(p => p != null && p.Contains("TestResult:"));
               }
               return logEntry != null;
           });

            int idx = logEntry.IndexOf("{");
            logEntry = logEntry.Substring(idx);

            return JObject.Parse(logEntry);
        }

        protected async Task<LogMessage> WaitForTraceAsync(string functionName, Func<LogMessage, bool> filter, int timeout)
        {
            LogMessage logMessage = null;

            await TestHelpers.Await(() =>
            {
                logMessage = Fixture.Host.GetLogMessages(LogCategories.CreateFunctionUserCategory(functionName)).SingleOrDefault(filter);
                return logMessage != null;
            }, timeout);

            return logMessage;
        }

        protected async Task<LogMessage> WaitForTraceAsync(string functionName, Func<LogMessage, bool> filter)
        {
            return await WaitForTraceAsync(functionName, filter, 30000);
        }

        protected async Task<LogMessage> WaitForTraceAsync(Func<LogMessage, bool> filter)
        {
            LogMessage logMessage = null;

            await TestHelpers.Await(() =>
            {
                logMessage = Fixture.Host.GetLogMessages().SingleOrDefault(filter);
                return logMessage != null;
            });

            return logMessage;
        }

    }
}