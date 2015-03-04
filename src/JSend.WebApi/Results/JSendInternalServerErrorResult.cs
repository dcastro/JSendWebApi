using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="ErrorResponse"/> containing the specified error details
    /// with status code <see cref="HttpStatusCode.InternalServerError"/>.
    /// </summary>
    public class JSendInternalServerErrorResult : IHttpActionResult
    {
        private readonly JSendResult<ErrorResponse> _result;

        /// <summary>Initializes a new instance of <see cref="JSendExceptionResult"/>.</summary>
        /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.</param>
        /// <param name="errorCode">A numeric code corresponding to the error, if applicable.</param>
        /// <param name="data"> An optional generic container for any other information about the error.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendInternalServerErrorResult(string message, int? errorCode, object data, ApiController controller)
        {
            var response = new ErrorResponse(message, errorCode, data);

            _result = new JSendResult<ErrorResponse>(HttpStatusCode.InternalServerError, response, controller);
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public ErrorResponse Response
        {
            get { return _result.Response; }
        }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        /// <summary>Gets the error message explaining what went wrong.</summary>
        public string Message
        {
            get { return _result.Response.Message; }
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
