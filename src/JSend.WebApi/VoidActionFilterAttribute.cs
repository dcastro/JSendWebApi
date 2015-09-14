using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace JSend.WebApi
{
    /// <summary>
    /// An action filter that selects a <see cref="JSendVoidResultConverter"/> as the action result converter for actions
    /// that do not return a value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class VoidActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Selects a <see cref="JSendVoidResultConverter"/> as the action result converter for actions
        /// that do not return a value.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null) throw new ArgumentNullException(nameof(actionContext));

            var returnType = actionContext.ActionDescriptor.ReturnType;

            if (returnType == null)
            {
                actionContext.ActionDescriptor = new DelegatingActionDescriptor(
                    descriptor: actionContext.ActionDescriptor,
                    resultConverter: new JSendVoidResultConverter());
            }
        }
    }
}
