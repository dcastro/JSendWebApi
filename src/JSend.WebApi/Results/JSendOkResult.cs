using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="SuccessResponse"/> with status code <see cref="HttpStatusCode.OK"/>.
    /// </summary>
    public class JSendOkResult : IJSendResult<SuccessResponse>
    {
        private readonly JSendResult<SuccessResponse> _result;

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendOkResult(ApiController controller)
            : this(new JSendResult<SuccessResponse>.ControllerDependencyProvider(controller))
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="request">The request message which led to this result.</param>
        public JSendOkResult(HttpRequestMessage request)
            : this(new JSendResult<SuccessResponse>.RequestDependencyProvider(request))
        {

        }

        private JSendOkResult(JSendResult<SuccessResponse>.IDependencyProvider dependencies)
        {
            var response = new SuccessResponse();

            _result = new JSendResult<SuccessResponse>(HttpStatusCode.OK, response, dependencies.RequestMessage);
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

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request
        {
            get { return _result.Request; }
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
