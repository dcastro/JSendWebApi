using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using JSendWebApi.Results;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public abstract class JSendApiController : ApiController
    {
        private JsonSerializerSettings _settings;
        private Encoding _encoding;

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
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _settings = value;
            }
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _encoding = value;
            }
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

        protected internal virtual JSendInvalidModelStateResult JSendBadRequest(ModelStateDictionary modelState)
        {
            return new JSendInvalidModelStateResult(this, modelState);
        }
    }
}
