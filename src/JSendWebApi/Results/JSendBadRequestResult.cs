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

namespace JSendWebApi.Results
{
    public class JSendBadRequestResult<T> : IHttpActionResult
    {
        private readonly JsonResult<FailJSendResponse<T>> _result;

        public JSendBadRequestResult(JSendApiController controller, T content)
        {
            _result = new JsonResult<FailJSendResponse<T>>(
                new FailJSendResponse<T>(content),
                controller.JsonSerializerSettings,
                controller.Encoding,
                controller);
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = await _result.ExecuteAsync(cancellationToken);
            message.StatusCode = HttpStatusCode.BadRequest;
            return message;
        }
    }
}
