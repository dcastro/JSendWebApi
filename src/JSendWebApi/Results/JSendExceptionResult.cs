using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using JSendWebApi.Properties;
using JSendWebApi.Responses;
using Newtonsoft.Json;

namespace JSendWebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="ErrorResponse"/> containing the specified error details
    /// with status code <see cref="HttpStatusCode.InternalServerError"/>.
    /// </summary>
    public class JSendExceptionResult : IHttpActionResult
    {
        private readonly JSendResult<ErrorResponse> _result;
        private readonly Exception _exception;

        /// <summary>Initializes a new instance of <see cref="JSendExceptionResult"/>.</summary>
        /// <param name="exception">The exception to include in the the error.</param>
        /// <param name="message">
        /// An optional meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.
        /// If none is provided, and if the controller's <see cref="HttpRequestContext.IncludeErrorDetail"/> is set to <see langword="true"/>,
        /// the exception's message will be used instead.
        /// </param>
        /// <param name="errorCode">
        /// A numeric code corresponding to the error, if applicable.
        /// </param>
        /// <param name="data">
        /// An optional generic container for any other information about the error.
        /// If none is provided, and if the controller's <see cref="HttpRequestContext.IncludeErrorDetail"/> is set to <see langword="true"/>,
        /// the exception's details will be used instead.
        /// </param>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public JSendExceptionResult(Exception exception, string message, int? errorCode, object data,
            JSendApiController controller)
            : this(exception, message, errorCode, data, new ControllerDependencyProvider(controller))
        {

        }

        /// <summary>Initializes a new instance of <see cref="JSendExceptionResult"/>.</summary>
        /// <param name="exception">The exception to include in the the error.</param>
        /// <param name="message">
        /// An optional meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong.
        /// If none is provided, and if <paramref name="includeErrorDetail"/> is <see langword="true"/>,
        /// the exception's message will be used instead.
        /// </param>
        /// <param name="errorCode">
        /// A numeric code corresponding to the error, if applicable.
        /// </param>
        /// <param name="data">
        /// An optional generic container for any other information about the error.
        /// If none is provided, and if <paramref name="includeErrorDetail"/> is <see langword="true"/>,
        /// the exception's details will be used instead.
        /// </param>
        /// <param name="includeErrorDetail">
        /// <see langword="true"/> if the response should include exception messages/stack traces
        /// when no <paramref name="message"/> or <paramref name="data"/> are provided; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="settings">The serializer settings.</param>
        /// <param name="encoding">The content encoding.</param>
        /// <param name="request">The request message which led to this result.</param>
        public JSendExceptionResult(Exception exception, string message, int? errorCode, object data,
            bool includeErrorDetail, JsonSerializerSettings settings, Encoding encoding, HttpRequestMessage request)
            : this(exception, message, errorCode, data,
                new DirectDependencyProvider(includeErrorDetail, settings, encoding, request))
        {

        }

        private JSendExceptionResult(Exception exception, string message, int? errorCode, object data,
            IDependencyProvider dependencies)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            var response = BuildResponse(dependencies.IncludeErrorDetail, exception, message, errorCode, data);

            _exception = exception;
            _result = new JSendResult<ErrorResponse>(HttpStatusCode.InternalServerError, response,
                dependencies.JsonSerializerSettings, dependencies.Encoding, dependencies.RequestMessage);
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

        /// <summary>Gets the exception to include in the error.</summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        private static ErrorResponse BuildResponse(bool includeErrorDetail, Exception ex, string message,
            int? errorCode, object data)
        {
            return new ErrorResponse(
                message: BuildErrorMessage(includeErrorDetail, ex, message),
                code: errorCode,
                data: BuildData(includeErrorDetail, ex, data));
        }

        private static string BuildErrorMessage(bool includeErrorDetail, Exception ex, string message)
        {
            if (message != null)
                return message;
            if (includeErrorDetail)
                return ex.Message;

            return StringResources.DefaultErrorMessage;
        }

        private static object BuildData(bool includeErrorDetail, Exception ex, object data)
        {
            if (data != null)
                return data;
            if (includeErrorDetail)
                return ex.ToString();
            return null;
        }

        /// <summary>Creates an <see cref="HttpResponseMessage"/> asynchronously.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }

        private interface IDependencyProvider
        {
            bool IncludeErrorDetail { get; }
            JsonSerializerSettings JsonSerializerSettings { get; }
            Encoding Encoding { get; }
            HttpRequestMessage RequestMessage { get; }
        }

        private class DirectDependencyProvider : IDependencyProvider
        {
            private readonly bool _includeErrorDetail;
            private readonly JsonSerializerSettings _jsonSerializerSettings;
            private readonly Encoding _encoding;
            private readonly HttpRequestMessage _requestMessage;

            public DirectDependencyProvider(bool includeErrorDetail, JsonSerializerSettings jsonSerializerSettings,
                Encoding encoding, HttpRequestMessage requestMessage)
            {
                _includeErrorDetail = includeErrorDetail;
                _jsonSerializerSettings = jsonSerializerSettings;
                _encoding = encoding;
                _requestMessage = requestMessage;
            }

            public bool IncludeErrorDetail
            {
                get { return _includeErrorDetail; }
            }

            public JsonSerializerSettings JsonSerializerSettings
            {
                get { return _jsonSerializerSettings; }
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

            public bool IncludeErrorDetail
            {
                get { return _controller.RequestContext.IncludeErrorDetail; }
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
