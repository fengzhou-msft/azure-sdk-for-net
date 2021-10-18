// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Purview.Catalog.Tests
{
    public class GlossaryClientTest : GlossaryClientTestBase
    {
        public GlossaryClientTest(bool isAsync) : base(isAsync)
        {
        }

        [RecordedTest]
        public async Task GetGlossaryCategory()
        {
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetGlossariesAsync(new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(1, fetchBodyJson.GetArrayLength());
            JsonElement glossaryItemJson = fetchBodyJson[0];
            Assert.AreEqual("Glossary", glossaryItemJson.GetProperty("name").GetString());
        }
        [RecordedTest]
        public async Task ImportGlossaryTermsViaCsv()
        {
            var client = GetGlossariesClient();
            var data = new JsonData(new Dictionary<string, string>
            {
                ["file"] = "\"Name\",\"Nick Name\",\"Status\",\"Definition\",\"Acronym\",\"Resources\",\"Related Terms\",\"Synonyms\",\"Stewards\",\"Experts\",\"Parent Term Name\"\n\"Parent Name 1_Parent Name 2_Term Name 1\",\"Term Name 1\",\"Draft\",\"Definition of Term Name 1\",\"TN1\",\"Purview Project:https://web.purview.azure.com;Azure portal:https://portal.azure.com;\",\"Term Name 4;Parent Term 2_Term Name 5;\",\"Term Name 2;Parent Name 1_Term Name 3;\",\"Feng.Zhou@microsoft.com\",\"Feng.Zhou@microsoft.com\",\"Parent Name 1_Parent Name 2\""
            });
            Operation<BinaryData> resultOperation = await client.ImportGlossaryTermsViaCsvAsync("6c57d99a-ddac-4f2a-bf8c-e55c5a7070d7", RequestContent.Create(data));
            Response<BinaryData> fetchResponse = await resultOperation.WaitForCompletionAsync();
            JsonElement fetchBodyJson = JsonDocument.Parse(fetchResponse.Value).RootElement;
            Assert.AreEqual("RUNNING", fetchBodyJson.GetProperty("status").GetString());
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
