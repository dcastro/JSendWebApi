using System.Web.Http;
using JSend.WebApi;

namespace JSend.Client.FunctionalTests
{
    [RoutePrefix("users")]
    public class UsersController : JSendApiController
    {
        [Route("success"), HttpGet]
        public void SuccessAction()
        {

        }
    }
}
