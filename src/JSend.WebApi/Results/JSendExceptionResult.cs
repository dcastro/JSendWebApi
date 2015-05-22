using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
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
            ApiController controller)
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
        /// <param name="request">The request message which led to this result.</param>
        public JSendExceptionResult(Exception exception, string message, int? errorCode, object data,
            bool includeErrorDetail, HttpRequestMessage request)
            : this(exception, message, errorCode, data, new DirectDependencyProvider(includeErrorDetail, request))
        {

        }

        private JSendExceptionResult(Exception exception, string message, int? errorCode, object data,
            IDependencyProvider dependencies)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            var response = BuildResponse(dependencies.IncludeErrorDetail, exception, message, errorCode, data);

            _exception = exception;
            _result = new JSendResult<ErrorResponse>(HttpStatusCode.InternalServerError, response,
                dependencies.RequestMessage);
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

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request
        {
            get { return _result.Request; }
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

            HttpRequestMessage RequestMessage { get; }
        }

        private sealed class DirectDependencyProvider : IDependencyProvider
        {
            private readonly bool _includeErrorDetail;
            private readonly HttpRequestMessage _requestMessage;

            public DirectDependencyProvider(bool includeErrorDetail, HttpRequestMessage requestMessage)
            {
                _includeErrorDetail = includeErrorDetail;
                _requestMessage = requestMessage;
            }

            public bool IncludeErrorDetail
            {
                get { return _includeErrorDetail; }
            }

            public HttpRequestMessage RequestMessage
            {
                get { return _requestMessage; }
            }
        }

        private sealed class ControllerDependencyProvider : IDependencyProvider
        {
            private readonly IDependencyProvider _dependencies;

            public ControllerDependencyProvider(ApiController controller)
            {
                if (controller == null)
                    throw new ArgumentNullException("controller");

                var includeErrorDetail = controller.Request.ShouldIncludeErrorDetail();
                var request = controller.Request;

                _dependencies = new DirectDependencyProvider(includeErrorDetail, request);
            }

            public bool IncludeErrorDetail
            {
                get { return _dependencies.IncludeErrorDetail; }
            }

            public HttpRequestMessage RequestMessage
            {
                get { return _dependencies.RequestMessage; }
            }
        }
    }
}
