using System;
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
    public class JSendOkResult : IHttpActionResult
    {
        private readonly JSendResult<SuccessJSendResponse> _result;

        public JSendOkResult(JSendApiController controller)
        {
            if (controller == null) throw new ArgumentNullException("controller");

            _result = InitializeResult(controller.JsonSerializerSettings, controller.Encoding, controller.Request);
        }

        public JSendOkResult(JsonSerializerSettings settings, Encoding encoding, HttpRequestMessage request)
        {
            _result = InitializeResult(settings, encoding, request);
        }

        private JSendResult<SuccessJSendResponse> InitializeResult(JsonSerializerSettings settings, Encoding encoding,
            HttpRequestMessage request)
        {
            return new JSendResult<SuccessJSendResponse>(settings, encoding, request, new SuccessJSendResponse(),
                HttpStatusCode.OK);
        }

        public SuccessJSendResponse Response
        {
            get { return _result.Response; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
