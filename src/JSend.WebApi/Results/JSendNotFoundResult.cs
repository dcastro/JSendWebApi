using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="FailResponse"/> with status code <see cref="HttpStatusCode.BadRequest"/>.
    /// </summary>
    public class JSendNotFoundResult : IJSendResult<FailResponse>
    {
        private readonly JSendResult<FailResponse> _result;

        /// <summary>Initializes a new instance of <see cref="JSendInvalidModelStateResult"/>.</summary>
        /// <param name="reason">
        /// The reason why the requested resource could not be found. If none is specified, a default message will be used instead.
        /// </param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendNotFoundResult(string reason, ApiController controller)
        {
            if (reason == null)
                reason = StringResources.NotFound_DefaultMessage;

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(StringResources.NotFound_WhiteSpaceReason, "reason");

            _result = new JSendResult<FailResponse>(HttpStatusCode.NotFound, new FailResponse(reason), controller);
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

        /// <summary>Gets the reason why the requested resource could not be found.</summary>
        public string Reason
        {
            get { return (string) _result.Response.Data; }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
