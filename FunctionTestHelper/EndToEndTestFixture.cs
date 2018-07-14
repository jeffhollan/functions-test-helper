// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;

namespace FunctionTestHelper
{
    public abstract class EndToEndTestFixture : IDisposable
    {
        private string _copiedRootPath;

        protected EndToEndTestFixture(string rootPath, string testId, string extensionName = null, string extensionVersion = null)
        {
            FixtureId = testId;

            _copiedRootPath = Path.Combine(Path.GetTempPath(), "FunctionsE2E", DateTime.UtcNow.ToString("yyMMdd-HHmmss"));
            FileUtility.CopyDirectory(rootPath, _copiedRootPath);

            // Allow derived classes to limit functions. We'll update host.json in the copied location
            // so it only affects this fixture's instance.
            IEnumerable<string> functions = GetActiveFunctions();
            string hostJsonPath = Path.Combine(_copiedRootPath, "host.json");
            JObject hostJson = JObject.Parse(File.ReadAllText(hostJsonPath));
            if (functions != null && functions.Any())
            {
                hostJson["functions"] = JArray.FromObject(functions);
            }
            File.WriteAllText(hostJsonPath, hostJson.ToString());

            UpdateEnvironmentVariables(Path.Combine(_copiedRootPath, "local.settings.json"));

            string connectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.Storage);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            QueueClient = storageAccount.CreateCloudQueueClient();
            BlobClient = storageAccount.CreateCloudBlobClient();
            TableClient = storageAccount.CreateCloudTableClient();

            Host = new TestFunctionHost(_copiedRootPath);

            // We can currently only support a single extension.
            if (extensionName != null && extensionVersion != null)
            {
                Host.SetNugetPackageSources("http://www.myget.org/F/azure-appservice/api/v2", "https://api.nuget.org/v3/index.json");
                Host.InstallBindingExtension(extensionName, extensionVersion).Wait(TimeSpan.FromSeconds(30));
            }

            Host.StartAsync().Wait(TimeSpan.FromSeconds(30));
        }

        private void UpdateEnvironmentVariables(string secretsPath)
        {
            var localSettings = File.ReadAllText(secretsPath);
            JObject settingValues = JObject.Parse(localSettings)["Values"] as JObject;
            foreach (var secret in settingValues)
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(secret.Key)))
                {
                    Environment.SetEnvironmentVariable(secret.Key, (string)secret.Value, EnvironmentVariableTarget.Process);
                }
            }
        }

        public CloudQueueClient QueueClient { get; private set; }

        public CloudTableClient TableClient { get; private set; }

        public CloudBlobClient BlobClient { get; private set; }

        public TestFunctionHost Host { get; private set; }

        public string FixtureId { get; private set; }

        /// <summary>
        /// Override this to set the list of functions to write to host.json.
        /// </summary>
        /// <returns>The list of enabled functions.</returns>
        protected virtual IEnumerable<string> GetActiveFunctions()
        {
            return Enumerable.Empty<string>();
        }



        public virtual void Dispose()
        {
            Host?.Dispose();

            // Clean up all but the last 5 directories for debugging failures.
            var directoriesToDelete = Directory.EnumerateDirectories(Path.GetDirectoryName(_copiedRootPath))
                .OrderByDescending(p => p)
                .Skip(5);

            foreach (string directory in directoriesToDelete)
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch
                {
                    // best effort
                }
            }
        }
    }
}