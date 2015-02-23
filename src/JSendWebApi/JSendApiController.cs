using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;
using JSendWebApi.Results;
using Newtonsoft.Json;

namespace JSendWebApi
{
    public abstract class JSendApiController : ApiController
    {
        private JsonSerializerSettings _settings;
        private Encoding _encoding;

        protected JSendApiController()
            : this(new JsonSerializerSettings())
        {

        }

        protected JSendApiController(JsonSerializerSettings jsonSerializerSettings)
            : this(
                jsonSerializerSettings,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true))
        {
        }

        protected JSendApiController(JsonSerializerSettings jsonSerializerSettings, Encoding encoding)
        {
            if (jsonSerializerSettings == null) throw new ArgumentNullException("jsonSerializerSettings");
            if (encoding == null) throw new ArgumentNullException("encoding");

            _settings = jsonSerializerSettings;
            _encoding = encoding;
        }

        public JsonSerializerSettings JsonSerializerSettings
        {
            get { return _settings; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _settings = value;
            }
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _encoding = value;
            }
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            this.Configuration.Services.Replace(typeof (IExceptionHandler),
                new JSendExceptionHandler(_settings, _encoding));

            this.Configuration.Filters.Add(
                new ValueActionFilter(_settings, _encoding));

            this.Configuration.Filters.Add(
                new VoidActionFilter(_settings, _encoding));
        }

        protected internal virtual JSendOkResult JSendOk()
        {
            return new JSendOkResult(this);
        }

        protected internal virtual JSendOkResult<T> JSendOk<T>(T content)
        {
            return new JSendOkResult<T>(this, content);
        }

        protected internal virtual JSendBadRequestResult JSendBadRequest(string reason)
        {
            return new JSendBadRequestResult(this, reason);
        }

        protected internal virtual JSendInvalidModelStateResult JSendBadRequest(ModelStateDictionary modelState)
        {
            return new JSendInvalidModelStateResult(this, modelState);
        }

        protected internal virtual JSendCreatedResult<T> JSendCreated<T>(Uri location, T content)
        {
            return new JSendCreatedResult<T>(this, location, content);
        }

        protected internal virtual JSendCreatedResult<T> JSendCreated<T>(string location, T content)
        {
            if (location == null) throw new ArgumentNullException("location");

            return JSendCreated(new Uri(location, UriKind.RelativeOrAbsolute), content);
        }

        protected internal virtual JSendCreatedAtRouteResult<T> JSendCreatedAtRoute<T>(string routeName,
            IDictionary<string, object> routeValues, T content)
        {
            return new JSendCreatedAtRouteResult<T>(this, routeName, routeValues, content);
        }

        protected internal virtual JSendCreatedAtRouteResult<T> JSendCreatedAtRoute<T>(string routeName,
            object routeValues, T content)
        {
            return JSendCreatedAtRoute(routeName, new HttpRouteValueDictionary(routeValues), content);
        }

        protected internal virtual JSendInternalServerErrorResult JSendInternalServerError(string message,
            int? errorCode = null, object data = null)
        {
            return new JSendInternalServerErrorResult(this, message, errorCode, data);
        }

        protected internal virtual JSendExceptionResult JSendInternalServerError(Exception ex, string message = null,
            int? errorCode = null, object data = null)
        {
            return new JSendExceptionResult(this, ex, message, errorCode, data);
        }

        protected internal virtual JSendNotFoundResult JSendNotFound(string reason)
        {
            return new JSendNotFoundResult(this, reason);
        }

        protected internal virtual JSendNotFoundResult JSendNotFound()
        {
            return JSendNotFound(null);
        }

        protected internal virtual JSendRedirectResult JSendRedirect(Uri location)
        {
            return new JSendRedirectResult(this, location);
        }

        protected internal virtual JSendRedirectResult JSendRedirect(string location)
        {
            return JSendRedirect(new Uri(location));
        }
    }
}
