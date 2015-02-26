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
        public void TestableInitialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }
    }
}
