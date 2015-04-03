using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using FluentAssertions;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class GenericJSendResponseTests
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
                .And.Message.Should().StartWith(StringResources.SuccessResponseWithoutData);
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
                .And.Message.Should().StartWith("JSend status does not indicate success: \"fail\".");
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
                .And.Message.Should().Be("JSend status does not indicate success: \"fail\".");
        }

        [Theory, JSendAutoData]
        public void EnsureSuccessStatus_ReturnsSelf_WhenStatusIsSuccess([WithData] JSendResponse<Model> successResponse)
        {
            // Exercise system
            var response = successResponse.EnsureSuccessStatus();
            // Verify outcome
            response.Should().BeSameAs(successResponse);
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
        [PropertyData("EquivalentResponses")]
        public void TwoResponses_AreEqual_WhenTheirFieldsMatch(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeTrue();
        }

        [Theory]
        [PropertyData("DistinctResponses")]
        public void TwoResponses_AreNotEqual_WhenTheirFieldsDoNotMatch(JSendResponse<string> first,
            JSendResponse<string> second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeFalse();
        }

        [Theory]
        [PropertyData("DistinctResponses")]
        public void TwoResponses_AreNotEqual_WhenTheirFieldsDoNotMatch_AndRightOperandIsCastToBaseClass(
            JSendResponse<string> first, JSendResponse<string> second)
        {
            // Note: If JSendResponse implemented IEquatable<T>, then this test would fail.
            // See http://blog.mischel.com/2013/01/05/inheritance-and-iequatable-do-not-mix/

            // Exercise system and verify outcome
            first.Equals((JSendResponse) second).Should().BeFalse();
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

        [Fact]
        public void ResponseIsNotEqualToNonGenericResponse()
        {
            // Fixture setup
            var generic = new JSendResponse<string>(HttpResponseMessageSingleton);
            var nonGeneric = new JSendResponse(HttpResponseMessageSingleton);
            // Exercise system and verify outcome
            generic.Equals(nonGeneric).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void Equals_IsReflexive(JSendResponse<string> response)
        {
            // Exercise system and verify outcome
            response.Equals(response).Should().BeTrue();
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
        [PropertyData("DistinctResponses")]
        public void Equals_IsSymmetric(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var firstEqualsSecond = first.Equals(second);
            var secondEqualsFirst = second.Equals(first);
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
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
        [PropertyData("DistinctResponses")]
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
        [PropertyData("EquivalentResponses")]
        [PropertyData("DistinctResponses")]
        public void EqualityOperator_IsSymmetric(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var firstEqualsSecond = first == second;
            var secondEqualsFirst = second == first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
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
        [PropertyData("DistinctResponses")]
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
        [PropertyData("EquivalentResponses")]
        [PropertyData("DistinctResponses")]
        public void InequalityOperator_IsSymmetric(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system
            var firstEqualsSecond = first != second;
            var secondEqualsFirst = second != first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [PropertyData("EquivalentResponses")]
        public void EqualResponsesHaveTheSameHashCode(JSendResponse<string> first, JSendResponse<string> second)
        {
            // Exercise system and verify outcome
            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
