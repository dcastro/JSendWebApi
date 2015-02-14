using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    internal class HttpRequestMessageCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<HttpRequestMessage>(
                c => c.OmitAutoProperties());
        }
    }
}
