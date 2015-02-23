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
    public class JSendOkResult<T> : IHttpActionResult
    {
        private readonly JSendResult<SuccessResponse> _result;

        public JSendOkResult(JSendApiController controller, T content)
        {
            if (controller == null) throw new ArgumentNullException("controller");

            _result = InitializeResult(controller.JsonSerializerSettings, controller.Encoding, controller.Request,
                content);
        }

        public JSendOkResult(JsonSerializerSettings settings, Encoding encoding, HttpRequestMessage request, T content)
        {
            _result = InitializeResult(settings, encoding, request, content);
        }

        private static JSendResult<SuccessResponse> InitializeResult(JsonSerializerSettings settings,
            Encoding encoding, HttpRequestMessage request, T content)
        {
            return new JSendResult<SuccessResponse>(settings, encoding, request, new SuccessResponse(content),
                HttpStatusCode.OK);
        }

        public SuccessResponse Response
        {
            get { return _result.Response; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
