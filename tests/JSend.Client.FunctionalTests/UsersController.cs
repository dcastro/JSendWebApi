using System.Net;
using System.Net.Http;
using System.Web.Http;
using JSend.WebApi;

namespace JSend.Client.FunctionalTests
{
    [RoutePrefix("users")]
    public class UsersController : JSendApiController
    {
        public static readonly User TestUser = new User {Username = "DCastro"};
        public static readonly string ErrorMessage = "dummy error message";
        public static readonly int ErrorCode = 80;
        public static readonly string ErrorData = "dummy error data";

        [Route("success"), HttpGet]
        public void SuccessAction()
        {

        }

        [Route("success-with-user"), HttpGet]
        public User SuccessWithUserAction()
        {
            return TestUser;
        }

        [Route("fail"), HttpGet]
        public IHttpActionResult FailAction()
        {
            return JSendFail(HttpStatusCode.BadRequest, ErrorData);
        }

        [Route("error"), HttpGet]
        public IHttpActionResult ErrorAction()
        {
            return JSendError(HttpStatusCode.InternalServerError, ErrorMessage, ErrorCode, ErrorData);
        }

        [Route("no-content"), HttpGet]
        public HttpResponseMessage NoContentAction()
        {
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}
