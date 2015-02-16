using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendBadRequestResult<T> : IHttpActionResult
    {
        private readonly JSendResult<FailJSendResponse> _result;

        public JSendBadRequestResult(JSendApiController controller, T content)
        {
            _result = new JSendResult<FailJSendResponse>(controller, new FailJSendResponse(content),
                HttpStatusCode.BadRequest);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
