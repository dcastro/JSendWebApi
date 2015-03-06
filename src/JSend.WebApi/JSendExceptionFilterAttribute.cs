using System;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using JSend.WebApi.Results;

namespace JSend.WebApi
{
    /// <summary>Represents an unhandled exception handler that provides JSend error responses.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class JSendExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Handles an exception by creating a <see cref="JSendExceptionResult"/> with the unhandled exception.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            if (actionExecutedContext == null) throw new ArgumentNullException("actionExecutedContext");

            Contract.Assert(actionExecutedContext.Exception != null);
            Contract.Assert(actionExecutedContext.ActionContext != null);

            var exception = actionExecutedContext.Exception;
            var includeErrorDetail = actionExecutedContext.Request.ShouldIncludeErrorDetail();
            var formatter =
                actionExecutedContext.ActionContext.ControllerContext.Configuration.GetJsonMediaTypeFormatter();
            var request = actionExecutedContext.Request;

            var result = new JSendExceptionResult(exception, null, null, null,
                includeErrorDetail, formatter, request);

            actionExecutedContext.Response = await result.ExecuteAsync(cancellationToken);
        }
    }
}
