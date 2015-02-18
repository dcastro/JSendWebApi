using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;

namespace JSendWebApi
{
    public class JSendMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            value = new SuccessJSendResponse(value);

            return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
        }
    }
}
