using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSendWebApi.Tests
{
    internal class TestableJSendApiController : JSendApiController
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
    }
}
