using System;
using FluentAssertions;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class JSendErrorTests
    {
        [Theory, JSendAutoData]
        public void StatusCannotBeSuccess(string message, int? code, JToken data)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendError(JSendStatus.Success, message, code, data);
            ctor.ShouldThrow<ArgumentException>()
                .And.Message.Should().StartWith(StringResources.ErrorWithSuccessStatus);
        }

        [Theory]
        [InlineJSendAutoData(JSendStatus.Fail)]
        [InlineJSendAutoData(JSendStatus.Error)]
        public void StatusIsCorrectlyInitialized(JSendStatus status, string message, int? code, JToken data)
        {
            // Exercise system
            var error = new JSendError(status, message, code, data);
            // Verify outcome
            error.Status.Should().Be(status);
        }

        [Theory, JSendAutoData]
        public void MessageIsCorrectlyInitialized(string message, int? code, JToken data)
        {
            // Exercise system
            var error = new JSendError(JSendStatus.Fail, message, code, data);
            // Verify outcome
            error.Message.Should().Be(message);
        }

        [Theory, JSendAutoData]
        public void CodeIsCorrectlyInitialized(string message, int? code, JToken data)
        {
            // Exercise system
            var error = new JSendError(JSendStatus.Fail, message, code, data);
            // Verify outcome
            error.Code.Should().Be(code);
        }

        [Theory, JSendAutoData]
        public void DataIsCorrectlyInitialized(string message, int? code, JToken data)
        {
            // Exercise system
            var error = new JSendError(JSendStatus.Fail, message, code, data);
            // Verify outcome
            error.Data.Should().BeSameAs(data);
        }
    }
}
