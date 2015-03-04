using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;

namespace JSend.WebApi
{
    /// <summary>
    /// Defines methods to easily create JSend formatted responses.
    /// </summary>
    public abstract class JSendApiController : ApiController
    {
        /// <summary>
        /// Initializes the <see cref="JSendApiController"/> instance with the specified <paramref name="controllerContext"/>.
        /// </summary>
        /// <param name="controllerContext">The <see cref="HttpControllerContext"/> object that is used for the initialization.</param>
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            Configuration.Services.Replace(typeof (IExceptionHandler),
                new JSendExceptionHandler());

            Configuration.Filters.Add(
                new ValueActionFilter());

            Configuration.Filters.Add(
                new VoidActionFilter());
        }

        /// <summary>Creates a <see cref="JSendOkResult"/> (200 OK).</summary>
        /// <returns>A <see cref="JSendOkResult"/>.</returns>
        protected internal virtual JSendOkResult JSendOk()
        {
            return new JSendOkResult(this);
        }

        /// <summary>Creates a <see cref="JSendOkResult{T}"/> with the specified content.</summary>
        /// <typeparam name="T">The type of the content in the entity body.</typeparam>
        /// <param name="content">The content value to format in the entity body.</param>
        /// <returns>A <see cref="JSendOkResult{T}"/> with the specified content.</returns>
        protected internal virtual JSendOkResult<T> JSendOk<T>(T content)
        {
            return new JSendOkResult<T>(content, this);
        }

        /// <summary>Creates a <see cref="JSendBadRequestResult"/> (400 Bad Request) with the specified error message.</summary>
        /// <param name="reason">The reason why the request could not be processed.</param>
        /// <returns>A <see cref="JSendBadRequestResult"/> with the specified error message.</returns>
        protected internal virtual JSendBadRequestResult JSendBadRequest(string reason)
        {
            return new JSendBadRequestResult(reason, this);
        }

        /// <summary>Creates a <see cref="JSendInvalidModelStateResult"/> (400 Bad Request) with the specified model state.</summary>
        /// <param name="modelState">The invalid model state to include in the response's body as key-value pairs.</param>
        /// <returns>A <see cref="JSendInvalidModelStateResult"/> with the specified model state.</returns>
        protected internal virtual JSendInvalidModelStateResult JSendBadRequest(ModelStateDictionary modelState)
        {
            return new JSendInvalidModelStateResult(modelState, this);
        }

        /// <summary>Creates a <see cref="JSendCreatedResult{T}"/> (201 Created) with the specified values.</summary>
        /// <typeparam name="T">The type of the content in the entity body.</typeparam>
        /// <param name="location">The location at which the content has been created.</param>
        /// <param name="content">The content value to format in the entity body.</param>
        /// <returns>A <see cref="JSendCreatedResult{T}"/> with the specified values.</returns>
        protected internal virtual JSendCreatedResult<T> JSendCreated<T>(Uri location, T content)
        {
            return new JSendCreatedResult<T>(location, content, this);
        }

        /// <summary>Creates a <see cref="JSendCreatedResult{T}"/> (201 Created) with the specified values.</summary>
        /// <typeparam name="T">The type of the content in the entity body.</typeparam>
        /// <param name="location">The location at which the content has been created. Must be a relative or absolute URL.</param>
        /// <param name="content">The content value to format in the entity body.</param>
        /// <returns>A <see cref="JSendCreatedResult{T}"/> with the specified values.</returns>
        protected internal virtual JSendCreatedResult<T> JSendCreated<T>(string location, T content)
        {
            if (location == null) throw new ArgumentNullException("location");

            return JSendCreated(new Uri(location, UriKind.RelativeOrAbsolute), content);
        }

        /// <summary>Creates a <see cref="JSendCreatedAtRouteResult{T}"/> (201 Created) with the specified values.</summary>
        /// <typeparam name="T">The type of the content in the entity body.</typeparam>
        /// <param name="routeName">The name of the route to use to generate the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="content">The content value to format in the entity body.</param>
        /// <returns>A <see cref="JSendCreatedAtRouteResult{T}"/> with the specified values.</returns>
        protected internal virtual JSendCreatedAtRouteResult<T> JSendCreatedAtRoute<T>(string routeName,
            IDictionary<string, object> routeValues, T content)
        {
            return new JSendCreatedAtRouteResult<T>(routeName, routeValues, content, this);
        }

        /// <summary>Creates a <see cref="JSendCreatedAtRouteResult{T}"/> (201 Created) with the specified values.</summary>
        /// <typeparam name="T">The type of the content in the entity body.</typeparam>
        /// <param name="routeName">The name of the route to use to generate the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="content">The content value to format in the entity body.</param>
        /// <returns>A <see cref="JSendCreatedAtRouteResult{T}"/> with the specified values.</returns>
        protected internal virtual JSendCreatedAtRouteResult<T> JSendCreatedAtRoute<T>(string routeName,
            object routeValues, T content)
        {
            return JSendCreatedAtRoute(routeName, new HttpRouteValueDictionary(routeValues), content);
        }

        /// <summary>
        /// Creates a <see cref="JSendInternalServerErrorResult"/> (500 Internal Server Error) with the specified error message
        /// and an optional error code and additional data.
        /// </summary>
        /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.</param>
        /// <param name="errorCode">A numeric code corresponding to the error, if applicable.</param>
        /// <param name="data"> An optional generic container for any other information about the error.</param>
        /// <returns>A <see cref="JSendInternalServerErrorResult"/> with the specified values.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
            Justification = "The default values will never change. Furthermore, since C#, F# and VB.NET support optional parameters, it's acceptable to force other CLS-compliant languages that don't support this feature to explicitly provide arguments for each default parameter.")]
        protected internal virtual JSendInternalServerErrorResult JSendInternalServerError(string message,
            int? errorCode = null, object data = null)
        {
            return new JSendInternalServerErrorResult(message, errorCode, data, this);
        }

        /// <summary>
        /// Creates a <see cref="JSendExceptionResult"/> (500 Internal Server Error) with the specified exception
        /// and an optional error message, error code and additional data.
        /// </summary>
        /// <param name="exception">The exception to include in the error.</param>
        /// <param name="message">
        /// An optional meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.
        /// If none is provided, and if <see cref="HttpRequestContext.IncludeErrorDetail"/> is set to <see langword="true"/>,
        /// the exception's message will be used instead.
        /// </param>
        /// <param name="errorCode">
        /// A numeric code corresponding to the error, if applicable.
        /// </param>
        /// <param name="data">
        /// An optional generic container for any other information about the error.
        /// If none is provided, and if <see cref="HttpRequestContext.IncludeErrorDetail"/> is set to <see langword="true"/>,
        /// the exception's details will be used instead.
        /// </param>
        /// <returns>A <see cref="JSendExceptionResult"/> with the specified values.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
            Justification = "The default values will never change. Furthermore, since C#, F# and VB.NET support optional parameters, it's acceptable to force other CLS-compliant languages that don't support this feature to explicitly provide arguments for each default parameter.")]
        protected internal virtual JSendExceptionResult JSendInternalServerError(Exception exception, string message = null,
            int? errorCode = null, object data = null)
        {
            return new JSendExceptionResult(exception, message, errorCode, data, this);
        }

        /// <summary>Creates a <see cref="JSendNotFoundResult"/> (404 Not Found) with the specified error message.</summary>
        /// <param name="reason">The reason why the requested resource could not be found.</param>
        /// <returns>A <see cref="JSendNotFoundResult"/> with the specified error message.</returns>
        protected internal virtual JSendNotFoundResult JSendNotFound(string reason)
        {
            return new JSendNotFoundResult(reason, this);
        }

        /// <summary>Creates a <see cref="JSendNotFoundResult"/> (404 Not Found).</summary>
        /// <returns>A <see cref="JSendNotFoundResult"/>.</returns>
        protected internal virtual JSendNotFoundResult JSendNotFound()
        {
            return JSendNotFound(null);
        }

        /// <summary>Creates a <see cref="JSendRedirectResult"/> (302 Found) with the specified location.</summary>
        /// <param name="location">The location to which to redirect.</param>
        /// <returns>A <see cref="JSendRedirectResult"/> with the specified location.</returns>
        protected internal virtual JSendRedirectResult JSendRedirect(Uri location)
        {
            return new JSendRedirectResult(location, this);
        }

        /// <summary>Creates a <see cref="JSendRedirectResult"/> (302 Found) with the specified location.</summary>
        /// <param name="location">The location to which to redirect.</param>
        /// <returns>A <see cref="JSendRedirectResult"/> with the specified location.</returns>
        protected internal virtual JSendRedirectResult JSendRedirect(string location)
        {
            return JSendRedirect(new Uri(location));
        }

        /// <summary>Creates a <see cref="JSendRedirectToRouteResult"/> (302 Found) with the specified values.</summary>
        /// <param name="routeName">The name of the route to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <returns>A <see cref="JSendRedirectToRouteResult"/> with the specified values.</returns>
        protected internal virtual JSendRedirectToRouteResult JSendRedirectToRoute(string routeName,
            IDictionary<string, object> routeValues)
        {
            return new JSendRedirectToRouteResult(routeName, routeValues, this);
        }

        /// <summary>Creates a <see cref="JSendRedirectToRouteResult"/> (302 Found) with the specified values.</summary>
        /// <param name="routeName">The name of the route to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <returns>A <see cref="JSendRedirectToRouteResult"/> with the specified values.</returns>
        protected internal virtual JSendRedirectToRouteResult JSendRedirectToRoute(string routeName,
            object routeValues)
        {
            return JSendRedirectToRoute(routeName, new HttpRouteValueDictionary(routeValues));
        }

        /// <summary>Creates a <see cref="JSendUnauthorizedResult"/> (401 Unauthorized) with the specified challenge headers.</summary>
        /// <param name="challenges">The WWW-Authenticate challenges.</param>
        /// <returns>A <see cref="JSendUnauthorizedResult"/> with the specified values.</returns>
        protected internal virtual JSendUnauthorizedResult JSendUnauthorized(
            IEnumerable<AuthenticationHeaderValue> challenges)
        {
            return new JSendUnauthorizedResult(challenges, this);
        }

        /// <summary>Creates a <see cref="JSendUnauthorizedResult"/> (401 Unauthorized) with the specified challenge headers.</summary>
        /// <param name="challenges">The WWW-Authenticate challenges.</param>
        /// <returns>A <see cref="JSendUnauthorizedResult"/> with the specified values.</returns>
        protected internal virtual JSendUnauthorizedResult JSendUnauthorized(
            params AuthenticationHeaderValue[] challenges)
        {
            return JSendUnauthorized(challenges.AsEnumerable());
        }

        /// <summary>Creates a <see cref="JSendResult{TResponse}"/> with the specified status code and JSend response.</summary>
        /// <typeparam name="TResponse">The type of the JSend response in the entity body.</typeparam>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="response">The JSend response to format in the entity body.</param>
        /// <returns>A <see cref="JSendResult{TResponse}"/> with the specified values.</returns>
        protected internal virtual JSendResult<TResponse> JSend<TResponse>(HttpStatusCode statusCode, TResponse response)
            where TResponse : IJSendResponse
        {
            return new JSendResult<TResponse>(statusCode, response, this);
        }

        /// <summary>Creates a <see cref="JSendResult{TResponse}"/> with a <see cref="SuccessResponse"/> and the specified status code and data.</summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="data">The data value to format in the entity body.</param>
        /// <returns>A <see cref="JSendResult{TResponse}"/> with a <see cref="SuccessResponse"/> and the specified values.</returns>
        protected internal virtual JSendResult<SuccessResponse> JSendSuccess(HttpStatusCode statusCode, object data)
        {
            return JSend(statusCode, new SuccessResponse(data));
        }

        /// <summary>Creates a <see cref="JSendResult{TResponse}"/> with a <see cref="FailResponse"/> and the specified status code and data.</summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="data">The data value to format in the entity body.</param>
        /// <returns>A <see cref="JSendResult{TResponse}"/> with a <see cref="FailResponse"/> and the specified values.</returns>
        protected internal virtual JSendResult<FailResponse> JSendFail(HttpStatusCode statusCode, object data)
        {
            return JSend(statusCode, new FailResponse(data));
        }

        /// <summary>
        /// Creates a <see cref="JSendResult{TResponse}"/> with a <see cref="ErrorResponse"/> and
        /// the specified status code, error message, error code and data.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.</param>
        /// <param name="errorCode">A numeric code corresponding to the error, if applicable.</param>
        /// <param name="data"> An optional generic container for any other information about the error.</param>
        /// <returns>A <see cref="JSendResult{TResponse}"/> with a <see cref="ErrorResponse"/> and the specified values.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
            Justification = "The default values will never change. Furthermore, since C#, F# and VB.NET support optional parameters, it's acceptable to force other CLS-compliant languages that don't support this feature to explicitly provide arguments for each default parameter.")]
        protected internal virtual JSendResult<ErrorResponse> JSendError(HttpStatusCode statusCode, string message,
            int? errorCode = null, object data = null)
        {
            return JSend(statusCode, new ErrorResponse(message, errorCode, data));
        }
    }
}
