using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace JSendWebApi.Tests
{
    public class TestableJSendApiController : JSendApiController
    {
        public TestableJSendApiController()
        {

        }

        public TestableJSendApiController(JsonSerializerSettings jsonSerializerSettings) : base(jsonSerializerSettings)
        {

        }

        public TestableJSendApiController(JsonSerializerSettings jsonSerializerSettings, Encoding encoding)
            : base(jsonSerializerSettings, encoding)
        {

        }

        public void TestableInitialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }
    }
}
