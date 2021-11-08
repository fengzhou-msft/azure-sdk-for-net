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
    public class RelationshipsTest : RelationshipsTestBase
    {
        public RelationshipsTest(bool isAsync) : base(isAsync)
        {
        }

        [RecordedTest]
        public async Task RelationshipsOperations()
        {
            string glossaryTermResource1GUID = "aab4d687-59f2-44d8-ae7a-f5d6155a2fb0";
            string glossaryTermResource2GUID = "8a825468-d64e-4ecf-a416-c03fd9a14762";
            var client = GetRelationshipsClient();
            //Create
            var createData = new
            {
                typeName = "AtlasGlossarySynonym",
                attributes = new
                {
                    description = "Test description"
                },
                end1 = new
                {
                    guid = glossaryTermResource1GUID,
                    typeName = "AtlasGlossaryTerm"
                },
                end2 = new
                {
                    guid = glossaryTermResource2GUID,
                    typeName = "AtlasGlossaryTerm"
                },
                status = "ACTIVE"
            };
            Response createResponse = await client.CreateAsync(RequestContent.Create(createData));
            Assert.AreEqual(200, createResponse.Status);
            JsonElement createBodyJson = JsonDocument.Parse(GetContentFromResponse(createResponse)).RootElement;
            string guid = createBodyJson.GetProperty("guid").GetString();
            //Update
            var updateData = new
            {
                typeName = "AtlasGlossarySynonym",
                attributes = new
                {
                    description = "Updated Test description"
                },
                guid = guid,
                end1 = new
                {
                    guid = glossaryTermResource1GUID,
                    typeName = "AtlasGlossaryTerm"
                },
                end2 = new
                {
                    guid = glossaryTermResource2GUID,
                    typeName = "AtlasGlossaryTerm"
                },
                status = "ACTIVE"
            };
            Response updateResponse = await client.UpdateAsync(RequestContent.Create(updateData));
            Assert.AreEqual(200, updateResponse.Status);
            //Get
            Response getResponse = await client.GetAsync(guid, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            Assert.AreEqual("Updated Test description", getBodyJson.GetProperty("relationship").GetProperty("attributes").GetProperty("description").GetString());
            //Delete
            Response deleteResponse = await client.DeleteAsync(guid);
            Assert.AreEqual(204, deleteResponse.Status);
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
