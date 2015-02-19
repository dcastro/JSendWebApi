using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests
{
    public class JSendMediaTypeFormatterTests
    {
        [Theory, JSendAutoData]
        public async Task WrapsValueInSuccessJSendResponse(
            Model model, [NoAutoProperties] MemoryStream ms, HttpContent httpContent, TransportContext context,
            [NoAutoProperties] JSendMediaTypeFormatter formatter)
        {
            // Fixture setup
            var jsendSuccess = JsonConvert.SerializeObject(new SuccessJSendResponse(model));
            // Exercise system
            await formatter.WriteToStreamAsync(model.GetType(), model, ms, httpContent, context);
            // Verify outcome
            ms.Position = 0;
            using (var sreader = new StreamReader(ms))
            {
                var content = await sreader.ReadToEndAsync();
                content.Should().Be(jsendSuccess);
            }
        }

        [Theory, JSendAutoData]
        public async Task WrapsHttpErrorsWithExceptionDetailsInErrorJSendResponse(
            Exception ex, [NoAutoProperties] MemoryStream ms, HttpContent httpContent, TransportContext context,
            [NoAutoProperties] JSendMediaTypeFormatter formatter)
        {
            // Fixture setup
            var httpError = new HttpError(ex, includeErrorDetail: true);
            var jsendError = JsonConvert.SerializeObject(new ErrorJSendResponse(ex.Message, httpError));
            // Exercise system
            await formatter.WriteToStreamAsync(httpError.GetType(), httpError, ms, httpContent, context);
            // Verify outcome
            ms.Position = 0;
            using (var sreader = new StreamReader(ms))
            {
                var content = await sreader.ReadToEndAsync();
                content.Should().Be(jsendError);
            }
        }

        [Theory, JSendAutoData]
        public async Task WrapsHttpErrorsWithoutExceptionDetailsInErrorJSendResponse(
            Exception ex, [NoAutoProperties] MemoryStream ms, HttpContent httpContent, TransportContext context,
            [NoAutoProperties] JSendMediaTypeFormatter formatter)
        {
            // Fixture setup
            var httpError = new HttpError(ex, includeErrorDetail: false);
            var jsendError = JsonConvert.SerializeObject(new ErrorJSendResponse(httpError.Message));
            // Exercise system
            await formatter.WriteToStreamAsync(httpError.GetType(), httpError, ms, httpContent, context);
            // Verify outcome
            ms.Position = 0;
            using (var sreader = new StreamReader(ms))
            {
                var content = await sreader.ReadToEndAsync();
                content.Should().Be(jsendError);
            }
        }
    }
}
