using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace JSend.WebApi.Tests.FixtureCustomizations
{
    internal class TestConventions : CompositeCustomization
    {
        public TestConventions()
            : base(
                new AutoConfiguredMoqCustomization(),
                new HttpRequestContextCustomization(),
                new HttpResquestMessageCustomization(),
                new JSendApiControllerCustomization(),
                new JsonMediaTypeFormatterCustomization(),
                new UrlHelperCustomization(),
                new RandomHttpStatusCodeCustomization(),
                new RecursionCustomization())
        {

        }

        private class HttpRequestContextCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                //we freeze the HttpRequestContext so that the same context is injected into the controller and the request object
                //otherwise, a conflict would happen and an InvalidOperationException would be thrown
                var requestContext = fixture.Build<HttpRequestContext>()
                    .Without(x => x.ClientCertificate)
                    .Create();

                fixture.Inject(requestContext);
            }
        }

        private class HttpResquestMessageCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<HttpRequestMessage>(
                    c => c.Do(
                        req => req.SetRequestContext(fixture.Create<HttpRequestContext>())));
            }
        }

        private class JSendApiControllerCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new TypeRelay(
                        typeof (JSendApiController),
                        typeof (TestableJSendApiController)));
            }
        }

        private class JsonMediaTypeFormatterCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<JsonMediaTypeFormatter>(
                    c => c.OmitAutoProperties());
            }
        }

        internal const string RouteLink = "http://localhost/models/";

        private class UrlHelperCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var urlFactoryMock = new Mock<UrlHelper>();
                urlFactoryMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                    .Returns((string name, IDictionary<string, object> values) => RouteLink);

                fixture.Inject(urlFactoryMock.Object);
            }
        }

        private class RandomHttpStatusCodeCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var rnd = new Random();
                fixture.Register<HttpStatusCode>(() => Generate(rnd));
            }

            private static HttpStatusCode Generate(Random rnd)
            {
                var values = Enum.GetValues(typeof (HttpStatusCode));
                return (HttpStatusCode) values.GetValue(rnd.Next(values.Length));
            }
        }

        private class RecursionCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                // There are many circular dependencies within the WebApi framework
                // (e.g., HttpActionDescriptor - HttpActionBinding - HttpActionDescriptor)
                // so we have to allow AutoFixture to create circular dependencies
                // (but only resolve the first level).
                fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));

                fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            }
        }
    }
}
