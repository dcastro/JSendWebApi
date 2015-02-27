using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using JSend.WebApi.Responses;
using Newtonsoft.Json;

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
        private readonly JsonResult<TResponse> _jsonResult;

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="response">The JSend response to format in the entity body.</param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendResult(HttpStatusCode statusCode, TResponse response, JSendApiController controller)
            : this(statusCode, response, new ControllerDependencyProvider(controller))
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendOkResult"/>.</summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param>
        /// <param name="response">The JSend response to format in the entity body.</param>
        /// <param name="settings">The serializer settings.</param>
        /// <param name="encoding">The content encoding.</param>
        /// <param name="request">The request message which led to this result.</param>
        public JSendResult(HttpStatusCode statusCode, TResponse response, JsonSerializerSettings settings,
            Encoding encoding, HttpRequestMessage request)
            : this(statusCode, response, new DirectDependencyProvider(settings, encoding, request))
        {

        }

        private JSendResult(HttpStatusCode statusCode, TResponse response, IDependencyProvider dependencies)
        {
            if (response == null) throw new ArgumentNullException("response");

            _response = response;
            _statusCode = statusCode;
            _jsonResult = new JsonResult<TResponse>(
                response,
                dependencies.JsonSerializerSettings,
                dependencies.Encoding,
                dependencies.RequestMessage);
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
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _jsonResult.ExecuteAsync(cancellationToken);
            message.StatusCode = _statusCode;
            return message;
        }

        private interface IDependencyProvider
        {
            JsonSerializerSettings JsonSerializerSettings { get; }
            Encoding Encoding { get; }
            HttpRequestMessage RequestMessage { get; }
        }

        private class DirectDependencyProvider : IDependencyProvider
        {
            private readonly JsonSerializerSettings _settings;
            private readonly Encoding _encoding;
            private readonly HttpRequestMessage _requestMessage;

            public DirectDependencyProvider(JsonSerializerSettings settings, Encoding encoding,
                HttpRequestMessage requestMessage)
            {
                _settings = settings;
                _encoding = encoding;
                _requestMessage = requestMessage;
            }

            public JsonSerializerSettings JsonSerializerSettings
            {
                get { return _settings; }
            }

            public Encoding Encoding
            {
                get { return _encoding; }
            }

            public HttpRequestMessage RequestMessage
            {
                get { return _requestMessage; }
            }
        }

        private class ControllerDependencyProvider : IDependencyProvider
        {
            private readonly JSendApiController _controller;

            public ControllerDependencyProvider(JSendApiController controller)
            {
                if (controller == null) throw new ArgumentNullException("controller");

                _controller = controller;
            }

            public JsonSerializerSettings JsonSerializerSettings
            {
                get { return _controller.JsonSerializerSettings; }
            }

            public Encoding Encoding
            {
                get { return _controller.Encoding; }
            }

            public HttpRequestMessage RequestMessage
            {
                get { return _controller.Request; }
            }
        }
    }
}
