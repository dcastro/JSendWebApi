using System.Web.Http;

namespace JSend.WebApi.FunctionalTests
{
    [RoutePrefix("users")]
    public class UsersController : JSendApiController
    {
        public static readonly User TestUser = new User {Username = "DCastro"};

        [Route("ok"), HttpGet]
        public IHttpActionResult OkAction()
        {
            return JSendOk();
        }

        [Route("ok-with-user"), HttpGet]
        public IHttpActionResult OkWithDataAction()
        {
            return JSendOk(TestUser);
        }
    }
}
