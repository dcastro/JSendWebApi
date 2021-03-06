﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using JSend.WebApi.Extensions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;

namespace JSend.WebApi.Results
{
    /// <summary>
    /// Represents an action result that returns a <see cref="ErrorResponse"/> containing the specified error details
    /// with status code <see cref="HttpStatusCode.InternalServerError"/>.
    /// </summary>
    public class JSendExceptionResult : IJSendResult<ErrorResponse>
    {
        private readonly string _message;
        private readonly int? _errorCode;
        private readonly object _data;
        private readonly IDependencyProvider _dependencies;

        private ErrorResponse _response;

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
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            Exception = exception;
            _message = message;
            _errorCode = errorCode;
            _data = data;
            _dependencies = dependencies;
        }

        /// <summary>Gets the response to be formatted into the message's body.</summary>
        public ErrorResponse Response
        {
            get
            {
                if (_response == null)
                {
                    _response = BuildResponse(_dependencies.IncludeErrorDetail, Exception, _message, _errorCode, _data);
                }
                return _response;
            }
        }

        /// <summary>Gets the HTTP status code for the response message.</summary>
        public HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;

        /// <summary>Gets the request message which led to this result.</summary>
        public HttpRequestMessage Request => _dependencies.RequestMessage;

        /// <summary>Gets the exception to include in the error.</summary>
        public Exception Exception { get; }

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
            var result = new JSendResult<ErrorResponse>(StatusCode, Response, Request);

            return result.ExecuteAsync(cancellationToken);
        }

        private interface IDependencyProvider
        {
            bool IncludeErrorDetail { get; }

            HttpRequestMessage RequestMessage { get; }
        }

        private sealed class DirectDependencyProvider : IDependencyProvider
        {
            public DirectDependencyProvider(bool includeErrorDetail, HttpRequestMessage requestMessage)
            {
                if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

                IncludeErrorDetail = includeErrorDetail;
                RequestMessage = requestMessage;
            }

            public bool IncludeErrorDetail { get; }

            public HttpRequestMessage RequestMessage { get; }
        }

        private sealed class ControllerDependencyProvider : IDependencyProvider
        {
            private readonly ApiController _controller;
            private IDependencyProvider _resolvedDependencies;

            public ControllerDependencyProvider(ApiController controller)
            {
                if (controller == null)
                    throw new ArgumentNullException(nameof(controller));

                _controller = controller;
            }

            public bool IncludeErrorDetail
            {
                get
                {
                    EnsureResolved();
                    return _resolvedDependencies.IncludeErrorDetail;
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

            private void EnsureResolved()
            {
                if (_resolvedDependencies == null)
                {
                    var includeErrorDetail = _controller.Request.ShouldIncludeErrorDetail();
                    var request = _controller.GetRequestOrThrow();

                    _resolvedDependencies = new DirectDependencyProvider(includeErrorDetail, request);
                }
            }
        }
    }
}
