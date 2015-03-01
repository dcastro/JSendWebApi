using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using JSend.WebApi.Results;
using Newtonsoft.Json;

namespace JSend.WebApi
{
    /// <summary>Represents an unhandled exception handler that provides JSend error responses.</summary>
    public class JSendExceptionHandler : ExceptionHandler
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        /// <summary>Initializes a new instance of <see cref="JSendExceptionHandler"/>.</summary>
        /// <param name="serializerSettings">The serializer settings used to serialize JSend responses.</param>
        /// <param name="encoding">The encoding used to encode JSend responses.</param>
        public JSendExceptionHandler(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        /// <summary>
        /// Handles an exception by creating a <see cref="JSendExceptionResult"/> with the unhandled exception.
        /// </summary>
        /// <param name="context">The exception handler context.</param>
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var includeErrorDetail = context.Request.ShouldIncludeErrorDetail();

            context.Result = new JSendExceptionResult(
                context.Exception, null, null, null, includeErrorDetail, _serializerSettings, _encoding, context.Request);
        }
    }
}
