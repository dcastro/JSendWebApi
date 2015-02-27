using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using Newtonsoft.Json;

namespace JSend.WebApi
{
    /// <summary>
    /// A converter for creating JSend formatted responses from actions that do not return a value.
    /// </summary>
    public class JSendVoidResultConverter : IActionResultConverter
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        /// <summary>Initializes a new instance of <see cref="JSendVoidResultConverter"/>.</summary>
        /// <param name="serializerSettings">The serializer settings used to serialize JSend responses.</param>
        /// <param name="encoding">The encoding used to encode JSend responses.</param>
        public JSendVoidResultConverter(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> with status code <see cref="HttpStatusCode.OK"/>
        /// whose body contains a <see cref="SuccessResponse"/>.
        /// </summary>
        /// <param name="actionResult">The action result.</param>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns>The newly created <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Convert(HttpControllerContext controllerContext, object actionResult)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            var result = new JSendOkResult(_serializerSettings, _encoding, controllerContext.Request);

            return result.ExecuteAsync(CancellationToken.None).Result;
        }
    }
}
