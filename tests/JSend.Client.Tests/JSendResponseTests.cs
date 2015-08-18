using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using FluentAssertions;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Ploeh.Albedo;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.Client.Tests
{
    public class JSendResponseTests
    {
        private class WithDataAttribute : CustomizeAttribute, ICustomization
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return this;
            }

            public void Customize(IFixture fixture)
            {
                fixture.Customize<JSendResponse<Model>>(
                    c => c.FromFactory(
                        (Model model, HttpResponseMessage msg) => new JSendResponse<Model>(model, msg)));
            }
        }

        private class WithoutDataAttribute : CustomizeAttribute, ICustomization
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return this;
            }

            public void Customize(IFixture fixture)
            {
                fixture.Customize<JSendResponse<Model>>(
                    c => c.FromFactory(
                        (HttpResponseMessage msg) => new JSendResponse<Model>(msg)));
            }
        }

        private class WithErrorAttribute : CustomizeAttribute, ICustomization
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return this;
            }

            public void Customize(IFixture fixture)
            {
                fixture.Customize<JSendResponse<Model>>(
                    c => c.FromFactory(
                        (JSendError error, HttpResponseMessage msg) => new JSendResponse<Model>(error, msg)));
            }
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenHttpResponseMessageIsNull(JSendError error)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendResponse<int>(0, null));
            Assert.Throws<ArgumentNullException>(() => new JSendResponse<int>(null));
            Assert.Throws<ArgumentNullException>(() => new JSendResponse<int>(error, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenErrorIsNull(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendResponse<int>(null, httpResponseMessage));
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccessWhenNoErrorIsProvided(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse<int>(httpResponseMessage);
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
            var response = new JSendResponse<int>(error, httpResponseMessage);
            // Verify outcome
            response.Status.Should().Be(error.Status);
        }

        [Theory, JSendAutoData]
        public void IsSuccessIsTrueWhenNoErrorIsProvided(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse<int>(httpResponseMessage);
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
            var response = new JSendResponse<int>(error, httpResponseMessage);
            // Verify outcome
            response.IsSuccess.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void HttpResponseMessageIsCorrectlyInitialized(ConstructorInitializedMemberAssertion assertion)
        {
            // Fixture setup
            var property = new Properties<JSendResponse<int>>().Select(rsp => rsp.HttpResponseMessage);
            // Exercise system and verify outcome
            assertion.Verify(property);
        }

        [Theory, JSendAutoData]
        public void ErrorIsCorrectlyInitialized(ConstructorInitializedMemberAssertion assertion)
        {
            // Fixture setup
            var property = new Properties<JSendResponse<int>>().Select(rsp => rsp.Error);
            // Exercise system and verify outcome
            assertion.Verify(property);
        }

        [Theory, JSendAutoData]
        public void DataIsCorrectlyInitialized(Model model, HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse<Model>(model, httpResponseMessage);
            // Verify outcome
            response.Data.Should().Be(model);
        }

        [Theory, JSendAutoData]
        public void DataThrowsExceptionWhenNoneIsProvided(HttpResponseMessage httpResponseMessage)
        {
            // Exercise system
            var response = new JSendResponse<Model>(httpResponseMessage);
            // Verify outcome
            Action data = () => { var x = response.Data; };
            data.ShouldThrow<JSendRequestException>()
                .WithMessage(StringResources.SuccessResponseWithoutData);
        }

        [Theory, JSendAutoData]
        public void DataThrowsExceptionWhenErrorIsProvided(HttpResponseMessage httpResponseMessage)
        {
            // Fixture setup
            var error = new JSendError(JSendStatus.Fail, null, null, null);
            // Exercise system
            var response = new JSendResponse<Model>(error, httpResponseMessage);
            // Verify outcome
            Action data = () => { var x = response.Data; };
            data.ShouldThrow<JSendRequestException>()
                .WithMessage("JSend status does not indicate success: \"fail\".");
        }

        [Theory, JSendAutoData]
        public void HasDataWhenDataIsProvided([WithData] JSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.HasData.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void DoesNotHaveDataWhenNoneIsProvided([WithoutData] JSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.HasData.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void DoesNotHaveDataWhenErrorIsProvided([WithError] JSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.HasData.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_ReturnsData_WhenDataIsProvided([WithData] JSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.GetDataOrDefault().Should().Be(response.Data);
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_ReturnsDefault_WhenDataIsNotProvided([WithoutData] JSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.GetDataOrDefault().Should().Be(default(Model));
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_WithDefaultValue_ReturnsData_WhenDataIsProvided(
            [WithData] JSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.GetDataOrDefault(null).Should().Be(response.Data);
        }

        [Theory, JSendAutoData]
        public void GetDataOrDefault_WithDefaultValue_ReturnsDefault_WhenDataIsNotProvided(
            [WithoutData] JSendResponse<Model> response)
        {
            // Fixture setup
            Model defaultValue = new Model();
            // Exercise system and verify outcome
            response.GetDataOrDefault(defaultValue).Should().Be(defaultValue);
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_Throws_WhenStatusIsNotSuccess(HttpResponseMessage httpResponseMessage)
        {
            // Fixture setup
            var error = new JSendError(JSendStatus.Fail, null, null, null);
            var nonSuccessResponse = new JSendResponse<Model>(error, httpResponseMessage);
            // Exercise system and verify outcome
            nonSuccessResponse.Invoking(rsp => rsp.EnsureSuccessStatus())
                .ShouldThrow<JSendRequestException>()
                .WithMessage("JSend status does not indicate success: \"fail\".");
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_ReturnsSelf_WhenStatusIsSuccess([WithData] JSendResponse<Model> successResponse)
        {
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
            var response = new JSendResponse<int>(spy);
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
            var response = new JSendResponse<int>(error, spy);

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
                        new JSendResponse<string>(HttpResponseMessageSingleton),
                        new JSendResponse<string>(HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton),
                        new JSendResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse<string>("a", HttpResponseMessageSingleton),
                        new JSendResponse<string>("a", HttpResponseMessageSingleton)
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
                        new JSendResponse<string>(new HttpResponseMessage()),
                        new JSendResponse<string>(new HttpResponseMessage())
                    },
                    new object[]
                    {
                        new JSendResponse<string>(HttpResponseMessageSingleton),
                        new JSendResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse<string>(new JSendError(JSendStatus.Fail, null, null, null),
                            HttpResponseMessageSingleton),
                        new JSendResponse<string>(new JSendError(JSendStatus.Error, null, null, null),
                            HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse<string>(HttpResponseMessageSingleton),
                        new JSendResponse<string>("a", HttpResponseMessageSingleton)
                    },

                    new object[]
                    {
                        new JSendResponse<string>(HttpResponseMessageSingleton),
                        new JSendResponse<string>(default(string), HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse<string>("a", HttpResponseMessageSingleton),
                        new JSendResponse<string>("b", HttpResponseMessageSingleton)
                    },
                    new object[]
                    {
                        new JSendResponse<string>("a", HttpResponseMessageSingleton),
                        new JSendResponse<string>(new JSendError(JSendStatus.Error, null, null, null),
                            HttpResponseMessageSingleton)
                    }
                };
            }
        }

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
        public void ResponseIsNotEqualToNull(JSendResponse<string> response)
        {
            // Exercise system and verify outcome
            response.Equals(null).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ResponseIsNotEqualToInstanceOfAnotherType(JSendResponse<string> response, string other)
        {
            // Exercise system and verify outcome
            response.Equals(other).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void Equals_IsReflexive(JSendResponse<string> response)
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
        public void EqualityOperator_ReturnsFalse_WhenLeftOperandsIsNull(JSendResponse<string> response)
        {
            // Exercise system
            var areEqual = null == response;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsFalse_WhenRightOperandsIsNull(JSendResponse<string> response)
        {
            // Exercise system
            var areEqual = response == null;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_IsReflexive(JSendResponse<string> response)
        {
#pragma warning disable 1718
            // Exercise system
            var areEqual = response == response;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
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
        public void InequalityOperator_ReturnsTrue_WhenLeftOperandsIsNull(JSendResponse<string> response)
        {
            // Exercise system
            var areNotEqual = null != response;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsTrue_WhenRightOperandsIsNull(JSendResponse<string> response)
        {
            // Exercise system
            var areNotEqual = response != null;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_IsReflexive(JSendResponse<string> response)
        {
#pragma warning disable 1718
            // Exercise system
            var areNotEqual = response != response;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
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
