using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    }
}
