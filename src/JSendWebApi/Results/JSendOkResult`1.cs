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

        public JSendOkResult(T content, JSendApiController controller)
        {
            if (controller == null) throw new ArgumentNullException("controller");

            _result = InitializeResult(content, controller.JsonSerializerSettings, controller.Encoding, controller.Request);
        }

        public JSendOkResult(T content, JsonSerializerSettings settings, Encoding encoding, HttpRequestMessage request)
        {
            _result = InitializeResult(content, settings, encoding, request);
        }

        private static JSendResult<SuccessResponse> InitializeResult(T content, JsonSerializerSettings settings,
            Encoding encoding, HttpRequestMessage request)
        {
            var response = new SuccessResponse(content);

            return new JSendResult<SuccessResponse>(HttpStatusCode.OK, response, settings, encoding, request);
        }

        public SuccessResponse Response
        {
            get { return _result.Response; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _result.StatusCode; }
        }

        public T Content
        {
            get { return (T) _result.Response.Data; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
