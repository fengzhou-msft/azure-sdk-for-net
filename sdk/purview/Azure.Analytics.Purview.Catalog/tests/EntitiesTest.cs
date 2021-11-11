// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Analytics.Purview.Catalog.Tests
{
    public class EntitiesTest : EntitiesTestBase
    {
        public EntitiesTest(bool isAsync) : base(isAsync)
        {
        }

        [RecordedTest]
        public async Task GetByGuid()
        {
            var client = GetEntitiesClient();
            Response fetchResponse = await client.GetByGuidAsync("26fff2b7-bb60-4443-ad26-47d466de2f45", new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual("s3://testpurview/taxi-tip-data.csv", fetchBodyJson.GetProperty("entity").GetProperty("attributes").GetProperty("qualifiedName").GetString());
        }

        /// <summary>
        /// Operations for Entity
        /// </summary>
        /// <remarks>
        ///     CreateOrUpdate
        ///     GetByGuid
        ///     GetHeader
        ///     DeleteByGuid
        /// </remarks>
        [RecordedTest]
        public async Task EntityOperations()
        {
            var client = GetEntitiesClient();
            //Create
            var data = new
            {
                entity = new
                {
                    typeName = "aws_s3_v2_bucket",
                    attributes = new
                    {
                        qualifiedName = "s3://testpurview",
                        description = "test-description1020",
                        accountId = TestEnvironment.S3AccountID,
                        name = "test-datasource1020",
                        region = "US West (N. California)"
                    }
                }
            };
            Response createResponse = await client.CreateOrUpdateAsync(RequestContent.Create(data), new());
            Assert.AreEqual(200, createResponse.Status);
            JsonElement createBodyJson = JsonDocument.Parse(GetContentFromResponse(createResponse)).RootElement;
            var guidMatch = Regex.Match(createBodyJson.GetProperty("guidAssignments").GetRawText(), @"[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}");
            Assert.IsTrue(guidMatch.Success);
            //Get
            Response getResponse = await client.GetByGuidAsync(guidMatch.Value, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            Assert.AreEqual("s3://testpurview", getBodyJson.GetProperty("entity").GetProperty("attributes").GetProperty("qualifiedName").GetString());
            //GetHeader
            Response getHeaderResponse = await client.GetHeaderAsync(guidMatch.Value,new());
            Assert.AreEqual(200, getHeaderResponse.Status);
            JsonElement getHeaderBodyJson = JsonDocument.Parse(GetContentFromResponse(getHeaderResponse)).RootElement;
            Assert.AreEqual("s3://testpurview", getHeaderBodyJson.GetProperty("attributes").GetProperty("qualifiedName").GetString());
            //Delete
            Response deleteResponse = await client.DeleteByGuidAsync(guidMatch.Value);
            Assert.AreEqual(200, deleteResponse.Status);
        }

        [RecordedTest]
        public async Task ClassificationOperations()
        {
            string guid = "07bccfca-2aba-4c83-bc07-dceb7e77dd31";
            string typeName = "test_classification1025";
            var client = GetEntitiesClient();
            //Add
            var data = new
            {
                classification = new
                {
                    typeName = typeName
                },
                entityGuids = new[]
                {
                    guid
                }
            };
            Response createResponse = await client.AddClassificationAsync(RequestContent.Create(data));
            Assert.AreEqual(204, createResponse.Status);
            //Get
            Response getResponse = await client.GetClassificationAsync(guid, typeName, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            Assert.AreEqual(getBodyJson.GetProperty("entityStatus").GetString(), "ACTIVE");
            //Delete
            Response deleteResponse = await client.DeleteClassificationAsync(guid, typeName);
            Assert.AreEqual(204, deleteResponse.Status);
        }
        [RecordedTest]
        public async Task ClassificationsOperations()
        {
            string guid = "e6d790c3-93b2-4e5c-a5b6-3d711a440f79";
            string qualifiedName = "s3://testpurview/localcsvfile.csv";
            string objectType = "aws_s3_v2_object";
            var client = GetEntitiesClient();
            //Add
            var data = new[]
            {
                new
                {
                    typeName = "MICROSOFT.FINANCIAL.US.ABA_ROUTING_NUMBER",
                    entityGuid = guid
                },
                new
                {
                    typeName = "MICROSOFT.FINANCIAL.CREDIT_CARD_NUMBER",
                    entityGuid = guid
                }
            };
            Response createResponse = await client.AddClassificationsAsync(guid,RequestContent.Create(data));
            Assert.AreEqual(204, createResponse.Status);
            //Update
            var updateData = new[]
            {
                new
                {
                    typeName = "MICROSOFT.FINANCIAL.US.ABA_ROUTING_NUMBER",
                    entityGuid = guid,
                    entityStatus = "ACTIVE"
                }
            };
            Response updateResponse = await client.UpdateClassificationsAsync(guid, RequestContent.Create(updateData));
            Assert.AreEqual(204, updateResponse.Status);
            //Update by unique attribute
            var updateByAttributeData = new[]
            {
                new
                {
                    typeName = "MICROSOFT.FINANCIAL.US.ABA_ROUTING_NUMBER",
                    entityGuid = guid
                }
            };
            Response updateByAttributeResponse = await client.UpdateClassificationsByUniqueAttributeAsync(objectType, RequestContent.Create(updateByAttributeData), qualifiedName);
            Assert.AreEqual(204, updateByAttributeResponse.Status);
            //Get
            Response getResponse = await client.GetClassificationsAsync(guid, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            //One assigned by system
            Assert.AreEqual(3, getBodyJson.GetProperty("list").GetArrayLength());
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
