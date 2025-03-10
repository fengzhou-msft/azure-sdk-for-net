// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.Search.Documents.Indexes.Models
{
    /// <summary> A list of skills. </summary>
    public partial class SearchIndexerSkillset
    {
        /// <summary> Initializes a new instance of SearchIndexerSkillset. </summary>
        /// <param name="name"> The name of the skillset. </param>
        /// <param name="skills"> A list of skills in the skillset. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name"/> or <paramref name="skills"/> is null. </exception>
        public SearchIndexerSkillset(string name, IEnumerable<SearchIndexerSkill> skills)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (skills == null)
            {
                throw new ArgumentNullException(nameof(skills));
            }

            Name = name;
            Skills = skills.ToList();
        }

        /// <summary> Initializes a new instance of SearchIndexerSkillset. </summary>
        /// <param name="name"> The name of the skillset. </param>
        /// <param name="description"> The description of the skillset. </param>
        /// <param name="skills"> A list of skills in the skillset. </param>
        /// <param name="cognitiveServicesAccount"> Details about cognitive services to be used when running skills. </param>
        /// <param name="knowledgeStore"> Definition of additional projections to azure blob, table, or files, of enriched data. </param>
        /// <param name="Etag"> The ETag of the skillset. </param>
        /// <param name="encryptionKey"> A description of an encryption key that you create in Azure Key Vault. This key is used to provide an additional level of encryption-at-rest for your skillset definition when you want full assurance that no one, not even Microsoft, can decrypt your skillset definition in Azure Cognitive Search. Once you have encrypted your skillset definition, it will always remain encrypted. Azure Cognitive Search will ignore attempts to set this property to null. You can change this property as needed if you want to rotate your encryption key; Your skillset definition will be unaffected. Encryption with customer-managed keys is not available for free search services, and is only available for paid services created on or after January 1, 2019. </param>
        internal SearchIndexerSkillset(string name, string description, IList<SearchIndexerSkill> skills, CognitiveServicesAccount cognitiveServicesAccount, SearchIndexerKnowledgeStore knowledgeStore, string Etag, SearchResourceEncryptionKey encryptionKey)
        {
            Name = name;
            Description = description;
            Skills = skills;
            CognitiveServicesAccount = cognitiveServicesAccount;
            KnowledgeStore = knowledgeStore;
            _etag = Etag;
            EncryptionKey = encryptionKey;
        }

        /// <summary> The name of the skillset. </summary>
        public string Name { get; set; }
        /// <summary> The description of the skillset. </summary>
        public string Description { get; set; }
        /// <summary> Details about cognitive services to be used when running skills. </summary>
        public CognitiveServicesAccount CognitiveServicesAccount { get; set; }
        /// <summary> Definition of additional projections to azure blob, table, or files, of enriched data. </summary>
        public SearchIndexerKnowledgeStore KnowledgeStore { get; set; }
        /// <summary> A description of an encryption key that you create in Azure Key Vault. This key is used to provide an additional level of encryption-at-rest for your skillset definition when you want full assurance that no one, not even Microsoft, can decrypt your skillset definition in Azure Cognitive Search. Once you have encrypted your skillset definition, it will always remain encrypted. Azure Cognitive Search will ignore attempts to set this property to null. You can change this property as needed if you want to rotate your encryption key; Your skillset definition will be unaffected. Encryption with customer-managed keys is not available for free search services, and is only available for paid services created on or after January 1, 2019. </summary>
        public SearchResourceEncryptionKey EncryptionKey { get; set; }
    }
}
