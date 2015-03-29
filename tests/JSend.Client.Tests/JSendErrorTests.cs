using System;
using System.Collections.Generic;
using FluentAssertions;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Xunit;
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

        public static IEnumerable<object[]> EquivalentErrors
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, null, null),
                        new JSendError(JSendStatus.Fail, null, null, null)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Error, "a", null, null),
                        new JSendError(JSendStatus.Error, "a", null, null)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, 2, null),
                        new JSendError(JSendStatus.Fail, null, 2, null)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, null, Token),
                        new JSendError(JSendStatus.Fail, null, null, Token)
                    }
                };
            }
        }

        private static readonly JToken Token = new JObject();

        public static IEnumerable<object[]> DistinctErrors
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, null, null),
                        new JSendError(JSendStatus.Error, null, null, null)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Error, "a", null, null),
                        new JSendError(JSendStatus.Error, "b", null, null)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Error, "lasst", null, null),
                        new JSendError(JSendStatus.Error, "laßt", null, null)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, 2, null),
                        new JSendError(JSendStatus.Fail, null, 3, null)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, null, new JObject()),
                        new JSendError(JSendStatus.Fail, null, null, new JObject())
                    }
                };
            }
        }

        [Theory]
        [PropertyData("EquivalentErrors")]
        public void TwoErrors_AreEqual_WhenTheirFieldsMatch(JSendError first, JSendError second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void ErrorIsEqualToItself(JSendError error)
        {
            // Exercise system and verify outcome
            error.Equals(error).Should().BeTrue();
        }

        [Theory]
        [PropertyData("DistinctErrors")]
        public void TwoErrors_AreNotEqual_WhenTheirFieldsDoNotMatch(JSendError first, JSendError second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ErrorIsNotEqualToNull(JSendError error)
        {
            // Exercise system and verify outcome
            error.Equals(null).Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void ErrorIsNotEqualToInstanceOfAnotherType(JSendError error, string other)
        {
            // Exercise system and verify outcome
            error.Equals(other).Should().BeFalse();
        }

        [Theory]
        [PropertyData("EquivalentErrors")]
        public void EqualityOperator_ReturnsTrue_WhenFieldsMatch(JSendError first, JSendError second)
        {
            // Exercise system
            var areEqual = first == second;
            // Verify outcome
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void EqualityOperator_ReturnsTrue_WhenBothErrorsAreNull()
        {
#pragma warning disable 1718
            // Fixture setup
            JSendError error = null;
            // Exercise system
            var areEqual = error == error;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsTrue_WhenBothErrorsAreTheSameInstance(JSendError error)
        {
#pragma warning disable 1718
            // Exercise system
            var areEqual = error == error;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory]
        [PropertyData("DistinctErrors")]
        public void EqualityOperator_ReturnsFalse_WhenFieldsDoNotMatch(JSendError first, JSendError second)
        {
            // Exercise system
            var areEqual = first == second;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsFalse_WhenLeftOperandsIsNull(JSendError error)
        {
            // Exercise system
            var areEqual = null == error;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void EqualityOperator_ReturnsFalse_WhenRightOperandsIsNull(JSendError error)
        {
            // Exercise system
            var areEqual = error == null;
            // Verify outcome
            areEqual.Should().BeFalse();
        }

        [Theory]
        [PropertyData("EquivalentErrors")]
        public void InequalityOperator_ReturnsFalse_WhenFieldsMatch(JSendError first, JSendError second)
        {
            // Exercise system
            var areNotEqual = first != second;
            // Verify outcome
            areNotEqual.Should().BeFalse();
        }

        [Fact]
        public void InequalityOperator_ReturnsFalse_WhenBothErrorsAreNull()
        {
#pragma warning disable 1718
            // Fixture setup
            JSendError error = null;
            // Exercise system
            var areNotEqual = error != error;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsFalse_WhenBothErrorsAreTheSameInstance(JSendError error)
        {
#pragma warning disable 1718
            // Exercise system
            var areNotEqual = error != error;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory]
        [PropertyData("DistinctErrors")]
        public void InequalityOperator_ReturnsTrue_WhenFieldsDoNotMatch(JSendError first, JSendError second)
        {
            // Exercise system
            var areNotEqual = first != second;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsTrue_WhenLeftOperandsIsNull(JSendError error)
        {
            // Exercise system
            var areNotEqual = null != error;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void InequalityOperator_ReturnsTrue_WhenRightOperandsIsNull(JSendError error)
        {
            // Exercise system
            var areNotEqual = error != null;
            // Verify outcome
            areNotEqual.Should().BeTrue();
        }

        [Theory]
        [PropertyData("EquivalentErrors")]
        public void EqualErrorsHaveTheSameHashCode(JSendError first, JSendError second)
        {
            // Exercise system and verify outcome
            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
