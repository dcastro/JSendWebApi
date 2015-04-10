using System;
using System.Collections.Generic;
using FluentAssertions;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Xunit;

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

        [Theory, JSendAutoData]
        public void PropertiesAreCorrectlyInitialized(IFixture fixture, ConstructorInitializedMemberAssertion assertion)
        {
            // Fixture setup
            fixture.Inject(JSendStatus.Error);
            var properties = typeof (JSendError).GetProperties();
            // Exercise system and verify outcome
            assertion.Verify(properties);
        }

        private static readonly JToken JTokenSingleton = new JObject();

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
                        new JSendError(JSendStatus.Fail, null, null, JTokenSingleton),
                        new JSendError(JSendStatus.Fail, null, null, JTokenSingleton)
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, null, new JObject()),
                        new JSendError(JSendStatus.Fail, null, null, new JObject())
                    },
                    new object[]
                    {
                        new JSendError(JSendStatus.Fail, null, null, new JObject {{"key", "a"}}),
                        new JSendError(JSendStatus.Fail, null, null, new JObject {{"key", "a"}})
                    }
                };
            }
        }

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

                        // Here, we're verifying that these two strings are treated as different.
                        // This requires the usage of StringComparison.Ordinal.
                        // StringComparison.InvariantCulture would treat both strings as equals, and that's not what we want.
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
                        new JSendError(JSendStatus.Fail, null, null, new JObject {{"key", "a"}}),
                        new JSendError(JSendStatus.Fail, null, null, new JObject {{"key", "b"}})
                    }
                };
            }
        }

        [Theory]
        [MemberData("EquivalentErrors")]
        public void TwoErrors_AreEqual_WhenTheirFieldsMatch(JSendError first, JSendError second)
        {
            // Exercise system and verify outcome
            first.Equals(second).Should().BeTrue();
        }

        [Theory]
        [MemberData("DistinctErrors")]
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

        [Theory, JSendAutoData]
        public void Equals_IsReflexive(JSendError error)
        {
            // Exercise system and verify outcome
            error.Equals(error).Should().BeTrue();
        }

        [Theory]
        [MemberData("EquivalentErrors")]
        [MemberData("DistinctErrors")]
        public void Equals_IsSymmetric(JSendError first, JSendError second)
        {
            // Exercise system
            var firstEqualsSecond = first.Equals(second);
            var secondEqualsFirst = second.Equals(first);
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [MemberData("EquivalentErrors")]
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

        [Theory]
        [MemberData("DistinctErrors")]
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

        [Theory, JSendAutoData]
        public void EqualityOperator_IsReflexive(JSendError error)
        {
#pragma warning disable 1718
            // Exercise system
            var areEqual = error == error;
            // Verify outcome
            areEqual.Should().BeTrue();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
        [MemberData("EquivalentErrors")]
        [MemberData("DistinctErrors")]
        public void EqualityOperator_IsSymmetric(JSendError first, JSendError second)
        {
            // Exercise system
            var firstEqualsSecond = first == second;
            var secondEqualsFirst = second == first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [MemberData("EquivalentErrors")]
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

        [Theory]
        [MemberData("DistinctErrors")]
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

        [Theory, JSendAutoData]
        public void InequalityOperator_IsReflexive(JSendError error)
        {
#pragma warning disable 1718
            // Exercise system
            var areNotEqual = error != error;
            // Verify outcome
            areNotEqual.Should().BeFalse();
#pragma warning restore 1718
        }

        [Theory, JSendAutoData]
        [MemberData("EquivalentErrors")]
        [MemberData("DistinctErrors")]
        public void InequalityOperator_IsSymmetric(JSendError first, JSendError second)
        {
            // Exercise system
            var firstEqualsSecond = first != second;
            var secondEqualsFirst = second != first;
            // Verify outcome
            firstEqualsSecond.Should().Be(secondEqualsFirst);
        }

        [Theory]
        [MemberData("EquivalentErrors")]
        public void EqualErrorsHaveTheSameHashCode(JSendError first, JSendError second)
        {
            // Exercise system and verify outcome
            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
