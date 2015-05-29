using System;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using JSend.WebApi.Properties;

namespace JSend.WebApi.Extensions
{
    internal static class ApiControllerExtensions
    {
        internal static HttpRequestMessage GetRequestOrThrow(this ApiController controller)
        {
            var request = controller.Request;

            if (request == null)
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        StringResources.TypePropertyMustNotBeNull,
                        typeof (ApiController).Name,
                        "Request"));

            return request;
        }
    }
}
