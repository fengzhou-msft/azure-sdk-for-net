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
    public class CollectionTest : CollectionTestBase
    {
        public CollectionTest(bool isAsync) : base(isAsync)
        {
        }

        [RecordedTest]
        public async Task CreateOrUpdateEntity()
        {
            var client = GetCollectionsClient();
            var data = new
            {
                entity = new
                {
                    typeName = "aws_s3_v2_bucket",
                    attributes = new
                    {
                        name = "test-datasource1019v2",
                        qualifiedName = "s3://testpurview"
                    }
                }
            };
            Response fetchResponse = await client.CreateOrUpdateEntityAsync("dotnetLLCPurviewAccount", RequestContent.Create(data));
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual("s3://testpurview", fetchBodyJson.GetProperty("mutatedEntities").GetProperty("CREATE")[0].GetProperty("attributes").GetProperty("qualifiedName").GetString());
        }

        [RecordedTest]
        public async Task CreateOrUpdateEntityInBulk()
        {
            var client = GetCollectionsClient();
            var data = new
            {
                entities = new[]
                {
                    new
                    {
                        typeName = "aws_s3_v2_bucket",
                        attributes = new
                        {
                            name = "test-datasource1019v2",
                            qualifiedName = "s3://testpurview",
                            description = "test-descriptionv2"
                        }
                    }
                }
            };
            Response fetchResponse = await client.CreateOrUpdateEntityInBulkAsync("dotnetLLCPurviewAccount", RequestContent.Create(data));
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual("s3://testpurview", fetchBodyJson.GetProperty("mutatedEntities").GetProperty("UPDATE")[0].GetProperty("attributes").GetProperty("qualifiedName").GetString());
        }

        [RecordedTest]
        public async Task MoveEntitiesToCollection()
        {
            var client = GetCollectionsClient();
            var data = new
            {
                entityGuids = new string[]
                {
                    "91cec3bb-0f91-470a-9f3b-f7f1d1196cde"
                }
            };
            Response fetchResponse = await client.MoveEntitiesToCollectionAsync("nhx5gh", RequestContent.Create(data));
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(200, fetchResponse.Status);
            Assert.AreEqual("s3://testpurview", fetchBodyJson.GetProperty("mutatedEntities").GetProperty("UPDATE")[0].GetProperty("attributes").GetProperty("qualifiedName").GetString());
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
