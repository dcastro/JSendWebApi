using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.FunctionalTests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.FunctionalTests
{
    public class HttpActionResultTests
    {
        [Theory]
        [InlineJSendAutoData("ok", HttpStatusCode.OK)]
        [InlineJSendAutoData("ok-with-user", HttpStatusCode.OK)]
        [InlineJSendAutoData("created-with-string", HttpStatusCode.Created)]
        [InlineJSendAutoData("created-with-uri", HttpStatusCode.Created)]
        [InlineJSendAutoData("created-at-route-with-object", HttpStatusCode.Created)]
        [InlineJSendAutoData("created-at-route-with-dictionary", HttpStatusCode.Created)]
        [InlineJSendAutoData("redirect-with-string", HttpStatusCode.Redirect)]
        [InlineJSendAutoData("redirect-with-uri", HttpStatusCode.Redirect)]
        [InlineJSendAutoData("redirect-to-route-with-object", HttpStatusCode.Redirect)]
        [InlineJSendAutoData("redirect-to-route-with-dictionary", HttpStatusCode.Redirect)]
        [InlineJSendAutoData("badrequest-with-reason", HttpStatusCode.BadRequest)]
        [InlineJSendAutoData("badrequest-with-modelstate", HttpStatusCode.BadRequest)]
        [InlineJSendAutoData("unauthorized", HttpStatusCode.Unauthorized)]
        [InlineJSendAutoData("notfound", HttpStatusCode.NotFound)]
        [InlineJSendAutoData("notfound-with-reason", HttpStatusCode.NotFound)]
        [InlineJSendAutoData("internal-server-error", HttpStatusCode.InternalServerError)]
        [InlineJSendAutoData("internal-server-error-with-exception", HttpStatusCode.InternalServerError)]
        [InlineJSendAutoData("jsend", HttpStatusCode.Gone)]
        [InlineJSendAutoData("jsend-success", HttpStatusCode.Gone)]
        [InlineJSendAutoData("jsend-fail", HttpStatusCode.Gone)]
        [InlineJSendAutoData("jsend-error", HttpStatusCode.Gone)]
        public async Task ActionsReturnExpectedStatusCode(
            string route, HttpStatusCode expectedCode,
            HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/" + route);

                // Verify outcome
                response.StatusCode.Should().Be(expectedCode);
            }
        }

        [Theory]
        [InlineJSendAutoData("ok")]
        [InlineJSendAutoData("ok-with-user")]
        [InlineJSendAutoData("created-with-string")]
        [InlineJSendAutoData("created-with-uri")]
        [InlineJSendAutoData("created-at-route-with-object")]
        [InlineJSendAutoData("created-at-route-with-dictionary")]
        [InlineJSendAutoData("redirect-with-string")]
        [InlineJSendAutoData("redirect-with-uri")]
        [InlineJSendAutoData("redirect-to-route-with-object")]
        [InlineJSendAutoData("redirect-to-route-with-dictionary")]
        [InlineJSendAutoData("badrequest-with-reason")]
        [InlineJSendAutoData("badrequest-with-modelstate")]
        [InlineJSendAutoData("unauthorized")]
        [InlineJSendAutoData("notfound")]
        [InlineJSendAutoData("notfound-with-reason")]
        [InlineJSendAutoData("internal-server-error")]
        [InlineJSendAutoData("internal-server-error-with-exception")]
        [InlineJSendAutoData("jsend")]
        [InlineJSendAutoData("jsend-success")]
        [InlineJSendAutoData("jsend-fail")]
        [InlineJSendAutoData("jsend-error")]
        public async Task ActionsSetContentTypeHeader(string route, HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/" + route);

                // Verify outcome
                response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
                response.Content.Headers.ContentType.CharSet.Should().Be(Encoding.UTF8.WebName);
            }
        }

        [Theory]
        [InlineJSendAutoData("created-with-string")]
        [InlineJSendAutoData("created-with-uri")]
        [InlineJSendAutoData("created-at-route-with-object")]
        [InlineJSendAutoData("created-at-route-with-dictionary")]
        [InlineJSendAutoData("redirect-with-string")]
        [InlineJSendAutoData("redirect-with-uri")]
        [InlineJSendAutoData("redirect-to-route-with-object")]
        [InlineJSendAutoData("redirect-to-route-with-dictionary")]
        public async Task ActionsSetLocationHeader(string route, HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/" + route);

                // Verify outcome
                response.Headers.Location.Should().Be(UsersController.TestLocation);
            }
        }

        [Theory, JSendAutoData]
        public async Task UnauthorizedAction_SetsAuthenticationHeader(HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/unauthorized");

                // Verify outcome
                response.Headers.WwwAuthenticate
                    .Should().ContainSingle(UsersController.AuthenticationHeader);
            }
        }

        public static IEnumerable<object[]> RoutesAndExpectedContent
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        "ok", new JObject
                        {
                            {"status", "success"},
                            {"data", null}
                        }
                    },
                    new object[]
                    {
                        "ok-with-user", new JObject
                        {
                            {"status", "success"},
                            {"data", JObject.FromObject(UsersController.TestUser)}
                        }
                    },
                    new object[]
                    {
                        "created-with-string", new JObject
                        {
                            {"status", "success"},
                            {"data", JObject.FromObject(UsersController.TestUser)}
                        }
                    },
                    new object[]
                    {
                        "created-with-uri", new JObject
                        {
                            {"status", "success"},
                            {"data", JObject.FromObject(UsersController.TestUser)}
                        }
                    },
                    new object[]
                    {
                        "created-at-route-with-object", new JObject
                        {
                            {"status", "success"},
                            {"data", JObject.FromObject(UsersController.TestUser)}
                        }
                    },
                    new object[]
                    {
                        "created-at-route-with-dictionary", new JObject
                        {
                            {"status", "success"},
                            {"data", JObject.FromObject(UsersController.TestUser)}
                        }
                    },
                    new object[]
                    {
                        "redirect-with-string", new JObject
                        {
                            {"status", "success"},
                            {"data", null}
                        }
                    },
                    new object[]
                    {
                        "redirect-with-uri", new JObject
                        {
                            {"status", "success"},
                            {"data", null}
                        }
                    },
                    new object[]
                    {
                        "redirect-to-route-with-object", new JObject
                        {
                            {"status", "success"},
                            {"data", null}
                        }
                    },
                    new object[]
                    {
                        "redirect-to-route-with-dictionary", new JObject
                        {
                            {"status", "success"},
                            {"data", null}
                        }
                    },
                    new object[]
                    {
                        "badrequest-with-reason", new JObject
                        {
                            {"status", "fail"},
                            {"data", UsersController.ErrorMessage}
                        }
                    },
                    new object[]
                    {
                        "badrequest-with-modelstate", new JObject
                        {
                            {"status", "fail"},
                            {
                                "data", new JObject
                                {
                                    {UsersController.ModelErrorKey, new JArray(UsersController.ModelErrorValue)}
                                }
                            }
                        }
                    },
                    new object[]
                    {
                        "unauthorized", new JObject
                        {
                            {"status", "fail"},
                            {"data", "Authorization has been denied for this request."}
                        }
                    },
                    new object[]
                    {
                        "notfound", new JObject
                        {
                            {"status", "fail"},
                            {"data", "The requested resource could not be found."}
                        }
                    },
                    new object[]
                    {
                        "notfound-with-reason", new JObject
                        {
                            {"status", "fail"},
                            {"data", UsersController.ErrorMessage}
                        }
                    },
                    new object[]
                    {
                        "internal-server-error", new JObject
                        {
                            {"status", "error"},
                            {"message", UsersController.ErrorMessage},
                            {"code", UsersController.ErrorCode},
                            {"data", JToken.FromObject(UsersController.ErrorData)}
                        }
                    },
                    new object[]
                    {
                        "internal-server-error-with-exception", new JObject
                        {
                            {"status", "error"},
                            {"message", UsersController.Exception.Message},
                            {"data", UsersController.Exception.ToString()}
                        }
                    },
                    new object[]
                    {
                        "jsend", new JObject
                        {
                            {"status", "success"},
                            {"data", null}
                        }
                    },
                    new object[]
                    {
                        "jsend-success", new JObject
                        {
                            {"status", "success"},
                            {"data", JToken.FromObject(UsersController.TestUser)}
                        }
                    },
                    new object[]
                    {
                        "jsend-fail", new JObject
                        {
                            {"status", "fail"},
                            {"data", UsersController.ErrorMessage}
                        }
                    },
                    new object[]
                    {
                        "jsend-error", new JObject
                        {
                            {"status", "error"},
                            {"message", UsersController.ErrorMessage},
                            {"code", UsersController.ErrorCode},
                            {"data", JToken.FromObject(UsersController.ErrorData)}
                        }
                    }
                };
            }
        }

        [Theory]
        [PropertyData("RoutesAndExpectedContent")]
        public async Task ActionsReturnExpectedContent(string route, JObject expectedContent)
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new TestConventions());

            using (var server = fixture.Create<HttpServer>())
            using (var client = new HttpClient(server))
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/" + route);

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                Assert.Equal(expectedContent, content, JToken.EqualityComparer);
            }
        }
    }
}
