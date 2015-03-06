using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http.Controllers;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;

namespace JSend.WebApi
{
    /// <summary>
    /// A converter for creating JSend formatted responses from actions that do not return a value.
    /// </summary>
    public class JSendVoidResultConverter : IActionResultConverter
    {
        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> with status code <see cref="HttpStatusCode.OK"/>
        /// whose body contains a <see cref="SuccessResponse"/>.
        /// </summary>
        /// <param name="actionResult">The action result.</param>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns>The newly created <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Convert(HttpControllerContext controllerContext, object actionResult)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            
            var request = controllerContext.Request;

            var result = new JSendOkResult(request);

            return result.ExecuteAsync(CancellationToken.None).Result;
        }
    }
}
