using System.Net;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendOkResult<T> : BaseJSendResult<SuccessJSendResponse<T>>
    {
        public JSendOkResult(JSendApiController controller, T content)
            : base(controller, new SuccessJSendResponse<T>(content), HttpStatusCode.OK)
        {

        }
    }
}
