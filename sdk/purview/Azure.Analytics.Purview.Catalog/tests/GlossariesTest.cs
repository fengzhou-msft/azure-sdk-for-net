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
    public class GlossariesTest : GlossariesTestBase
    {
        public GlossariesTest(bool isAsync) : base(isAsync)
        {
        }

        /// <summary>
        /// Operations for Glossary
        /// </summary>
        /// <remarks>
        ///     CreateGlossary
        ///     UpdateGlossary
        ///     PartialUpdateGlossary
        ///     GetGlossary
        ///     GetDetailedGlossary
        ///     DeleteGlossary
        /// </remarks>
        [RecordedTest]
        public async Task GlossaryOperations()
        {
            var client = GetGlossariesClient();

            //Create
            var createData = new
            {
                name = "GlossaryTest1025",
                shortDescription = "Example Short Description",
                longDescription = "Example Long Description",
                language = "en",
                usage = "Example Glossary"
            };
            Response createResponse = await client.CreateGlossaryAsync(RequestContent.Create(createData));
            Assert.AreEqual(200, createResponse.Status);
            JsonElement createBodyJson = JsonDocument.Parse(GetContentFromResponse(createResponse)).RootElement;
            Assert.AreEqual("GlossaryTest1025", createBodyJson.GetProperty("name").GetString());
            string guid = createBodyJson.GetProperty("guid").GetString();

            //Update
            var updateData = new
            {
                name = "GlossaryTest1025",
                longDescription = "Updated Long Description"
            };
            Response updateResponse = await client.UpdateGlossaryAsync(guid, RequestContent.Create(updateData));
            Assert.AreEqual(200, updateResponse.Status);

            //Update(partial)
            var partialUpdateData = new
            {
                shortDescription = "Updated Short Description"
            };
            Response partialUpdateResponse = await client.PartialUpdateGlossaryAsync(guid, RequestContent.Create(partialUpdateData));
            Assert.AreEqual(200, partialUpdateResponse.Status);

            //Get
            Response getResponse = await client.GetGlossaryAsync(guid, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            Assert.AreEqual("Updated Short Description", getBodyJson.GetProperty("shortDescription").GetString());
            Assert.AreEqual("Updated Long Description", getBodyJson.GetProperty("longDescription").GetString());
            //Get Detail
            Response getDetailResponse = await client.GetDetailedGlossaryAsync(guid, new());
            Assert.AreEqual(200, getDetailResponse.Status);
            JsonElement getDetailBodyJson = JsonDocument.Parse(GetContentFromResponse(getDetailResponse)).RootElement;
            Assert.AreEqual("Updated Short Description", getDetailBodyJson.GetProperty("shortDescription").GetString());
            Assert.AreEqual("Updated Long Description", getDetailBodyJson.GetProperty("longDescription").GetString());
            //Delete
            Response deleteResponse = await client.DeleteGlossaryAsync(guid);
            Assert.AreEqual(204, deleteResponse.Status);
        }

        /// <summary>
        /// Operations for Glossary Term
        /// </summary>
        /// <remarks>
        ///     CreateGlossaryTerm
        ///     UpdateGlossaryTerm
        ///     PartialUpdateGlossaryTerm
        ///     GetGlossaryTerm
        ///     DeleteGlossaryTerm
        /// </remarks>
        [RecordedTest]
        public async Task GlossaryTermOperations()
        {
            var client = GetGlossariesClient();
            string glossaryGUID = "c9843adb-8d1f-439d-a834-3a44943aa664";
            //Create
            var createData = new
            {
                name = "TestGlossaryTerm",
                shortDescription = "Example Short Description",
                longDescription = "Example Long Description",
                status = "Approved",
                anchor = new
                {
                    glossaryGuid = glossaryGUID,
                }
            };
            Response createResponse = await client.CreateGlossaryTermAsync(RequestContent.Create(createData));
            Assert.AreEqual(200, createResponse.Status);
            JsonElement createBodyJson = JsonDocument.Parse(GetContentFromResponse(createResponse)).RootElement;
            Assert.AreEqual("TestGlossaryTerm", createBodyJson.GetProperty("name").GetString());
            string guid = createBodyJson.GetProperty("guid").GetString();

            //Update
            var updateData = new
            {
                name = "TestGlossaryTerm",
                shortDescription = "Updated Short Description",
                status = "Approved",
                anchor = new
                {
                    glossaryGuid = glossaryGUID
                }
            };
            Response updateResponse = await client.UpdateGlossaryTermAsync(guid, RequestContent.Create(updateData));
            Assert.AreEqual(200, updateResponse.Status);

            //Update(partial)
            var partialUpdateData = new
            {
                longDescription = "Updated Long Description"
            };
            Response partialUpdateResponse = await client.PartialUpdateGlossaryTermAsync(guid, RequestContent.Create(partialUpdateData));
            Assert.AreEqual(200, partialUpdateResponse.Status);

            //Get
            Response getResponse = await client.GetGlossaryTermAsync(guid, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            Assert.AreEqual("Updated Short Description",getBodyJson.GetProperty("shortDescription").GetString());
            Assert.AreEqual("Updated Long Description", getBodyJson.GetProperty("longDescription").GetString());

            //Delete
            Response deleteResponse = await client.DeleteGlossaryTermAsync(guid);
            Assert.AreEqual(204, deleteResponse.Status);
        }

        /// <summary>
        /// Operations for Glossary Category
        /// </summary>
        /// <remarks>
        ///     CreateGlossaryCategory
        ///     UpdateGlossaryCategory
        ///     PartialUpdateGlossaryCategory
        ///     GetGlossaryCategory
        ///     DeleteGlossaryCategory
        /// </remarks>
        [RecordedTest]
        public async Task GlossaryCategoryOperations()
        {
            var client = GetGlossariesClient();
            string glossaryGUID = "984848ac-78ec-409c-8f64-8acf4870ebf5";
            string glossaryTermGUID = "5d84a98e-65eb-4aa5-92f1-cf9954008bbf";
            //Create
            var createData = new
            {
                name = "GlossaryCategoryTest",
                shortDescription = "Example Short Description",
                anchor = new
                {
                    glossaryGuid = glossaryGUID
                },
                terms = new[]
                {
                    new {
                        termGuid = glossaryTermGUID
                    }
                }
            };
            Response createResponse = await client.CreateGlossaryCategoryAsync(RequestContent.Create(createData));
            Assert.AreEqual(200, createResponse.Status);
            JsonElement createBodyJson = JsonDocument.Parse(GetContentFromResponse(createResponse)).RootElement;
            string guid = createBodyJson.GetProperty("guid").GetString();

            //Update
            var updateData = new
            {
                name = "GlossaryCategoryTest",
                shortDescription = "Updated Short Description",
                anchor = new
                {
                    glossaryGuid = glossaryGUID
                },
                terms = new[]
                {
                    new
                    {
                        termGuid = glossaryTermGUID
                    }
                }
            };
            Response updateResponse = await client.UpdateGlossaryCategoryAsync(guid, RequestContent.Create(updateData));
            Assert.AreEqual(200, updateResponse.Status);

            //Update(partial)
            var partialUpdateData = new
            {
                longDescription = "Updated Long Description",
            };
            Response partialUpdateResponse = await client.PartialUpdateGlossaryCategoryAsync(guid, RequestContent.Create(partialUpdateData));
            Assert.AreEqual(200, partialUpdateResponse.Status);

            //Get
            Response getResponse = await client.GetGlossaryCategoryAsync(guid, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            Assert.AreEqual("Updated Short Description", getBodyJson.GetProperty("shortDescription").GetString());
            Assert.AreEqual("Updated Long Description", getBodyJson.GetProperty("longDescription").GetString());
            //Delete
            Response deleteResponse = await client.DeleteGlossaryCategoryAsync(guid, new());
            Assert.AreEqual(204, deleteResponse.Status);
        }

        [RecordedTest]
        public async Task GetCategoryTerms()
        {
            string categoryGuid = "b9014169-51b1-40b9-8501-fe53aba0cf6d";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetCategoryTermsAsync(categoryGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task GetCategoryTermsHeaders()
        {
            string categoryGuid = "b9014169-51b1-40b9-8501-fe53aba0cf6d";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetCategoryTermsAsync(categoryGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task GetRelatedCategories()
        {
            string categoryGuid = "cece2ead-253d-4648-be41-e3a4158f9f37";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetRelatedCategoriesAsync(categoryGuid);
            Assert.AreEqual(200, fetchResponse.Status);
        }

        [RecordedTest]
        public async Task GetRelatedTerms()
        {
            string termGuid = "1736c1f2-c072-4fee-99a1-eb47d156bb12";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetRelatedTermsAsync(termGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetProperty("seeAlso").GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task GetGlossaries()
        {
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetGlossariesAsync();
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(2, fetchBodyJson.GetArrayLength());
            JsonElement glossaryItemJson = fetchBodyJson[0];
            Assert.AreEqual("Glossary", glossaryItemJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetGlossaryTerms()
        {
            string termGuid = "1736c1f2-c072-4fee-99a1-eb47d156bb12";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetGlossaryTermAsync(termGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetProperty("synonyms").GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task AssignTermToEntites()
        {
            string termGuid = "26229cd6-0779-4d65-abee-f37a0d610a20";
            var data = new[]
            {
                new
                {
                    guid = "bf77965b-b260-43cd-b753-f5d58b362834"
                }
            };
            var client = GetGlossariesClient();
            Response fetchResponse = await client.AssignTermToEntitiesAsync(termGuid, RequestContent.Create(data));
            Assert.AreEqual(204, fetchResponse.Status);
        }
        [RecordedTest]
        public async Task GetEntitesAssignedWithTerm()
        {
            string termGuid = "53a479f5-81bb-4305-9fe0-93af59a72a0b";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetEntitiesAssignedWithTermAsync(termGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task GetGlossaryTermsByGlossaryName()
        {
            string glossaryName = "Glossary";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetTermsByGlossaryNameAsync(glossaryName);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task GetGlossaryTermsHeaders()
        {
            string glossaryGuid = "ba730fdb-0283-491a-8ec6-ec4fe4b0da6a";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetGlossaryTermHeadersAsync(glossaryGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task GetGlossaryCategories()
        {
            string glossaryGuid = "c9843adb-8d1f-439d-a834-3a44943aa664";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetGlossaryCategoriesAsync(glossaryGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(), 1);
        }
        [RecordedTest]
        public async Task GetGlossaryCategoriesHeaders()
        {
            string glossaryGuid = "c9843adb-8d1f-439d-a834-3a44943aa664";
            var client = GetGlossariesClient();
            Response fetchResponse = await client.GetGlossaryCategoriesHeadersAsync(glossaryGuid);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(), 1);
        }
        //[RecordedTest]
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
