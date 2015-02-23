using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using Moq;
using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    internal class UrlHelperCustomization : ICustomization
    {
        public const string RouteLink = "http://localhost/models/";

        public void Customize(IFixture fixture)
        {
            var urlFactoryMock = new Mock<UrlHelper>();
            urlFactoryMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                .Returns((string name, IDictionary<string, object> values) => RouteLink);

            fixture.Inject(urlFactoryMock.Object);
        }
    }
}
