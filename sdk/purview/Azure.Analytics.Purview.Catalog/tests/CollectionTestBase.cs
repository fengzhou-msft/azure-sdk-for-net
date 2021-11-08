// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Core.TestFramework;

namespace Azure.Analytics.Purview.Catalog.Tests
{
    public class CollectionTestBase : RecordedTestBase<PurviewCatalogTestEnvironment>
    {
        public CollectionTestBase(bool isAsync) : base(isAsync)
        {
        }
        public CollectionTestBase(bool isAsync, RecordedTestMode mode) : base(isAsync, mode)
        {
        }
        public PurviewCollections GetCollectionsClient()
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) =>
            {
                return true;
            };
            var options = new PurviewCatalogClientOptions { Transport = new HttpClientTransport(httpHandler) };
            var catalogclient = new PurviewCatalogClient(TestEnvironment.Endpoint, TestEnvironment.Credential, InstrumentClientOptions(options));
            return InstrumentClient(catalogclient.Collections);
        }
    }
}
