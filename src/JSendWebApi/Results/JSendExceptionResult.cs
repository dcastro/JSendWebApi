using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendExceptionResult : IHttpActionResult
    {
        private readonly JSendResult<ErrorJSendResponse> _result;

        public JSendExceptionResult(JSendApiController controller, Exception exception, string message, int? errorCode,
            object data)
        {
            if (controller == null) throw new ArgumentNullException("controller");
            if (exception == null) throw new ArgumentNullException("exception");

            var response = BuildResponse(controller.RequestContext.IncludeErrorDetail, exception, message, errorCode, data);

            _result = new JSendResult<ErrorJSendResponse>(
                controller, response, HttpStatusCode.InternalServerError);
        }

        public ErrorJSendResponse Response
        {
            get { return _result.Response; }
        }

        private static ErrorJSendResponse BuildResponse(bool includeErrorDetail, Exception ex, string message,
            int? errorCode, object data)
        {
            return new ErrorJSendResponse(
                message: BuildErrorMessage(includeErrorDetail, ex, message),
                code: errorCode,
                data: BuildData(includeErrorDetail, ex, data));
        }

        private static string BuildErrorMessage(bool includeErrorDetail, Exception ex, string message)
        {
            if (message != null)
                return message;
            if (includeErrorDetail)
                return ex.Message;

            return "An error has occurred.";
        }

        private static object BuildData(bool includeErrorDetail, Exception ex, object data)
        {
            if (data != null)
                return data;
            if (includeErrorDetail)
                return ex.ToString();
            return null;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
