using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="FailResponse"/> with status code <see cref="HttpStatusCode.Unauthorized"/>.
    /// </summary>
    public class JSendUnauthorizedResult : IJSendResult<FailResponse>
    {
        private readonly IEnumerable<AuthenticationHeaderValue> _challenges;
        private readonly JSendResult<FailResponse> _result;

        /// <summary>Initializes a new instance of <see cref="JSendUnauthorizedResult"/>.</summary>
        /// <param name="challenges">The WWW-Authenticate challenges.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendUnauthorizedResult(IEnumerable<AuthenticationHeaderValue> challenges, ApiController controller)
            : this(challenges, new JSendResult<FailResponse>.ControllerDependencyProvider(controller))

        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendUnauthorizedResult"/>.</summary>
        /// <param name="challenges">The WWW-Authenticate challenges.</param>
        /// <param name="request">The request message which led to this result.</param>
        public JSendUnauthorizedResult(IEnumerable<AuthenticationHeaderValue> challenges, HttpRequestMessage request)
            : this(challenges, new JSendResult<FailResponse>.RequestDependencyProvider(request))
        {

        }

        private JSendUnauthorizedResult(IEnumerable<AuthenticationHeaderValue> challenges,
            JSendResult<FailResponse>.IDependencyProvider dependencies)
        {
            if (challenges == null) throw new ArgumentNullException("challenges");
            _challenges = challenges;

            var response = new FailResponse(StringResources.RequestNotAuthorized);
            _result = new JSendResult<FailResponse>(HttpStatusCode.Unauthorized, response, dependencies.RequestMessage);
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public FailResponse Response
        {
            get { return _result.Response; }
        }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request
        {
            get { return _result.Request; }
        }

        /// <summary>Gets the WWW-Authenticate challenges.</summary>
        public IEnumerable<AuthenticationHeaderValue> Challenges
        {
            get { return _challenges; }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);

            foreach (var challenge in _challenges)
                message.Headers.WwwAuthenticate.Add(challenge);

            return message;
        }
    }
}
