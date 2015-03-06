using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using JSend.WebApi.Results;

namespace JSend.WebApi
{
    /// <summary>
    /// An authorization filter that verifies the request's <see cref="IPrincipal"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "We want to support extensibility")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class JSendAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Processes requests that fail authorization by creating a JSend formatted <see cref="HttpStatusCode.Unauthorized"/> result.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null) throw new ArgumentNullException("actionContext");
            Contract.Assert(actionContext.ControllerContext != null);

            var result = new JSendUnauthorizedResult(
                Enumerable.Empty<AuthenticationHeaderValue>(), actionContext.Request);

            actionContext.Response = result.ExecuteAsync(CancellationToken.None).Result;
        }
    }
}
