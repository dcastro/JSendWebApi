using System.Net;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendBadRequestResult<T> : BaseJSendResult<FailJSendResponse<T>>
    {
        public JSendBadRequestResult(JSendApiController controller, T content)
            : base(controller, new FailJSendResponse<T>(content), HttpStatusCode.BadRequest)
        {

        }
    }
}
