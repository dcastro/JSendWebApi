using System.Net;
using JSendWebApi.Responses;

namespace JSendWebApi.Results
{
    public class JSendOkResult : BaseJSendResult<SuccessJSendResponse>
    {
        public JSendOkResult(JSendApiController controller)
            : base(controller, new SuccessJSendResponse(), HttpStatusCode.OK)
        {

        }
    }
}
