using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace JSend.WebApi
{
    /// <summary>
    /// An action filter that selects a <see cref="JSendValueResultConverter{T}"/> as the action result converter for actions
    /// that return an arbitrary T value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ValueActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Selects a <see cref="JSendValueResultConverter{T}"/> as the action result converter for actions
        /// that return an arbitrary T value.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null) throw new ArgumentNullException("actionContext");

            var returnType = actionContext.ActionDescriptor.ReturnType;

            if (returnType != null &&
                !returnType.IsGenericParameter &&
                !typeof (HttpResponseMessage).IsAssignableFrom(returnType) &&
                !typeof (IHttpActionResult).IsAssignableFrom(returnType))
            {
                Type valueConverterType = typeof (JSendValueResultConverter<>).MakeGenericType(returnType);
                var valueConverter =
                    (IActionResultConverter)
                        Activator.CreateInstance(valueConverterType);

                actionContext.ActionDescriptor = new DelegatingActionDescriptor(
                    descriptor: actionContext.ActionDescriptor,
                    resultConverter: valueConverter);
            }
        }
    }
}
