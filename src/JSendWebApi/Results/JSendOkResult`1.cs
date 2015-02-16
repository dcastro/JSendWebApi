using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendOkResult<T> : IHttpActionResult
    {
        private readonly JSendResult<SuccessJSendResponse> _result;

        public JSendOkResult(JSendApiController controller, T content)
        {
            _result = new JSendResult<SuccessJSendResponse>(controller, new SuccessJSendResponse(content),
                HttpStatusCode.OK);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
