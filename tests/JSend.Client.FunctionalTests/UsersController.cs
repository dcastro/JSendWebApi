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

        [Route("non-jsend"), HttpGet]
        public IHttpActionResult NonJSendAction()
        {
            return Ok(TestUser);
        }

        [Route("non-json"), HttpGet]
        public IHttpActionResult NonJsonAction()
        {
            var response = new SuccessResponse(TestUser);
            var formatter = Configuration.Formatters.XmlFormatter;

            return Content(HttpStatusCode.OK, response, formatter);
        }

        [Route("get"), HttpGet]
        public string GetAction()
        {
            return "get";
        }

        [Route("post"), HttpPost]
        public string PostAction()
        {
            return "post";
        }

        [Route("put"), HttpPut]
        public string PutAction()
        {
            return "put";
        }

        [Route("delete"), HttpDelete]
        public string DeleteAction()
        {
            return "delete";
        }

        [Route("post-echo"), HttpPost]
        public User PostEchoAction(User user)
        {
            return user;
        }

        [Route("put-echo"), HttpPut]
        public User PutEchoAction(User user)
        {
            return user;
        }

        [Route("echo-headers"), HttpGet]
        public IDictionary<string, IEnumerable<string>> EchoHeadersAction()
        {
            return Request.Headers.ToDictionary(
                header => header.Key, header => header.Value);
        }
    }
}
