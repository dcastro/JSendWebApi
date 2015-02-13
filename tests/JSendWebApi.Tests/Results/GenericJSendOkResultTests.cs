using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Results;
using JSendWebApi.Tests.TestClasses;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace JSendWebApi.Tests.Results
{
    public class GenericJSendOkResultTests
    {
        [Fact]
        public void IsHttpActionResult()
        {
            // Fixture setup
            var controller = new TestableJSendApiController();
            var model = new Model();
            // Exercise system
            var result = new JSendOkResult<Model>(controller, model);
            // Verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Fact]
        public async Task ExecuteAsyncReturnsSuccessJSendResponse()
        {
            // Fixture setup
            var controller = new TestableJSendApiController {Request = new HttpRequestMessage()};
            var model = new Model {Name = "test"};

            var jsendSuccess = JsonConvert.SerializeObject(new SuccessJSendResponse<Model>(model));
            var result = new JSendOkResult<Model>(controller, model);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }

        [Fact]
        public async Task SetsStatusCodeTo200()
        {
            // Fixture setup
            var controller = new TestableJSendApiController {Request = new HttpRequestMessage()};
            var model = new Model {Name = "test"};
            var result = new JSendOkResult<Model>(controller, model);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task SetsCharSetHeader()
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            var model = new Model();
            var controller = new TestableJSendApiController(new JsonSerializerSettings(), encoding)
            {
                Request = new HttpRequestMessage()
            };
            var result = new JSendOkResult<Model>(controller, model);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet = encoding.WebName;
        }

        [Fact]
        public async Task SetsContentTypeHeader()
        {
            // Fixture setup
            var model = new Model();
            var controller = new TestableJSendApiController { Request = new HttpRequestMessage() };
            var result = new JSendOkResult<Model>(controller, model);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
