using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="SuccessResponse"/> with status code <see cref="HttpStatusCode.Redirect"/>.
    /// </summary>
    public class JSendRedirectResult : IHttpActionResult
    {
        private readonly Uri _location;
        private readonly JSendResult<SuccessResponse> _result;

        /// <summary>Initializes a new instance of <see cref="JSendRedirectResult"/>.</summary>
        /// <param name="location">The location to which to redirect.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendRedirectResult(Uri location, JSendApiController controller)
        {
            if (location == null) throw new ArgumentNullException("location");

            _location = location;
            _result = new JSendResult<SuccessResponse>(HttpStatusCode.Redirect, new SuccessResponse(), controller);
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
