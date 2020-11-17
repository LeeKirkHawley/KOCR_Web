using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using api.K_OCR;

namespace UnitTests {
    [CollectionDefinition("Integration Tests")]
    class TestCollection  : ICollectionFixture<WebApplicationFactory<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
