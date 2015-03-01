using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that performs route generation and returns a <see cref="SuccessResponse"/>
    /// with status code <see cref="HttpStatusCode.Redirect"/>.
    /// </summary>
    public class JSendRedirectToRouteResult : IHttpActionResult
    {
        private readonly JSendResult<SuccessResponse> _result;
        private readonly Uri _location;

        /// <summary>Initializes a new instance of <see cref="JSendRedirectToRouteResult"/>.</summary>
        /// <param name="routeName">The name of the route to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "The parameter controller is validated by JSendResult<T>'s constructor.")]
        public JSendRedirectToRouteResult(string routeName, IDictionary<string, object> routeValues,
            JSendApiController controller)
        {
            _result = new JSendResult<SuccessResponse>(HttpStatusCode.Redirect, new SuccessResponse(), controller);

            UrlHelper urlFactory = controller.Url ?? new UrlHelper(controller.Request);

            string link = urlFactory.Link(routeName, routeValues);

            _location = new Uri(link);
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public SuccessResponse Response
        {
            get { return _result.Response; }
        }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        /// <summary>Gets the location to which to redirect.</summary>
        public Uri Location
        {
            get { return _location; }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);
            message.Headers.Location = _location;
            return message;
        }
    }
}
