using System;
using System.Collections.Generic;
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
        public void ConstructorThrowsWhenErrorIsNull(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendResponse(null, httpResponseMessage));
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccessWhenNoErrorIsProvided(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse(httpResponseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
        }

        [Theory]
        [InlineJSendAutoData(JSendStatus.Fail)]
        [InlineJSendAutoData(JSendStatus.Error)]
        public void StatusMatchesErrorStatusWhenErrorIsProvided(JSendStatus jsendErrorStatus,
            HttpResponseMessage httpResponseMessage)
        {
            // Fixture setup
            var error = new JSendError(jsendErrorStatus, null, null, null);
            // Exercise system
            var response = new JSendResponse(error, httpResponseMessage);
            // Verify outcome
            response.Status.Should().Be(error.Status);
        }

        [Theory, JSendAutoData]
        public void IsSuccessIsTrueWhenNoErrorIsProvided(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse(httpResponseMessage);
            // Verify outcome
            response.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [InlineJSendAutoData(JSendStatus.Fail)]
        [InlineJSendAutoData(JSendStatus.Error)]
        public void IsSuccessIsFalseWhenErrorIsProvided(JSendStatus jsendErrorStatus,
            HttpResponseMessage httpResponseMessage)
        {
            // Fixture setup
            var error = new JSendError(jsendErrorStatus, null, null, null);
            // Exercise system
            var response = new JSendResponse(error, httpResponseMessage);
            // Verify outcome
            response.IsSuccess.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void HttpResponseMessageIsCorrectlyInitialized(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse(httpResponseMessage);
            // Verify outcome
            response.HttpResponseMessage.Should().Be(httpResponseMessage);
        }

        [Theory, JSendAutoData]
        public void ErrorAndHttpResponseMessageAreCorrectlyInitialized(JSendError error,
            HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse(error, httpResponseMessage);
            // Verify outcome
            response.Error.Should().Be(error);
            response.HttpResponseMessage.Should().Be(httpResponseMessage);
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_Throws_WhenStatusIsNotSuccess(HttpResponseMessage httpResponseMessage)
        {
            // Fixture setup
            var error = new JSendError(JSendStatus.Fail, null, null, null);
            var nonSuccessResponse = new JSendResponse(error, httpResponseMessage);
            // Exercise system and verify outcome
            nonSuccessResponse.Invoking(rsp => rsp.EnsureSuccessStatus())
                .ShouldThrow<JSendRequestException>()
                .WithMessage("JSend status does not indicate success: \"fail\".");
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_ReturnsSelf_WhenStatusIsSuccess(HttpResponseMessage httpResponseMessage)
        {
            // Fixture setup
            var successResponse = new JSendResponse(httpResponseMessage);
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

        private static readonly HttpResponseMessage HttpResponseMessageSingleton = new HttpResponseMessage();

        public static IEnumerable<object[]> EquivalentResponses
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new JSendResponse(HttpResponseMessageSingleton),
                        new JSendResponse(HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton),
                        new JSendResponse(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton)
                    }
                };
            }
        }

        public static IEnumerable<object[]> DistinctResponses
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new JSendResponse(new HttpResponseMessage()),
                        new JSendResponse(new HttpResponseMessage())
                    },
                    new object[]
                    {
                        new JSendResponse(HttpResponseMessageSingleton),
                        new JSendResponse(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton),
                        new JSendResponse(new JSendError(JSendStatus.Error, null, null, null),
                            HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse(HttpResponseMessageSingleton),
                        new JSendResponse<string>(HttpResponseMessageSingleton)
                    }
                };
            }
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
        public void TwoResponses_AreEqual_WhenTheirFieldsMatch(JSendResponse first, JSendResponse second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeTrue();
        }

        [Theory]
        [PropertyData("DistinctResponses")]
        public void TwoResponses_AreNotEqual_WhenTheirFieldsDoNotMatch(JSendResponse first, JSendResponse second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ResponseIsNotEqualToNull(JSendResponse response)
        {
            // Exercise system and verify outcome
            response.Equals(null).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ResponseIsNotEqualToInstanceOfAnotherType(JSendResponse response, string other)
        {
            // Exercise system and verify outcome
            response.Equals(other).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void Equals_IsReflexive(JSendResponse response)
        {
            // Exercise system and verify outcome
            response.Equals(response).Should().BeTrue();
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
        [PropertyData("DistinctResponses")]
        public void Equals_IsSymmetric(JSendResponse first, JSendResponse second)
        {
            // Exercise system
            var firstEqualsSecond = first.Equals(second);
            var secondEqualsFirst = second.Equals(first);
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
        public void EqualityOperator_ReturnsTrue_WhenFieldsMatch(JSendResponse first, JSendResponse second)
        {
            // Exercise system
            var areEqual = first == second;
            // Verify outcome
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void EqualityOperator_ReturnsTrue_WhenBothResponsesAreNull()
        {
#pragma warning disable 1718
            // Fixture setup
            JSendResponse response = null;
            // Exercise system
            var areEqual = response == response;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory]
        [PropertyData("DistinctResponses")]
        public void EqualityOperator_ReturnsFalse_WhenFieldsDoNotMatch(JSendResponse first, JSendResponse second)
        {
            // Exercise system
            var areEqual = first == second;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsFalse_WhenLeftOperandsIsNull(JSendResponse response)
        {
            // Exercise system
            var areEqual = null == response;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsFalse_WhenRightOperandsIsNull(JSendResponse response)
        {
            // Exercise system
            var areEqual = response == null;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_IsReflexive(JSendResponse response)
        {
#pragma warning disable 1718
            // Exercise system
            var areEqual = response == response;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
        [PropertyData("EquivalentResponses")]
        [PropertyData("DistinctResponses")]
        public void EqualityOperator_IsSymmetric(JSendResponse first, JSendResponse second)
        {
            // Exercise system
            var firstEqualsSecond = first == second;
            var secondEqualsFirst = second == first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
        public void InequalityOperator_ReturnsFalse_WhenFieldsMatch(JSendResponse first, JSendResponse second)
        {
            // Exercise system
            var areNotEqual = first != second;
            // Verify outcome
            areNotEqual.Should().BeFalse();
        }

        [Fact]
        public void InequalityOperator_ReturnsFalse_WhenBothResponsesAreNull()
        {
#pragma warning disable 1718
            // Fixture setup
            JSendResponse response = null;
            // Exercise system
            var areNotEqual = response != response;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory]
        [PropertyData("DistinctResponses")]
        public void InequalityOperator_ReturnsTrue_WhenFieldsDoNotMatch(JSendResponse first, JSendResponse second)
        {
            // Exercise system
            var areNotEqual = first != second;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsTrue_WhenLeftOperandsIsNull(JSendResponse response)
        {
            // Exercise system
            var areNotEqual = null != response;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsTrue_WhenRightOperandsIsNull(JSendResponse response)
        {
            // Exercise system
            var areNotEqual = response != null;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_IsReflexive(JSendResponse response)
        {
#pragma warning disable 1718
            // Exercise system
            var areNotEqual = response != response;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
        [PropertyData("EquivalentResponses")]
        [PropertyData("DistinctResponses")]
        public void InequalityOperator_IsSymmetric(JSendResponse first, JSendResponse second)
        {
            // Exercise system
            var firstEqualsSecond = first != second;
            var secondEqualsFirst = second != first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
        public void EqualResponsesHaveTheSameHashCode(JSendResponse first, JSendResponse second)
        {
            // Exercise system and verify outcome
            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
