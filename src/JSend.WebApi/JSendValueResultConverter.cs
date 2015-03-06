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
    /// A converter for creating JSend formatted responses from actions that return an arbitrary T value.
    /// </summary>
    /// <typeparam name="T">The declared return type of an action.</typeparam>
    public class JSendValueResultConverter<T> : IActionResultConverter
    {
        /// <summary>
        /// Converts the specified <paramref name="actionResult"/> object to a <see cref="HttpResponseMessage"/>
        /// with status code <see cref="HttpStatusCode.OK"/> whose body contains a <see cref="SuccessResponse"/>.
        /// </summary>
        /// <param name="actionResult">The action result.</param>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns>The newly created <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Convert(HttpControllerContext controllerContext, object actionResult)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            var request = controllerContext.Request;

            T value = (T) actionResult;
            var result = new JSendOkResult<T>(value, request);

            return result.ExecuteAsync(CancellationToken.None).Result;
        }
    }
}
