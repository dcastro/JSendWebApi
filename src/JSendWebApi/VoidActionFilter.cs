using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public class VoidActionFilter : ActionFilterAttribute
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        public VoidActionFilter(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var returnType = actionContext.ActionDescriptor.ReturnType;

            if (returnType == null)
            {
                actionContext.ActionDescriptor = new DelegatingActionDescriptor(
                    descriptor: actionContext.ActionDescriptor,
                    resultConverter: new JSendVoidResultConverter(_serializerSettings, _encoding));
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
