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
using JSendWebApi.Responses;
using Newtonsoft.Json;

namespace JSendWebApi.Results
{
    public sealed class JSendResult<TResponse> : IHttpActionResult where TResponse : IJSendResponse
    {
        private readonly TResponse _response;
        private readonly HttpStatusCode _statusCode;
        private readonly JsonResult<TResponse> _jsonResult;

        public JSendResult(JSendApiController controller, TResponse response, HttpStatusCode statusCode)
            : this(new ControllerDependencyProvider(controller), response, statusCode)
        {

        }

        public JSendResult(JsonSerializerSettings settings, Encoding encoding, HttpRequestMessage request,
            TResponse response, HttpStatusCode code)
            : this(new DirectDependencyProvider(settings, encoding, request), response, code)
        {

        }

        private JSendResult(IDependencyProvider dependencies, TResponse response, HttpStatusCode statusCode)
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

        public TResponse Response
        {
            get { return _response; }
        }

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
