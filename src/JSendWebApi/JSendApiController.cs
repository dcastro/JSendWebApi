using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using JSendWebApi.Results;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public abstract class JSendApiController : ApiController
    {
        private readonly JsonSerializerSettings _settings;
        private readonly Encoding _encoding;

        protected JSendApiController()
            : this(new JsonSerializerSettings())
        {

        }

        protected JSendApiController(JsonSerializerSettings jsonSerializerSettings)
            : this(
                jsonSerializerSettings,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true))
        {
        }

        protected JSendApiController(JsonSerializerSettings jsonSerializerSettings, Encoding encoding)
        {
            if (jsonSerializerSettings == null) throw new ArgumentNullException("jsonSerializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _settings = jsonSerializerSettings;
            _encoding = encoding;
        }

        public JsonSerializerSettings JsonSerializerSettings
        {
            get { return _settings; }
        }

        public Encoding Encoding
        {
            get { return _encoding; }
        }

        protected internal virtual JSendOkResult JSendOk()
        {
            return new JSendOkResult(this);
        }

        protected internal virtual JSendOkResult<T> JSendOk<T>(T content)
        {
            return new JSendOkResult<T>(this, content);
        }

        protected internal virtual JSendBadRequestResult<string> JSendBadRequest(string reason)
        {
            return new JSendBadRequestResult<string>(this, reason);
        }
    }
}
