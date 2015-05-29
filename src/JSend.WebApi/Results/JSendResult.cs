using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using JSend.WebApi.Extensions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns the specified JSend response with the specified status code.
    /// </summary>
    /// <typeparam name="TResponse">The type of the JSend response.</typeparam>
    public sealed class JSendResult<TResponse> : IJSendResult<TResponse> where TResponse : IJSendResponse
    {
        private readonly HttpStatusCode _statusCode;
        private readonly TResponse _response;
        private readonly IDependencyProvider _dependencies;

        private static readonly MediaTypeHeaderValue MediaTypeHeader = new MediaTypeHeaderValue("application/json");

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
        /// <param name="request">The request message which led to this result.</param>
        public JSendResult(HttpStatusCode statusCode, TResponse response, HttpRequestMessage request)
            : this(statusCode, response, new RequestDependencyProvider(request))
        {

        }

        private JSendResult(HttpStatusCode statusCode, TResponse response, IDependencyProvider dependencies)
        {
            if (response == null) throw new ArgumentNullException("response");

            _statusCode = statusCode;
            _response = response;
            _dependencies = dependencies;
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

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request
        {
            get { return _dependencies.RequestMessage; }
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var mediaTypeHeader = new MediaTypeHeaderValue(MediaTypeHeader.MediaType);

            var result = new FormattedContentResult<TResponse>(_statusCode, _response,
                _dependencies.Formatter, mediaTypeHeader, _dependencies.RequestMessage);

            return result.ExecuteAsync(cancellationToken);
        }

        internal interface IDependencyProvider
        {
            JsonMediaTypeFormatter Formatter { get; }

            HttpRequestMessage RequestMessage { get; }
        }

        internal sealed class RequestDependencyProvider : IDependencyProvider
        {
            private readonly HttpRequestMessage _requestMessage;

            private JsonMediaTypeFormatter _formatter;

            public RequestDependencyProvider(HttpRequestMessage requestMessage)
            {
                if (requestMessage == null) throw new ArgumentNullException("requestMessage");

                _requestMessage = requestMessage;
            }

            public JsonMediaTypeFormatter Formatter
            {
                get
                {
                    EnsureResolved();
                    return _formatter;
                }
            }

            public HttpRequestMessage RequestMessage
            {
                get
                {
                    EnsureResolved();
                    return _requestMessage;
                }
            }

            private void EnsureResolved()
            {
                if (_formatter == null)
                {
                    var requestContext = _requestMessage.GetRequestContext();
                    if (requestContext == null)
                        throw new InvalidOperationException(StringResources.Request_RequestContextMustNotBeNull);

                    var configuration = requestContext.Configuration;
                    if (configuration == null)
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                StringResources.TypePropertyMustNotBeNull,
                                typeof (HttpRequestContext).Name,
                                "Configuration"));

                    var formatters = configuration.Formatters;
                    Contract.Assert(formatters != null);

                    var formatter = formatters.FindWriter(typeof (TResponse), MediaTypeHeader);

                    if (formatter == null)
                        throw new InvalidOperationException(StringResources.ConfigurationMustContainFormatter);

                    _formatter = formatters.JsonFormatter;
                }
            }
        }

        internal sealed class ControllerDependencyProvider : IDependencyProvider
        {
            private readonly ApiController _controller;

            private IDependencyProvider _resolvedDependencies;

            public ControllerDependencyProvider(ApiController controller)
            {
                if (controller == null) throw new ArgumentNullException("controller");

                _controller = controller;
            }

            public JsonMediaTypeFormatter Formatter
            {
                get
                {
                    EnsureResolved();
                    return _resolvedDependencies.Formatter;
                }
            }

            public HttpRequestMessage RequestMessage
            {
                get
                {
                    EnsureResolved();
                    return _resolvedDependencies.RequestMessage;
                }
            }

            public void EnsureResolved()
            {
                if (_resolvedDependencies == null)
                {
                    var request = _controller.GetRequestOrThrow();

                    _resolvedDependencies = new RequestDependencyProvider(request);
                }
            }
        }
    }
}
