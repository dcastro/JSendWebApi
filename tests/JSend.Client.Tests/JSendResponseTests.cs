using System;
using System.Net.Http;
using FluentAssertions;
using JSend.Client.Tests.FixtureCustomizations;
using Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class JSendResponseTests
    {
        [Theory, JSendAutoData]
        public void ConstructorsThrowsWhenHttpResponseMessageIsNull(JSendError error)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendResponse(error, null));
            Assert.Throws<ArgumentNullException>(() => new JSendResponse(null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenErrorIsNull(HttpResponseMessage responseMessage)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendResponse(null, responseMessage));
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccessWhenNoErrorIsProvided(HttpResponseMessage responseMessage)
        {
            // Exercise system
            var response = new JSendResponse(responseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
        }

        [Theory]
        [InlineJSendAutoData(JSendStatus.Fail)]
        [InlineJSendAutoData(JSendStatus.Error)]
        public void StatusMatchesErrorStatusWhenErrorIsProvided(JSendStatus jsendErrorStatus,
            HttpResponseMessage responseMessage)
        {
            // Fixture setup
            var error = new JSendError(jsendErrorStatus, null, null, null);
            // Exercise system
            var response = new JSendResponse(error, responseMessage);
            // Verify outcome
            response.Status.Should().Be(error.Status);
        }

        [Theory, JSendAutoData]
        public void IsSuccessIsTrueWhenNoErrorIsProvided(HttpResponseMessage responseMessage)
        {
            // Exercise system
            var response = new JSendResponse(responseMessage);
            // Verify outcome
            response.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [InlineJSendAutoData(JSendStatus.Fail)]
        [InlineJSendAutoData(JSendStatus.Error)]
        public void IsSuccessIsFalseWhenErrorIsProvided(JSendStatus jsendErrorStatus,
            HttpResponseMessage responseMessage)
        {
            // Fixture setup
            var error = new JSendError(jsendErrorStatus, null, null, null);
            // Exercise system
            var response = new JSendResponse(error, responseMessage);
            // Verify outcome
            response.IsSuccess.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ResponseMessageIsCorrectlyInitialized(HttpResponseMessage responseMessage)
        {
            // Exercise system
            var response = new JSendResponse(responseMessage);
            // Verify outcome
            response.ResponseMessage.Should().Be(responseMessage);
        }

        [Theory, JSendAutoData]
        public void ErrorAndResponseMessageAreCorrectlyInitialized(JSendError error, HttpResponseMessage responseMessage)
        {
            // Exercise system
            var response = new JSendResponse(error, responseMessage);
            // Verify outcome
            response.Error.Should().Be(error);
            response.ResponseMessage.Should().Be(responseMessage);
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_Throws_WhenStatusIsNotSuccess(JSendError error,
            HttpResponseMessage responseMessage)
        {
            // Fixture setup
            var nonSuccessResponse = new JSendResponse(error, responseMessage);
            // Exercise system and verify outcome
            Action ensureSuccessStatus = () => nonSuccessResponse.EnsureSuccessStatus();
            ensureSuccessStatus.ShouldThrow<JSendResponseException>()
                .And.Message.Should().Be("JSend status does not indicate success: \"fail\".");
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_ReturnsSelf_WhenStatusIsSuccess(HttpResponseMessage responseMessage)
        {
            // Fixture setup
            var successResponse = new JSendResponse(responseMessage);
            // Exercise system
            var response = successResponse.EnsureSuccessStatus();
            // Verify outcome
            response.Should().BeSameAs(successResponse);
        }

        public class HttpResponseMessageSpy : HttpResponseMessage
        {
            public bool Disposed = false;

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                Disposed = disposing;
            }
        }

        [Theory, JSendAutoData]
        public void DisposingOfTheResponse_DisposesOfTheHttpResponseMessage(HttpResponseMessageSpy spy)
        {
            // Fixture setup
            var response = new JSendResponse(spy);
            // Exercise system
            response.Dispose();
            // Verify outcome
            spy.Disposed.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_DisposesOfHttpResponseMessage_WhenStatusIsNotSuccess(JSendError error,
            HttpResponseMessageSpy spy)
        {
            // Fixture setup
            var response = new JSendResponse(error, spy);

            // Exercise system
            try
            {
                response.EnsureSuccessStatus();
            }
            catch
            {
            }
            // Verify outcome
            spy.Disposed.Should().BeTrue();
        }
    }
}
