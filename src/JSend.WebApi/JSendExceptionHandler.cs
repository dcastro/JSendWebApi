using System;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using JSend.WebApi.Results;

namespace JSend.WebApi
{
    /// <summary>Represents an unhandled exception handler that provides JSend error responses.</summary>
    public class JSendExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// Handles an exception by creating a <see cref="JSendExceptionResult"/> with the unhandled exception.
        /// </summary>
        /// <param name="context">The exception handler context.</param>
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.RequestContext == null)
                throw new ArgumentException("The exception handler context does not contain a request context",
                    "context");

            var includeErrorDetail = context.Request.ShouldIncludeErrorDetail();
            var formatter = context.RequestContext.Configuration.GetJsonMediaTypeFormatter();
            var request = context.Request;

            context.Result = new JSendExceptionResult(context.Exception, null, null, null,
                includeErrorDetail, formatter, request);
        }
    }
}
