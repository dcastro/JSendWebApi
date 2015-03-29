using System;
using System.Collections.Generic;
using System.Web.Http;

namespace JSend.WebApi.FunctionalTests
{
    [RoutePrefix("users")]
    public class UsersController : JSendApiController
    {
        public static readonly User TestUser = new User {Username = "DCastro"};
        public static readonly string CreatedLocation = "http://localhost/users/dummy-location/5";

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

        [Route("dummy-location/{id:int}", Name = "DummyLocation"), HttpGet]
        public void DummyLocation(int id)
        {
        }
    }
}
