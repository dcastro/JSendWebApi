using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http.Formatting;
using System.Web.Http;
using JSend.WebApi.Properties;

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
                        CultureInfo.CurrentCulture,
                        StringResources.ConfigurationMustContainFormatter,
                        typeof (JsonMediaTypeFormatter).FullName),
                    "configuration");

            return formatters.JsonFormatter;
        }
    }
}
