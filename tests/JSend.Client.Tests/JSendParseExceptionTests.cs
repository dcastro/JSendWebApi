using System;
using FluentAssertions;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class JSendParseExceptionTests
    {
        [Theory, JSendAutoData]
        public void BuildsMessageCorrectly(Exception ex )
        {
            // Fixture setup
            var responseType = typeof (JSendResponse<Model>);
            // Exercise system
            var exception = new JSendParseException(responseType, ex);
            // Verify outcome
            exception.Message.Should()
                .Be(
                    @"HTTP response message could not be parsed into an instance of type ""JSend.Client.JSendResponse`1[JSend.Client.Tests.TestTypes.Model]"".");
        }
    }
}
