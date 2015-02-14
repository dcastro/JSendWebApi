using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendOkResult : IHttpActionResult
    {
        private readonly JSendResult<SuccessJSendResponse> _result;

        public JSendOkResult(JSendApiController controller)
        {
            _result = new JSendResult<SuccessJSendResponse>(controller, new SuccessJSendResponse(), HttpStatusCode.OK);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
