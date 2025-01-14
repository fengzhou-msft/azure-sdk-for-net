// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace Azure.AI.MetricsAdvisor.Models
{
    /// <summary> The EmailHookParameterPatch. </summary>
    internal partial class EmailHookParameterPatch
    {
        /// <summary> Initializes a new instance of EmailHookParameterPatch. </summary>
        public EmailHookParameterPatch()
        {
            ToList = new ChangeTrackingList<string>();
        }

        /// <summary> Email TO: list. </summary>
        public IList<string> ToList { get; }
    }
}
