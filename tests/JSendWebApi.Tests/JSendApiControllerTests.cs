using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Results;
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
            result.Should().BeOfType<JSendOkResult>();
        }
    }
}
