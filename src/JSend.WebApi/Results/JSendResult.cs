using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns the specified JSend response with the specified status code.
    /// </summary>
    /// <typeparam name="TResponse">The type of the JSend response.</typeparam>
    public sealed class JSendResult<TResponse> : IHttpActionResult where TResponse : IJSendResponse
    {
        private readonly TResponse _response;
        private readonly HttpStatusCode _statusCode;
        private readonly FormattedContentResult<TResponse> _formattedContentResult;

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="response">The JSend response to format in the entity body.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendResult(HttpStatusCode statusCode, TResponse response, ApiController controller)
            : this(statusCode, response, new ControllerDependencyProvider(controller))
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="response">The JSend response to format in the entity body.</param>
        /// <param name="formatter">The formatter to use to format the content.</param>
        /// <param name="request">The request message which led to this result.</param>
        public JSendResult(HttpStatusCode statusCode, TResponse response, JsonMediaTypeFormatter formatter,
            HttpRequestMessage request)
            : this(statusCode, response, new DirectDependencyProvider(formatter, request))
        {

        }

        private JSendResult(HttpStatusCode statusCode, TResponse response, IDependencyProvider dependencies)
        {
            if (response == null) throw new ArgumentNullException("response");

            _response = response;
            _statusCode = statusCode;

            var mediaTypeHeader = new MediaTypeHeaderValue("application/json");

            _formattedContentResult = new FormattedContentResult<TResponse>(statusCode, response,
                dependencies.Formatter, mediaTypeHeader, dependencies.RequestMessage);
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public TResponse Response
        {
            get { return _response; }
        }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _formattedContentResult.ExecuteAsync(cancellationToken);
        }

        internal interface IDependencyProvider
        {
            JsonMediaTypeFormatter Formatter { get; }

            HttpRequestMessage RequestMessage { get; }
        }

        internal sealed class DirectDependencyProvider : IDependencyProvider
        {
            private readonly JsonMediaTypeFormatter _formatter;
            private readonly HttpRequestMessage _requestMessage;

            public DirectDependencyProvider(JsonMediaTypeFormatter formatter, HttpRequestMessage requestMessage)
            {
                _formatter = formatter;
                _requestMessage = requestMessage;
            }

            public JsonMediaTypeFormatter Formatter
            {
                get { return _formatter; }
            }

            public HttpRequestMessage RequestMessage
            {
                get { return _requestMessage; }
            }
        }

        internal sealed class ControllerDependencyProvider : IDependencyProvider
        {
            private readonly IDependencyProvider _dependencies;

            public ControllerDependencyProvider(ApiController controller)
            {
                if (controller == null) throw new ArgumentNullException("controller");

                var formatter = controller.Configuration.GetJsonMediaTypeFormatter();
                var request = controller.Request;

                _dependencies = new DirectDependencyProvider(formatter, request);
            }

            public JsonMediaTypeFormatter Formatter
            {
                get { return _dependencies.Formatter; }
            }

            public HttpRequestMessage RequestMessage
            {
                get { return _dependencies.RequestMessage; }
            }
        }
    }
}
