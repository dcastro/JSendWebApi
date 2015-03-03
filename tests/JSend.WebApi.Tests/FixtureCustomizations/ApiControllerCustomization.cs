using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Moq;
using Ploeh.AutoFixture;

namespace JSend.WebApi.Tests.FixtureCustomizations
{
    internal class ApiControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var controller = Mock.Of<ApiController>(ctr =>
                ctr.Request == fixture.Create<HttpRequestMessage>() &&
                ctr.Url == fixture.Create<UrlHelper>() &&
                ctr.Configuration == fixture.Create<HttpConfiguration>());

            fixture.Inject(controller);
        }
    }
}
