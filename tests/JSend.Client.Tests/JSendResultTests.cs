using System.Net.Http;
using FluentAssertions;
using JSend.Client.Tests.TestTypes;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class JSendResultTests
    {
        private class ResponseAutoDataAttribute : AutoDataAttribute
        {
            public ResponseAutoDataAttribute()
                : base(new Fixture().Customize(new AutoConfiguredMoqCustomization()))
            {

            }
        }

        [Theory, ResponseAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendResult<Model>).GetConstructors());
        }

        [Theory, ResponseAutoData]
        public void SuccessJSendResponseIsCorrectlyInitialized(SuccessResponse<string> jsendResponse,
            HttpResponseMessage httpResponse)
        {
            // Exercise system
            var result = new JSendResult<string>(jsendResponse, httpResponse);
            // Verify outcome
            result.JsendResponse.Should().Be(jsendResponse);
        }

        [Theory, ResponseAutoData]
        public void FailJSendResponseIsCorrectlyInitialized(FailResponse jsendResponse,
            HttpResponseMessage httpResponse)
        {
            // Exercise system
            var result = new JSendResult<string>(jsendResponse, httpResponse);
            // Verify outcome
            result.JsendResponse.Should().Be(jsendResponse);
        }

        [Theory, ResponseAutoData]
        public void ResponseMessageIsCorrectlyInitialized(SuccessResponse<string> jsendResponse,
            HttpResponseMessage httpResponse)
        {
            // Exercise system
            var result = new JSendResult<string>(jsendResponse, httpResponse);
            // Verify outcome
            result.ResponseMessage.Should().Be(httpResponse);
        }
    }
}
