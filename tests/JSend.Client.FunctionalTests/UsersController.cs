using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JSend.WebApi;
using JSend.WebApi.Responses;

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
        public User SuccessWithUserAction() => TestUser;

        [Route("fail"), HttpGet]
        public IHttpActionResult FailAction() => JSendFail(HttpStatusCode.BadRequest, ErrorData);

        [Route("error"), HttpGet]
        public IHttpActionResult ErrorAction()
            => JSendError(HttpStatusCode.InternalServerError, ErrorMessage, ErrorCode, ErrorData);

        [Route("no-content"), HttpGet]
        public IHttpActionResult NoContentAction() => StatusCode(HttpStatusCode.NoContent);

        [Route("non-jsend"), HttpGet]
        public IHttpActionResult NonJSendAction() => Ok(TestUser);

        [Route("non-json"), HttpGet]
        public IHttpActionResult NonJsonAction()
        {
            var response = new SuccessResponse(TestUser);
            var formatter = Configuration.Formatters.XmlFormatter;

            return Content(HttpStatusCode.OK, response, formatter);
        }

        [Route("empty-body"), HttpGet]
        public IHttpActionResult EmptyBody() => Ok();

        [Route("get"), HttpGet]
        public string GetAction() => "get";

        [Route("post"), HttpPost]
        public string PostAction() => "post";

        [Route("put"), HttpPut]
        public string PutAction() => "put";

        [Route("delete"), HttpDelete]
        public string DeleteAction() => "delete";

        [Route("post-echo"), HttpPost]
        public User PostEchoAction(User user) => user;

        [Route("put-echo"), HttpPut]
        public User PutEchoAction(User user) => user;

        [Route("echo-headers"), HttpGet]
        public IDictionary<string, IEnumerable<string>> EchoHeadersAction()
        {
            return Request.Headers.ToDictionary(
                header => header.Key, header => header.Value);
        }
    }
}
