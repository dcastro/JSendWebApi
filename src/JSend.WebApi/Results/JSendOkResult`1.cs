using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="SuccessResponse"/> with the specified content and
    /// with status code <see cref="HttpStatusCode.OK"/>.
    /// </summary>
    /// <typeparam name="T">The type of the content in the entity body.</typeparam>
    public class JSendOkResult<T> : IJSendResult<SuccessResponse>
    {
        private readonly JSendResult<SuccessResponse>.IDependencyProvider _dependencies;

        /// <summary>Initializes a new instance of <see cref="JSendOkResult{T}"/>.</summary>
        /// <param name="content">The content value to format in the entity body.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendOkResult(T content, ApiController controller)
            : this(content, new JSendResult<SuccessResponse>.ControllerDependencyProvider(controller))
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendOkResult{T}"/>.</summary>
        /// <param name="content">The content value to format in the entity body.</param>
        /// <param name="request">The request message which led to this result.</param>
        public JSendOkResult(T content, HttpRequestMessage request)
            : this(content, new JSendResult<SuccessResponse>.RequestDependencyProvider(request))
        {

        }

        private JSendOkResult(T content, JSendResult<SuccessResponse>.IDependencyProvider dependencies)
        {
            Response = new SuccessResponse(content);
            _dependencies = dependencies;
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public SuccessResponse Response { get; }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode => HttpStatusCode.OK;

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request => _dependencies.RequestMessage;

        /// <summary>Gets the content value to format in the entity body.</summary>
        public T Content => (T) Response.Data;

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = new JSendResult<SuccessResponse>(StatusCode, Response, _dependencies.RequestMessage);

            return result.ExecuteAsync(cancellationToken);
        }
    }
}
