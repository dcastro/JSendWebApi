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
        private static readonly FailResponse FailResponse = new FailResponse(StringResources.RequestNotAuthorized);

        private readonly JSendResult<FailResponse>.IDependencyProvider _dependencies;

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
            if (challenges == null) throw new ArgumentNullException(nameof(challenges));

            Challenges = challenges;
            _dependencies = dependencies;
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public FailResponse Response => FailResponse;

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request => _dependencies.RequestMessage;

        /// <summary>Gets the WWW-Authenticate challenges.</summary>
        public IEnumerable<AuthenticationHeaderValue> Challenges { get; }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = new JSendResult<FailResponse>(StatusCode, Response, Request);

            var message = await result.ExecuteAsync(cancellationToken);

            foreach (var challenge in Challenges)
                message.Headers.WwwAuthenticate.Add(challenge);

            return message;
        }
    }
}
