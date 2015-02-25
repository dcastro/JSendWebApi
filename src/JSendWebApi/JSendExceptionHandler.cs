using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using JSendWebApi.Results;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public class JSendExceptionHandler : ExceptionHandler
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        public JSendExceptionHandler(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            var includeErrorDetail = context.Request.ShouldIncludeErrorDetail();

            context.Result = new JSendExceptionResult(
                context.Exception, null, null, null, includeErrorDetail, _serializerSettings, _encoding, context.Request);
        }
    }
}
