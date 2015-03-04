using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace JSend.WebApi
{
    internal static class HttpConfigurationExtensions
    {
        [Pure]
        internal static JsonMediaTypeFormatter GetJsonMediaTypeFormatter(this HttpConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            var formatters = configuration.Formatters;
            Contract.Assert(formatters != null);

            if (formatters.JsonFormatter == null)
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The controller's configuration does not contain any formatter of type {0}.",
                        typeof (JsonMediaTypeFormatter).FullName));

            return formatters.JsonFormatter;
        }
    }
}
