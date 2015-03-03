using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ploeh.AutoFixture;

namespace JSend.WebApi.Tests.FixtureCustomizations
{
    public class JsonMediaTypeFormatterCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<JsonMediaTypeFormatter>(
                c => c.OmitAutoProperties());
        }
    }
}
