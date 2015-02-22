using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using JSendWebApi.Results;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public class JSendVoidResultConverter : IActionResultConverter
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        public JSendVoidResultConverter(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        public HttpResponseMessage Convert(HttpControllerContext controllerContext, object actionResult)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            var result = new JSendOkResult(_serializerSettings, _encoding, controllerContext.Request);

            return result.ExecuteAsync(CancellationToken.None).Result;
        }
    }
}
