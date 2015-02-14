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
        private readonly JSendResult<SuccessJSendResponse<T>> _result;

        public JSendOkResult(JSendApiController controller, T content)
        {
            _result = new JSendResult<SuccessJSendResponse<T>>(controller, new SuccessJSendResponse<T>(content),
                HttpStatusCode.OK);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
