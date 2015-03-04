using System.Web.Http.Controllers;

namespace JSend.WebApi.Tests
{
    public class TestableJSendApiController : JSendApiController
    {
        public void TestableInitialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }
    }
}
