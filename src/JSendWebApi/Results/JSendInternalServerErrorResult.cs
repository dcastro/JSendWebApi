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
    public class JSendInternalServerErrorResult : IHttpActionResult
    {
        private readonly JSendResult<ErrorResponse> _result;

        public JSendInternalServerErrorResult(JSendApiController controller, string message, int? errorCode, object data)
        {
            var response = new ErrorResponse(message, errorCode, data);

            _result = new JSendResult<ErrorResponse>(HttpStatusCode.InternalServerError, response, controller);
        }

        public ErrorResponse Response
        {
            get { return _result.Response; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        public string Message
        {
            get { return _result.Response.Message; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
