using System;
using System.Net.Http;
using FluentAssertions;
using JSend.Client.Tests.FixtureCustomizations;
using Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class JSendResultTests
    {
        [Theory, JSendAutoData]
        public void ConstructorsThrowsWhenHttpResponseMessageIsNull(JSendError error)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendResult(error, null));
            Assert.Throws<ArgumentNullException>(() => new JSendResult(null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenErrorIsNull(HttpResponseMessage responseMessage)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendResult(null, responseMessage));
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccessWhenNoErrorIsProvided(HttpResponseMessage responseMessage)
        {
            // Exercise system
            var result = new JSendResult(responseMessage);
            // Verify outcome
            result.Status.Should().Be(JSendStatus.Success);
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
            var result = new JSendResult(error, responseMessage);
            // Verify outcome
            result.Status.Should().Be(error.Status);
        }

        [Theory, JSendAutoData]
        public void IsSuccessIsTrueWhenNoErrorIsProvided(HttpResponseMessage responseMessage)
        {
            // Exercise system
            var result = new JSendResult(responseMessage);
            // Verify outcome
            result.IsSuccess.Should().BeTrue();
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
            var result = new JSendResult(error, responseMessage);
            // Verify outcome
            result.IsSuccess.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ResponseMessageIsCorrectlyInitialized(HttpResponseMessage responseMessage)
        {
            // Exercise system
            var result = new JSendResult(responseMessage);
            // Verify outcome
            result.ResponseMessage.Should().Be(responseMessage);
        }

        [Theory, JSendAutoData]
        public void ErrorAndResponseMessageAreCorrectlyInitialized(JSendError error, HttpResponseMessage responseMessage)
        {
            // Exercise system
            var result = new JSendResult(error, responseMessage);
            // Verify outcome
            result.Error.Should().Be(error);
            result.ResponseMessage.Should().Be(responseMessage);
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_Throws_WhenStatusIsNotSuccess(JSendError error,
            HttpResponseMessage responseMessage)
        {
            // Fixture setup
            var nonSuccessResult = new JSendResult(error, responseMessage);
            // Exercise system and verify outcome
            Action ensureSuccessStatus = () => nonSuccessResult.EnsureSuccessStatus();
            ensureSuccessStatus.ShouldThrow<JSendRequestException>()
                .And.Message.Should().Be("JSend status does not indicate success: \"fail\".");
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_ReturnsSelf_WhenStatusIsSuccess(HttpResponseMessage responseMessage)
        {
            // Fixture setup
            var successResult = new JSendResult(responseMessage);
            // Exercise system
            var result = successResult.EnsureSuccessStatus();
            // Verify outcome
            result.Should().BeSameAs(successResult);
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
        public void DisposingOfTheResult_DisposesOfTheHttpResponseMessage(HttpResponseMessageSpy spy)
        {
            // Fixture setup
            var result = new JSendResult(spy);
            // Exercise system
            result.Dispose();
            // Verify outcome
            spy.Disposed.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_DisposesOfHttpResponseMessage_WhenStatusIsNotSuccess(JSendError error,
            HttpResponseMessageSpy spy)
        {
            // Fixture setup
            var result = new JSendResult(error, spy);

            // Exercise system
            try
            {
                result.EnsureSuccessStatus();
            }
            catch
            {
            }
            // Verify outcome
            spy.Disposed.Should().BeTrue();
        }
    }
}
