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

namespace JSendWebApi.Results
{
    public abstract class BaseJSendResult<TResponse> : IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        private readonly JsonResult<TResponse> _jsonResult;

        protected BaseJSendResult(JSendApiController controller, TResponse response, HttpStatusCode statusCode)
        {
            if (controller == null) throw new ArgumentNullException("controller");
            if (response == null) throw new ArgumentNullException("response");

            _statusCode = statusCode;
            _jsonResult = new JsonResult<TResponse>(
                response,
                controller.JsonSerializerSettings,
                controller.Encoding,
                controller);
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _jsonResult.ExecuteAsync(cancellationToken);
            message.StatusCode = _statusCode;
            return message;
        }
    }
}
