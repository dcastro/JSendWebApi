using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendInvalidModelStateResult : IHttpActionResult
    {
        private readonly JSendResult<FailJSendResponse> _result;

        public JSendInvalidModelStateResult(JSendApiController controller, ModelStateDictionary modelState)
        {
            if (controller == null) throw new ArgumentNullException("controller");

            HttpError validationErrors =
                new HttpError(modelState, controller.RequestContext.IncludeErrorDetail).ModelState;

            _result = new JSendResult<FailJSendResponse>(
                controller,
                new FailJSendResponse(validationErrors),
                HttpStatusCode.BadRequest);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _result.ExecuteAsync(cancellationToken);
        }
    }
}
