using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http;

namespace JSend.WebApi.FunctionalTests
{
    [RoutePrefix("users")]
    public class UsersController : JSendApiController
    {
        public static readonly User TestUser = new User {Username = "DCastro"};
        public static readonly string CreatedLocation = "http://localhost/users/dummy-location/5";

        public static readonly string ErrorMessage = "dummy error message";
        public static readonly int ErrorCode = 80;
        public static readonly object ErrorData = DateTime.UtcNow;

        public static readonly Exception Exception = new InvalidOperationException("dummy exception message");

        public static readonly string ModelErrorKey = "Username";
        public static readonly string ModelErrorValue = "Invalid Username";

        public static readonly string AuthenticationHeader = "dummy-authentication-header";

        /// <summary>
        /// Dummy action to redirect to.
        /// </summary>
        [Route("dummy-location/{id:int}", Name = "DummyLocation"), HttpGet]
        public void DummyLocation(int id)
        {
        }

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

        [Route("created-with-string"), HttpGet]
        public IHttpActionResult CreatedWithStringAction()
        {
            return JSendCreated(CreatedLocation, TestUser);
        }

        [Route("created-with-uri"), HttpGet]
        public IHttpActionResult CreatedWithUriAction()
        {
            return JSendCreated(new Uri(CreatedLocation), TestUser);
        }

        [Route("created-at-route-with-object"), HttpGet]
        public IHttpActionResult CreatedAtRouteWithObjectAction()
        {
            return JSendCreatedAtRoute("DummyLocation", new {id = 5}, TestUser);
        }

        [Route("created-at-route-with-dictionary"), HttpGet]
        public IHttpActionResult CreatedAtRouteWithDictionaryAction()
        {
            var routeValues = new Dictionary<string, object>
            {
                {"id", 5}
            };
            return JSendCreatedAtRoute("DummyLocation", routeValues, TestUser);
        }

        [Route("redirect-with-string"), HttpGet]
        public IHttpActionResult RedirectWithStringAction()
        {
            return JSendRedirect(CreatedLocation);
        }

        [Route("redirect-with-uri"), HttpGet]
        public IHttpActionResult RedirectWithUriAction()
        {
            return JSendRedirect(new Uri(CreatedLocation));
        }

        [Route("redirect-to-route-with-object"), HttpGet]
        public IHttpActionResult RedirectToRouteWithObjectAction()
        {
            return JSendRedirectToRoute("DummyLocation", new {id = 5});
        }

        [Route("redirect-to-route-with-dictionary"), HttpGet]
        public IHttpActionResult RedirectToRouteWithDictionaryAction()
        {
            var routeValues = new Dictionary<string, object>
            {
                {"id", 5}
            };
            return JSendRedirectToRoute("DummyLocation", routeValues);
        }

        [Route("badrequest-with-reason"), HttpGet]
        public IHttpActionResult BadRequestWithReasonAction()
        {
            return JSendBadRequest(ErrorMessage);
        }

        [Route("badrequest-with-modelstate"), HttpGet]
        public IHttpActionResult BadRequestWithModelStateAction()
        {
            ModelState.AddModelError(ModelErrorKey, ModelErrorValue);
            return JSendBadRequest(ModelState);
        }

        [Route("unauthorized"), HttpGet]
        public IHttpActionResult UnauthorizedAction()
        {
            return JSendUnauthorized(new AuthenticationHeaderValue(AuthenticationHeader));
        }

        [Route("notfound"), HttpGet]
        public IHttpActionResult NotFoundAction()
        {
            return JSendNotFound();
        }

        [Route("notfound-with-reason"), HttpGet]
        public IHttpActionResult NotFoundWithReasonAction()
        {
            return JSendNotFound(ErrorMessage);
        }

        [Route("internal-server-error"), HttpGet]
        public IHttpActionResult InternalServerErrorAction()
        {
            return JSendInternalServerError(ErrorMessage, ErrorCode, ErrorData);
        }

        [Route("internal-server-error-with-exception"), HttpGet]
        public IHttpActionResult InternalServerErrorWithExceptionAction()
        {
            return JSendInternalServerError(Exception);
        }
    }
}
