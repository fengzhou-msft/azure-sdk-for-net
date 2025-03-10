// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace Azure.AI.MetricsAdvisor.Models
{
    internal partial class ServicePrincipalInKVParam : IUtf8JsonSerializable
    {
        void IUtf8JsonSerializable.Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("keyVaultEndpoint");
            writer.WriteStringValue(KeyVaultEndpoint);
            writer.WritePropertyName("keyVaultClientId");
            writer.WriteStringValue(KeyVaultClientId);
            writer.WritePropertyName("keyVaultClientSecret");
            writer.WriteStringValue(KeyVaultClientSecret);
            writer.WritePropertyName("servicePrincipalIdNameInKV");
            writer.WriteStringValue(ServicePrincipalIdNameInKV);
            writer.WritePropertyName("servicePrincipalSecretNameInKV");
            writer.WriteStringValue(ServicePrincipalSecretNameInKV);
            writer.WritePropertyName("tenantId");
            writer.WriteStringValue(TenantId);
            writer.WriteEndObject();
        }

        internal static ServicePrincipalInKVParam DeserializeServicePrincipalInKVParam(JsonElement element)
        {
            string keyVaultEndpoint = default;
            string keyVaultClientId = default;
            string keyVaultClientSecret = default;
            string servicePrincipalIdNameInKV = default;
            string servicePrincipalSecretNameInKV = default;
            string tenantId = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("keyVaultEndpoint"))
                {
                    keyVaultEndpoint = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("keyVaultClientId"))
                {
                    keyVaultClientId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("keyVaultClientSecret"))
                {
                    keyVaultClientSecret = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("servicePrincipalIdNameInKV"))
                {
                    servicePrincipalIdNameInKV = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("servicePrincipalSecretNameInKV"))
                {
                    servicePrincipalSecretNameInKV = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("tenantId"))
                {
                    tenantId = property.Value.GetString();
                    continue;
                }
            }
            return new ServicePrincipalInKVParam(keyVaultEndpoint, keyVaultClientId, keyVaultClientSecret, servicePrincipalIdNameInKV, servicePrincipalSecretNameInKV, tenantId);
        }
    }
}
