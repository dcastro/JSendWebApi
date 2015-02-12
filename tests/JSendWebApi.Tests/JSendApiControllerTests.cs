using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Results;
using JSendWebApi.Tests.TestClasses;
using Xunit;

namespace JSendWebApi.Tests
{
    public class JSendApiControllerTests
    {
        [Fact]
        public void JSendApiControllerIsApiController()
        {
            // Fixture setup
            // Exercise system
            var controller = new TestableJSendApiController();
            // Verify outcome
            controller.Should().BeAssignableTo<ApiController>();
        }

        [Fact]
        public void JSendOkReturnsJSendOkResult()
        {
            // Fixture setup
            var controller = new TestableJSendApiController();
            // Exercise system
            var result = controller.JSendOk();
            // Verify outcome
            result.Should().BeAssignableTo<JSendOkResult>();
        }

        [Fact]
        public void JSendOkWithContentReturnsJSendOkResult()
        {
            // Fixture setup
            var controller = new TestableJSendApiController();
            var model = new Model();
            // Exercise system
            var result = controller.JSendOk(model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendOkResult<Model>>();
        }
    }
}
