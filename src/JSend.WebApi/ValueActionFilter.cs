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

namespace JSend.WebApi
{
    /// <summary>
    /// An action filter that selects a <see cref="JSendValueResultConverter{T}"/> as the action result converter for actions
    /// that return an arbitrary T value.
    /// </summary>
    [CLSCompliant(false)]
    public class ValueActionFilter : ActionFilterAttribute
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Encoding _encoding;

        /// <summary>Initializes a new instance of <see cref="ValueActionFilter"/>.</summary>
        /// <param name="serializerSettings">The serializer settings to pass into the action result converter.</param>
        /// <param name="encoding">The encoding to pass into the action result converter.</param>
        public ValueActionFilter(JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            if (serializerSettings == null) throw new ArgumentNullException("serializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _serializerSettings = serializerSettings;
            _encoding = encoding;
        }

        /// <summary>
        /// Selects a <see cref="JSendValueResultConverter{T}"/> as the action result converter for actions
        /// that return an arbitrary T value.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            var returnType = actionContext.ActionDescriptor.ReturnType;

            if (returnType != null &&
                !returnType.IsGenericParameter &&
                !typeof (HttpResponseMessage).IsAssignableFrom(returnType) &&
                !typeof (IHttpActionResult).IsAssignableFrom(returnType))
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
