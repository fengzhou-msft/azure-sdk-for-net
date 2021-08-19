# ReferenceTypes
### AutoRest Configuration
> see https://aka.ms/autorest

``` yaml
azure-arm: true
arm-core: true
input-file:
  - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/b60aed793a60ad3548d9f8255b7a52564ad6bcb0/specification/common-types/resource-management/v3/types.json
namespace: Azure.ResourceManager.Resources
# autorest-core version
version: 3.4.5
# save-inputs: true
csharp: true
output-folder: $(this-folder)/Generated/Resources/
public-clients: true
skip-csproj-packagereference: true
directive:
  - remove-model: "AzureEntityResource"
  - remove-model: "ProxyResource"
  - remove-model: "ResourceModelWithAllowedPropertySet"
  - remove-model: "Identity"
  - remove-model: "Operation"
  - remove-model: "OperationListResult"
  - remove-model: "OperationStatusResult"
  - remove-model: "locationData"
  - remove-model: "encryptionProperties"
  - remove-model: "KeyVaultProperties"
  - remove-model: "locationData"
  - from: types.json
    where: $.definitions['Resource']
    transform: >
      $["x-ms-mgmt-referenceType"] = true
  - from: types.json
    where: $.definitions['Resource']
    transform: >
      $["x-is-abstract"] = true
  - from: types.json
    where: $.definitions['TrackedResource']
    transform: >
      $["x-ms-mgmt-referenceType"] = true
  - from: types.json
    where: $.definitions.*
    transform: >
      $["x-ms-mgmt-propertyReferenceType"] = true
  - from: types.json
    where: $.definitions.*
    transform: >
      $["x-namespace"] = "Azure.ResourceManager.Resources.Models"
  - from: types.json
    where: $.definitions.*
    transform: >
      $["x-accessibility"] = "public"
  - from: types.json
    where: $.definitions.*
    transform: >
      $["x-csharp-formats"] = "json"
  - from: types.json
    where: $.definitions.*
    transform: >
      $["x-csharp-usage"] = "model,input,output"
  - from: types.json
    where: $.definitions.*.properties[?(@.enum)]
    transform: >
      $["x-namespace"] = "Azure.ResourceManager.Resources.Models"
  - from: types.json
    where: $.definitions.*.properties[?(@.enum)]
    transform: >
      $["x-accessibility"] = "public"
```
