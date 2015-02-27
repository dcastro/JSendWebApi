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
    /// <summary>
    /// An action filter that selects a <see cref="JSendVoidResultConverter"/> as the action result converter for actions
    /// that do not return a value.
    /// </summary>
    public class VoidActionFilter : ActionFilterAttribute
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        /// <summary>Initializes a new instance of <see cref="VoidActionFilter"/>.</summary>
        /// <param name="serializerSettings">The serializer settings to pass into the action result converter.</param>
        /// <param name="encoding">The encoding to pass into the action result converter.</param>
        public VoidActionFilter(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        /// <summary>
        /// Selects a <see cref="JSendVoidResultConverter"/> as the action result converter for actions
        /// that return an arbitrary T value.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
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
