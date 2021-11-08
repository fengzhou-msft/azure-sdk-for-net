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
    public class TypesTest : TypeTestBase
    {
        public TypesTest(bool isAsync) : base(isAsync)
        {
        }
        /// <summary>
        /// Test for
        ///     Create Type Definitions
        ///     Update Atlas Type Definitions
        ///     Get Type Definition By Guid
        ///     Get Type Definition By Name
        ///     Delete Type Definitions
        ///     Test them in one test case to make sure they could work together.
        /// </summary>
        [RecordedTest]
        public async Task TypeOperations()
        {
            var client = GetTypesClient();
            //Create
            var createData = new
            {
                entityDefs = new[]
                {
                    new
                    {
                        name = "azure_sql_server_example",
                        superTypes = new[]
                        {
                            "azure_resource"
                        },
                        typeVersion = "1.0",
                        attributeDefs = new[]
                        {
                            new
                            {
                                name = "databases",
                                typeName = "array<azure_sql_db>",
                                cardinality = "SET",
                                isIndexable = false,
                                isOptional = true,
                                isUnique = false
                            }
                        }
                    }
                },
            };
            Response createResponse = await client.CreateTypeDefinitionsAsync(RequestContent.Create(createData));
            Assert.AreEqual(200, createResponse.Status);
            JsonElement createBodyJson = JsonDocument.Parse(GetContentFromResponse(createResponse)).RootElement;
            string guid = createBodyJson.GetProperty("entityDefs")[0].GetProperty("guid").GetString();
            //Update
            var updateData = new
            {
                entityDefs = new[]
                {
                    new
                    {
                        name = "azure_sql_server_example",
                        guid = guid,
                        description = "Updated Test Description",
                        superTypes = new[]
                        {
                            "azure_resource"
                        },
                        typeVersion = "1.0",
                        attributeDefs = new[]
                        {
                            new
                            {
                                name = "databases",
                                typeName = "array<azure_sql_db>",
                                cardinality = "SET",
                                isIndexable = false,
                                isOptional = true,
                                isUnique = false
                            }
                        }
                    }
                },
            };
            Response updateResponse = await client.UpdateAtlasTypeDefinitionsAsync(RequestContent.Create(updateData));
            Assert.AreEqual(200, updateResponse.Status);
            //Get
            Response getResponse = await client.GetTypeDefinitionByGuidAsync(guid, new());
            Assert.AreEqual(200, getResponse.Status);
            JsonElement getBodyJson = JsonDocument.Parse(GetContentFromResponse(getResponse)).RootElement;
            Assert.AreEqual("ENTITY", getBodyJson.GetProperty("category").GetString());
            Assert.AreEqual("Updated Test Description", getBodyJson.GetProperty("description").GetString());
            //Get by name
            Response getByNameResponse = await client.GetTypeDefinitionByNameAsync("azure_sql_server_example", new());
            Assert.AreEqual(200, getByNameResponse.Status);
            JsonElement getByNameBodyJson = JsonDocument.Parse(GetContentFromResponse(getByNameResponse)).RootElement;
            Assert.AreEqual("ENTITY", getByNameBodyJson.GetProperty("category").GetString());
            Assert.AreEqual("Updated Test Description", getByNameBodyJson.GetProperty("description").GetString());
            //Delete
            Response deleteResponse = await client.DeleteTypeDefinitionsAsync(RequestContent.Create(updateData));
            Assert.AreEqual(204, deleteResponse.Status);
        }
        [RecordedTest]
        public async Task DeleteTypeByNameAsync()
        {
            var client = GetTypesClient();
            string deleteResourceName = "azure_sql_server_example_delete";
            //Create Resource
            var createData = new
            {
                entityDefs = new[]
                {
                    new
                    {
                        name = "azure_sql_server_example_delete",
                        superTypes = new[]
                        {
                            "azure_resource"
                        },
                        typeVersion = "1.0",
                        attributeDefs = new[]
                        {
                            new
                            {
                                name = "databases",
                                typeName = "array<azure_sql_db>",
                                cardinality = "SET",
                                isIndexable = false,
                                isOptional = true,
                                isUnique = false
                            }
                        }
                    }
                },
            };
            _ =  await client.CreateTypeDefinitionsAsync(RequestContent.Create(createData));
            //Delete
            Response fetchResponse = await client.DeleteTypeByNameAsync(deleteResourceName);
            Assert.AreEqual(204, fetchResponse.Status);
        }

        [RecordedTest]
        public async Task GetAllTypeDefinitions()
        {
            var client = GetTypesClient();
            Response fetchResponse = await client.GetAllTypeDefinitionsAsync(new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.Greater(fetchBodyJson.GetProperty("enumDefs").GetArrayLength() , 1);
        }
        [RecordedTest]
        public async Task GetClassificationDefByGuid()
        {
            string guid = "1dab37e2-213f-a88d-7e13-064af9561e51";
            string name = "MICROSOFT.PERSONAL.EMAIL";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetClassificationDefByGuidAsync(guid,new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetClassificationDefByName()
        {
            string name = "MICROSOFT.PERSONAL.EMAIL";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetClassificationDefByNameAsync(name, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetEntityDefByName()
        {
            string name = "azure_mariadb_view_column";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetEntityDefinitionByNameAsync(name, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetEntityDefByGuid()
        {
            string guid = "128bd162-0c19-5441-ba2e-bb3426d148aa";
            string name = "azure_mariadb_view_column";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetEntityDefinitionByGuidAsync(guid, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetEnumDefByName()
        {
            string name = "glossary_term_status_value";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetEnumDefByNameAsync(name, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetEnumDefByGuid()
        {
            string guid = "6e30d01e-f5d0-9799-08e7-09b4f276c09c";
            string name = "glossary_term_status_value";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetEnumDefByGuidAsync(guid, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetRelationshipDefByName()
        {
            string name = "mysql_db_views";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetRelationshipDefByNameAsync(name, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetRelationshipDefByGuid()
        {
            string guid = "df32d2b1-1ed0-8fb4-c2b3-f45a5ad3b5ab";
            string name = "mysql_db_views";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetRelationshipDefByGuidAsync(guid, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetStructDefByName()
        {
            string name = "fs_permissions";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetStructDefByNameAsync(name, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetStructDefByGuid()
        {
            string guid = "f5e943b6-09dd-d672-a3a5-296546c5c163";
            string name = "fs_permissions";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetStructDefByGuidAsync(guid, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetTermTemplateDefByName()
        {
            string name = "Test_Template";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetTermTemplateDefByNameAsync(name, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetTermTemplateDefByGuid()
        {
            string guid = "e26f477f-62da-0e19-63df-ef90413d2cac";
            string name = "Test_Template";
            var client = GetTypesClient();
            Response fetchResponse = await client.GetTermTemplateDefByGuidAsync(guid, new());
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.AreEqual(name, fetchBodyJson.GetProperty("name").GetString());
        }

        [RecordedTest]
        public async Task GetTypeDefinitionHeaders()
        {
            var client = GetTypesClient();
            Response fetchResponse = await client.GetTypeDefinitionHeadersAsync(new(), true);
            Assert.AreEqual(200, fetchResponse.Status);
            JsonElement fetchBodyJson = JsonDocument.Parse(GetContentFromResponse(fetchResponse)).RootElement;
            Assert.GreaterOrEqual(fetchBodyJson.GetArrayLength(),1);
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
