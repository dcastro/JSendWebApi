using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;
using Newtonsoft.Json;

namespace JSendWebApi.Results
{
    public class JSendExceptionResult : IHttpActionResult
    {
        private readonly JSendResult<ErrorResponse> _result;
        private readonly Exception _exception;

        public JSendExceptionResult(JSendApiController controller, Exception exception, string message, int? errorCode,
            object data)
            : this(new ControllerDependencyProvider(controller), exception, message, errorCode, data)
        {

        }

        public JSendExceptionResult(bool includeErrorDetail, JsonSerializerSettings settings, Encoding encoding,
            HttpRequestMessage request, Exception exception, string message, int? errorCode, object data)
            : this(new DirectDependencyProvider(includeErrorDetail, settings, encoding, request),
                exception, message, errorCode, data)
        {

        }

        private JSendExceptionResult(IDependencyProvider dependencies, Exception exception, string message,
            int? errorCode, object data)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            var response = BuildResponse(dependencies.IncludeErrorDetail, exception, message, errorCode, data);

            _exception = exception;
            _result = new JSendResult<ErrorResponse>(
                dependencies.JsonSerializerSettings, dependencies.Encoding, dependencies.RequestMessage, response,
                HttpStatusCode.InternalServerError);
        }

        public ErrorResponse Response
        {
            get { return _result.Response; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

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

            return "An error has occurred.";
        }

        private static object BuildData(bool includeErrorDetail, Exception ex, object data)
        {
            if (data != null)
                return data;
            if (includeErrorDetail)
                return ex.ToString();
            return null;
        }

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
