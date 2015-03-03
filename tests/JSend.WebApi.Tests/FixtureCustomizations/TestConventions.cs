using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace JSend.WebApi.Tests.FixtureCustomizations
{
    internal class TestConventions : CompositeCustomization
    {
        public TestConventions()
            : base(
                new JSendApiControllerCustomization(),
                new ApiControllerCustomization(),
                new JsonMediaTypeFormatterCustomization(),
                new UrlHelperCustomization(),
                new RandomHttpStatusCodeCustomization(),
                new AutoConfiguredMoqCustomization())
        {

        }
    }
}
