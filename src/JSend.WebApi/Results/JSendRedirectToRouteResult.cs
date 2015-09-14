using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using JSend.WebApi.Extensions;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that performs route generation and returns a <see cref="SuccessResponse"/>
    /// with status code <see cref="HttpStatusCode.Redirect"/>.
    /// </summary>
    public class JSendRedirectToRouteResult : IJSendResult<SuccessResponse>
    {
        private static readonly SuccessResponse SuccessResponse = new SuccessResponse();

        private readonly ApiController _controller;
        private readonly JSendResult<SuccessResponse> _result;

        private UrlHelper _urlFactory;

        /// <summary>Initializes a new instance of <see cref="JSendRedirectToRouteResult"/>.</summary>
        /// <param name="routeName">The name of the route to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2",
            Justification = "The parameter controller is validated by JSendResult<T>'s constructor.")]
        public JSendRedirectToRouteResult(string routeName, IDictionary<string, object> routeValues,
            ApiController controller)
        {
            if (routeName == null) throw new ArgumentNullException(nameof(routeName));

            RouteName = routeName;
            RouteValues = routeValues;
            _controller = controller;

            _result = new JSendResult<SuccessResponse>(HttpStatusCode.Redirect, SuccessResponse, controller);
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public SuccessResponse Response => _result.Response;

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode => _result.StatusCode;

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request => _result.Request;

        /// <summary>Gets the name of the route to use for generating the URL.</summary>
        public string RouteName { get; }

        /// <summary>Gets the route data to use for generating the URL.</summary>
        public IDictionary<string, object> RouteValues { get; }

        /// <summary>Gets the factory to use to generate the route URL.</summary>
        public UrlHelper UrlFactory
        {
            get
            {
                if (_urlFactory == null)
                    _urlFactory = _controller.Url ?? new UrlHelper(_controller.GetRequestOrThrow());

                return _urlFactory;
            }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            string link = UrlFactory.Link(RouteName, RouteValues);

            var message = await _result.ExecuteAsync(cancellationToken);
            message.Headers.Location = new Uri(link);
            return message;
        }
    }
}
