using System;
using System.Net.Http;
using System.Reflection;
using FluentAssertions;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
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
            data.ShouldThrow<JSendResponseException>()
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
            data.ShouldThrow<JSendResponseException>()
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
                .ShouldThrow<JSendResponseException>()
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
    }
}
