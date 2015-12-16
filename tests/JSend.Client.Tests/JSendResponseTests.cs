using System;
using System.Collections.Generic;
using System.Net.Http;
using FluentAssertions;
using JSend.Client.Responses;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Xunit;

namespace JSend.Client.Tests
{
    public class JSendResponseTests
    {
        [Theory, JSendAutoData]
        public void HttpResponseMessageMustNotBeNull(JSendError error)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSuccessWithDataResponse<int>(0, null));
            Assert.Throws<ArgumentNullException>(() => new JSuccessResponse<int>(null));
            Assert.Throws<ArgumentNullException>(() => new JErrorResponse<int>(error, null));
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_ReturnsData_WhenDataIsProvided(JSuccessWithDataResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.GetDataOrDefault().Should().Be(response.Data);
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_ReturnsDefault_WhenDataIsNotProvided(JSuccessResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.GetDataOrDefault().Should().Be(default(Model));
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_WithDefaultValue_ReturnsData_WhenDataIsProvided(JSuccessWithDataResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.GetDataOrDefault(null).Should().Be(response.Data);
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_WithDefaultValue_ReturnsDefault_WhenDataIsNotProvided(JSuccessResponse<Model> response)
        {
            // Fixture setup
            Model defaultValue = new Model();
            // Exercise system and verify outcome
            response.GetDataOrDefault(defaultValue).Should().Be(defaultValue);
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_Throws_WhenStatusIsNotSuccess(JErrorResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Invoking(rsp => rsp.EnsureSuccessStatus())
                .ShouldThrow<JSendRequestException>()
                .WithMessage("JSend status does not indicate success: " + response + ".");
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_ReturnsSelf_WhenStatusIsSuccess(JSuccessResponse<Model> response)
        {
            // Exercise system
            var result = response.EnsureSuccessStatus();
            // Verify outcome
            result.Should().BeSameAs(response);
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
        public void DisposingOfTheResponse_DisposesOfTheHttpResponse(HttpResponseMessageSpy spy)
        {
            // Fixture setup
            var response = new JSuccessResponse<int>(spy);
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
            var response = new JErrorResponse<int>(error, spy);

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

        public class DerivedResponseA<T> : JSendResponse<T>
        {
            public DerivedResponseA(HttpResponseMessage httpResponse) : base(httpResponse)
            {
            }

            public override T Data { get; }
            public override bool HasData { get; }
            public override JSendStatus Status { get; }
            public override JSendError Error { get; }
        }

        public class DerivedResponseB<T> : JSendResponse<T>
        {
            public DerivedResponseB(HttpResponseMessage httpResponse) : base(httpResponse)
            {
            }

            public override T Data { get; }
            public override bool HasData { get; }
            public override JSendStatus Status { get; }
            public override JSendError Error { get; }
        }

        public static IEnumerable<object[]> EquivalentResponses => new[]
        {
            new object[]
            {
                new JSuccessResponse<string>(HttpResponseMessageSingleton),
                new JSuccessResponse<string>(HttpResponseMessageSingleton)
            },
            new object[]
            {
                new JErrorResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                    HttpResponseMessageSingleton),
                new JErrorResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                    HttpResponseMessageSingleton)
            },
            new object[]
            {
                new JSuccessWithDataResponse<string>("a", HttpResponseMessageSingleton),
                new JSuccessWithDataResponse<string>("a", HttpResponseMessageSingleton)
            }
        };

        public static IEnumerable<object[]> DistinctResponses => new[]
        {
            new object[]
            {
                new JSuccessResponse<string>(new HttpResponseMessage()),
                new JSuccessResponse<string>(new HttpResponseMessage())
            },
            new object[]
            {
                new JSuccessResponse<string>(HttpResponseMessageSingleton),
                new JErrorResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                    HttpResponseMessageSingleton)
            },
            new object[]
            {
                new JErrorResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                    HttpResponseMessageSingleton),
                new JErrorResponse<string>(new JSendError(JSendStatus.Error, null, null, null),
                    HttpResponseMessageSingleton)
            },
            new object[]
            {
                new JSuccessResponse<string>(HttpResponseMessageSingleton),
                new JSuccessWithDataResponse<string>("a", HttpResponseMessageSingleton)
            },

            new object[]
            {
                new JSuccessResponse<string>(HttpResponseMessageSingleton),
                new JSuccessWithDataResponse<string>(default(string), HttpResponseMessageSingleton)
            },
            new object[]
            {
                new JSuccessWithDataResponse<string>("a", HttpResponseMessageSingleton),
                new JSuccessWithDataResponse<string>("b", HttpResponseMessageSingleton)
            },
            new object[]
            {
                new JSuccessWithDataResponse<string>("a", HttpResponseMessageSingleton),
                new JErrorResponse<string>(new JSendError(JSendStatus.Error, null, null, null),
                    HttpResponseMessageSingleton)
            }
        };

        [Theory]
        [MemberData("EquivalentResponses")]
        public void TwoResponses_AreEqual_WhenTheirFieldsMatch(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeTrue();
        }

        [Theory]
        [MemberData("DistinctResponses")]
        public void TwoResponses_AreNotEqual_WhenTheirFieldsDoNotMatch(JSendResponse<string> first,
            JSendResponse<string> second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void TwoResponses_AreNotEqual_WhenTheirTypesDoNotMatch(HttpResponseMessage httpResponse)
        {
            // Fixture setup
            var first = new DerivedResponseA<int>(httpResponse);
            var second = new DerivedResponseB<int>(httpResponse);
            // Exercise system and verify outcome
            first.Equals(second).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ResponseIsNotEqualToNull(JSuccessResponse<string> response)
        {
            // Exercise system and verify outcome
            response.Equals(null).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ResponseIsNotEqualToInstanceOfAnotherType(JSuccessResponse<string> response, string other)
        {
            // Exercise system and verify outcome
            response.Equals(other).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void Equals_IsReflexive(JSuccessResponse<string> response)
        {
            // Exercise system and verify outcome
            response.Equals(response).Should().BeTrue();
        }

        [Theory]
        [MemberData("EquivalentResponses")]
        [MemberData("DistinctResponses")]
        public void Equals_IsSymmetric(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var firstEqualsSecond = first.Equals(second);
            var secondEqualsFirst = second.Equals(first);
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [MemberData("EquivalentResponses")]
        public void EqualityOperator_ReturnsTrue_WhenFieldsMatch(JSendResponse<string> first, JSendResponse<string> second)
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
            JSendResponse<string> response = null;
            // Exercise system
            var areEqual = response == response;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory]
        [MemberData("DistinctResponses")]
        public void EqualityOperator_ReturnsFalse_WhenFieldsDoNotMatch(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var areEqual = first == second;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsFalse_WhenLeftOperandsIsNull(JSuccessResponse<string> response)
        {
            // Exercise system
            var areEqual = null == response;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsFalse_WhenRightOperandsIsNull(JSuccessResponse<string> response)
        {
            // Exercise system
            var areEqual = response == null;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_IsReflexive(JSuccessResponse<string> response)
        {
#pragma warning disable 1718
            // Exercise system
            var areEqual = response == response;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory]
        [MemberData("EquivalentResponses")]
        [MemberData("DistinctResponses")]
        public void EqualityOperator_IsSymmetric(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var firstEqualsSecond = first == second;
            var secondEqualsFirst = second == first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [MemberData("EquivalentResponses")]
        public void InequalityOperator_ReturnsFalse_WhenFieldsMatch(JSendResponse<string> first, JSendResponse<string> second)
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
            JSendResponse<string> response = null;
            // Exercise system
            var areNotEqual = response != response;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory]
        [MemberData("DistinctResponses")]
        public void InequalityOperator_ReturnsTrue_WhenFieldsDoNotMatch(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var areNotEqual = first != second;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsTrue_WhenLeftOperandsIsNull(JSuccessResponse<string> response)
        {
            // Exercise system
            var areNotEqual = null != response;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsTrue_WhenRightOperandsIsNull(JSuccessResponse<string> response)
        {
            // Exercise system
            var areNotEqual = response != null;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_IsReflexive(JSuccessResponse<string> response)
        {
#pragma warning disable 1718
            // Exercise system
            var areNotEqual = response != response;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory]
        [MemberData("EquivalentResponses")]
        [MemberData("DistinctResponses")]
        public void InequalityOperator_IsSymmetric(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var firstEqualsSecond = first != second;
            var secondEqualsFirst = second != first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [MemberData("EquivalentResponses")]
        public void EqualResponsesHaveTheSameHashCode(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system and verify outcome
            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
