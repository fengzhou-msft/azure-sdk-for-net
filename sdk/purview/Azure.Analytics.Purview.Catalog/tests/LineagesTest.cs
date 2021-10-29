// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Purview.Catalog.Tests
{
    public class LineagesTest : LineagesTestBase
    {
        public LineagesTest(bool isAsync) : base(isAsync)
        {
        }

        [RecordedTest]
        public async Task GetLineageGraph()
        {
            string guid = "b88ba3c5-ae50-4803-809e-a783cbdcbf1a";
            var client = GetLineagesClient();
            Response fetchResponse = await client.GetLineageGraphAsync(guid, "INPUT",new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(guid, fetchBodyJson.GetProperty("baseEntityGuid").GetString());
        }
        [RecordedTest]
        public async Task NextPageLineage()
        {
            string guid = "b88ba3c5-ae50-4803-809e-a783cbdcbf1a";
            var client = GetLineagesClient();
            Response fetchResponse = await client.NextPageLineageAsync(guid, "OUTPUT", new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(guid, fetchBodyJson.GetProperty("baseEntityGuid").GetString());
        }
        private static BinaryData GetContentFromResponse(Response r)
        {
            // Workaround azure/azure-sdk-for-net#21048, which prevents .Content from working when dealing with responses
            // from the playback system.

            MemoryStream ms = new MemoryStream();
            r.ContentStream.CopyTo(ms);
            return new BinaryData(ms.ToArray());
        }
    }
}
