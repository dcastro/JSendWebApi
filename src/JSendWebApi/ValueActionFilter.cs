using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public class ValueActionFilter : ActionFilterAttribute
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        public ValueActionFilter(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var returnType = actionContext.ActionDescriptor.ReturnType;

            if (returnType != null &&
                !returnType.IsGenericParameter &&
                !returnType.IsAssignableFrom(typeof (HttpResponseMessage)) &&
                !returnType.IsAssignableFrom(typeof (IHttpActionResult)))
            {
                Type valueConverterType = typeof (JSendValueResultConverter<>).MakeGenericType(returnType);
                var valueConverter =
                    (IActionResultConverter)
                        Activator.CreateInstance(valueConverterType, _serializerSettings, _encoding);

                actionContext.ActionDescriptor = new DelegatingActionDescriptor(
                    descriptor: actionContext.ActionDescriptor,
                    resultConverter: valueConverter);
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
