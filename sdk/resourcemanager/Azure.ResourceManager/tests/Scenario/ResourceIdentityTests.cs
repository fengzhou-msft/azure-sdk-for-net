using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.TestFramework;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using NUnit.Framework;

namespace Azure.ResourceManager.Tests
{
    public class ResourceIdentityTests : ResourceManagerTestBase
    {
        private const string dummySSHKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQC+wWK73dCr+jgQOAxNsHAnNNNMEMWOHYEccp6wJm2gotpr9katuF/ZAdou5AaW1C61slRkHRkpRRX9FA9CYBiitZgvCCz+3nWNN7l/Up54Zps/pHWGZLHNJZRYyAB6j5yVLMVHIHriY49d/GZTZVNB8GoJv9Gakwc/fuEZYYl4YDFiGMBP///TzlI4jhiJzjKnEvqPFki5p2ZRJqcbCiF4pJrxUQR/RXqVFQdbRLZgYfJ8xGB878RENq3yQ39d8dVOkq4edbkzwcUmwwwkYVPIoDGsYLaRHnG+To7FvMeyO7xDVQkMKzopTQV8AuKpyvpqu0a9pWOMaiCyDytO7GGN you@me.com";
        protected AzureLocation DefaultLocation => AzureLocation.WestUS2;
        protected Subscription _subscription;
        protected ResourceGroup _resourceGroup;
        protected GenericResourceCollection _genericResourceCollection;

        public ResourceIdentityTests(bool isAsync)
            : base(isAsync)//, RecordedTestMode.Record)
        {
        }

        [TestCase]
        [RecordedTest]
        [AsyncOnly]
        public async Task CreateOrUpdate()
        {
            _subscription = await Client.GetDefaultSubscriptionAsync();
            _genericResourceCollection = Client.GetGenericResources();
            _resourceGroup = await CreateResourceGroupAsync();
            var userAssignedIdentity1 = await CreateUserAssignedIdentityAsync();
            var userAssignedIdentity2 = await CreateUserAssignedIdentityAsync();
            var vmCollection = _resourceGroup.GetVirtualMachines();
            var vmName = Recording.GenerateAssetName("testVM-");
            var nic = await CreateBasicDependenciesOfVirtualMachineAsync();
            var input = GetBasicLinuxVirtualMachineData(DefaultLocation, vmName, nic.Id);
            // Create with System Assigned Identity
            input.Identity = new ResourceIdentity(null, true);
            var lro = await vmCollection.CreateOrUpdateAsync(vmName, input);
            // Add two User Assigned Identities
            VirtualMachine vm = lro.Value;
            var vmId = vm.Data.Id;
            var userAssignedIdentities = new Dictionary<ResourceIdentifier, UserAssignedIdentity>{
                {userAssignedIdentity1.Id, new UserAssignedIdentity()},
                {userAssignedIdentity2.Id, new UserAssignedIdentity()}
            };
            Assert.NotNull(vm.Data.Identity.SystemAssignedIdentity);
            Assert.IsEmpty(vm.Data.Identity.UserAssignedIdentities);
            VirtualMachineUpdate options = new VirtualMachineUpdate(){
                Identity = new ResourceIdentity(userAssignedIdentities, vm.Data.Identity.SystemAssignedIdentity != null)
            };
            var updatedResponse = await vm.UpdateAsync(options);
            vm = updatedResponse.Value;
            Assert.NotNull(vm.Data.Identity.SystemAssignedIdentity);
            Assert.IsTrue(vm.Data.Identity.UserAssignedIdentities.ContainsKey(userAssignedIdentity1.Id));
            Assert.IsTrue(vm.Data.Identity.UserAssignedIdentities.ContainsKey(userAssignedIdentity2.Id));
            //Delete User Assigned Identity 1
            userAssignedIdentities = new Dictionary<ResourceIdentifier, UserAssignedIdentity>{
                {userAssignedIdentity1.Id, null}
            };
            options = new VirtualMachineUpdate(){
                Identity = new ResourceIdentity(userAssignedIdentities, vm.Data.Identity.SystemAssignedIdentity != null)
            };
            updatedResponse = await vm.UpdateAsync(options);
            vm = updatedResponse.Value;
            Assert.NotNull(vm.Data.Identity.SystemAssignedIdentity);
            Assert.NotNull(vm.Data.Identity.UserAssignedIdentities);
            Assert.IsFalse(vm.Data.Identity.UserAssignedIdentities.ContainsKey(userAssignedIdentity1.Id));
            Assert.IsTrue(vm.Data.Identity.UserAssignedIdentities.ContainsKey(userAssignedIdentity2.Id));
            //Delete User Assigned Identity 2
            userAssignedIdentities = new Dictionary<ResourceIdentifier, UserAssignedIdentity>{
                {userAssignedIdentity2.Id, null}
            };
            options = new VirtualMachineUpdate(){
                Identity = new ResourceIdentity(userAssignedIdentities, vm.Data.Identity.SystemAssignedIdentity != null)
            };
            RequestFailedException exception = Assert.ThrowsAsync<RequestFailedException>(async () => await vm.UpdateAsync(options));
            Assert.AreEqual(400, exception.Status);
            //Assert.IsTrue(exception.Message.Contains($"Removal of all user-assigned identities assigned to resource '{vmId}' with type 'SystemAssigned, UserAssigned' is invalid."));
            Assert.IsTrue(exception.Message.Contains($"Removal of all user-assigned identities assigned to resource") && exception.Message.Contains("with type 'SystemAssigned, UserAssigned' is invalid."));
            //vm = updatedResponse.Value;
            //Assert.NotNull(vm.Data.Identity.SystemAssignedIdentity);
            //Assert.IsEmpty(vm.Data.Identity.UserAssignedIdentities);
        }

        protected async Task<GenericResource> CreateUserAssignedIdentityAsync()
        {
            string userAssignedIdentityName = Recording.GenerateAssetName("testRi-");
            ResourceIdentifier userIdentityId = new ResourceIdentifier($"{_resourceGroup.Id}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/{userAssignedIdentityName}");
            var input = new GenericResourceData(DefaultLocation);
            var response = await _genericResourceCollection.CreateOrUpdateAsync(userIdentityId, input);
            return response.Value;
        }

        protected async Task<ResourceGroup> CreateResourceGroupAsync()
        {
            var resourceGroupName = Recording.GenerateAssetName("testRG-");
            var rgOp = await _subscription.GetResourceGroups().CreateOrUpdateAsync(
                resourceGroupName,
                new ResourceGroupData(DefaultLocation)
                {
                    Tags =
                    {
                        { "test", "env" }
                    }
                });
            return rgOp.Value;
        }

        protected async Task<GenericResource> CreateVirtualNetwork()
        {
            var vnetName = Recording.GenerateAssetName("testVNet-");
            var subnetName = Recording.GenerateAssetName("testSubnet-");
            ResourceIdentifier vnetId = new ResourceIdentifier($"{_resourceGroup.Id}/providers/Microsoft.Network/virtualNetworks/{vnetName}");
            var addressSpaces = new Dictionary<string, object>()
            {
                { "addressPrefixes", new List<string>() { "10.0.0.0/16" } }
            };
            var subnet = new Dictionary<string, object>()
            {
                { "name", subnetName },
                { "properties", new Dictionary<string, object>()
                {
                    { "addressPrefix", "10.0.2.0/24" }
                } }
            };
            var subnets = new List<object>() { subnet };
            var input = new GenericResourceData(DefaultLocation)
            {
                Properties = new Dictionary<string, object>()
                {
                    { "addressSpace", addressSpaces },
                    { "subnets", subnets }
                }
            };
            var operation = await _genericResourceCollection.CreateOrUpdateAsync(vnetId, input);
            return operation.Value;
        }

        protected ResourceIdentifier GetSubnetId(GenericResource vnet)
        {
            var properties = vnet.Data.Properties as IDictionary<string, object>;
            var subnets = properties["subnets"] as IEnumerable<object>;
            var subnet = subnets.First() as IDictionary<string, object>;
            return new ResourceIdentifier(subnet["id"] as string);
        }

        // WEIRD: second level resources cannot use GenericResourceCollection to create.
        // Exception thrown: System.InvalidOperationException : An invalid resource id was given /subscriptions/db1ab6f0-4769-4b27-930e-01e2ef9c123c/resourceGroups/testRG-4544/providers/Microsoft.Network/virtualNetworks/testVNet-9796/subnets/testSubnet-1786
        private async Task<GenericResource> CreateSubnet(ResourceIdentifier vnetId)
        {
            var subnetName = Recording.GenerateAssetName("testSubnet-");
            ResourceIdentifier subnetId = new ResourceIdentifier($"{vnetId}/subnets/{subnetName}");
            var input = new GenericResourceData(DefaultLocation)
            {
                Properties = new Dictionary<string, object>()
                {
                    { "addressPrefixes", new List<string>() { "10.0.2.0/24" } }
                }
            };
            var operation = await _genericResourceCollection.CreateOrUpdateAsync(subnetId, input);
            return operation.Value;
        }

        private async Task<GenericResource> CreateNetworkInterface(ResourceIdentifier subnetId)
        {
            var nicName = Recording.GenerateAssetName("testNic-");
            ResourceIdentifier nicId = new ResourceIdentifier($"{_resourceGroup.Id}/providers/Microsoft.Network/networkInterfaces/{nicName}");
            var input = new GenericResourceData(DefaultLocation)
            {
                Properties = new Dictionary<string, object>()
                {
                    { "ipConfigurations", new List<object>()
                        {
                            new Dictionary<string, object>()
                            {
                                { "name", "internal" },
                                { "properties", new Dictionary<string, object>()
                                    {
                                        { "subnet", new Dictionary<string, object>() { { "id", subnetId.ToString() } } }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var operation = await _genericResourceCollection.CreateOrUpdateAsync(nicId, input);
            return operation.Value;
        }

        protected async Task<GenericResource> CreateBasicDependenciesOfVirtualMachineAsync()
        {
            var vnet = await CreateVirtualNetwork();
            //var subnet = await CreateSubnet(vnet.Id as ResourceGroupResourceIdentifier);
            var nic = await CreateNetworkInterface(GetSubnetId(vnet));
            return nic;
        }

        protected static VirtualMachineData GetBasicLinuxVirtualMachineData(AzureLocation location, string computerName, ResourceIdentifier nicID, string adminUsername = "adminuser")
        {
            return new VirtualMachineData(location)
            {
                HardwareProfile = new HardwareProfile()
                {
                    VmSize = VirtualMachineSizeTypes.StandardF2
                },
                OsProfile = new OSProfile()
                {
                    AdminUsername = adminUsername,
                    ComputerName = computerName,
                    LinuxConfiguration = new LinuxConfiguration()
                    {
                        DisablePasswordAuthentication = true,
                        Ssh = new SshConfiguration()
                        {
                            PublicKeys = {
                                new SshPublicKeyInfo()
                                {
                                    Path = $"/home/{adminUsername}/.ssh/authorized_keys",
                                    KeyData = dummySSHKey,
                                }
                            }
                        }
                    }
                },
                NetworkProfile = new NetworkProfile()
                {
                    NetworkInterfaces =
                    {
                        new NetworkInterfaceReference()
                        {
                            Id = nicID,
                            Primary = true,
                        }
                    }
                },
                StorageProfile = new StorageProfile()
                {
                    OsDisk = new OSDisk(DiskCreateOptionTypes.FromImage)
                    {
                        OsType = OperatingSystemTypes.Linux,
                        Caching = CachingTypes.ReadWrite,
                        ManagedDisk = new ManagedDiskParameters()
                        {
                            StorageAccountType = StorageAccountTypes.StandardLRS
                        }
                    },
                    ImageReference = new ImageReference()
                    {
                        Publisher = "Canonical",
                        Offer = "UbuntuServer",
                        Sku = "16.04-LTS",
                        Version = "latest",
                    }
                }
            };
        }
    }
}
