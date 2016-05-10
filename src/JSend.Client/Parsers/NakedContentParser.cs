using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JSend.Client.Extensions;
using JSend.Client.Properties;
using JSend.Client.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using static JSend.Client.JSendSchemas;

namespace JSend.Client.Parsers
{
    /// <summary>
    /// This parser assumes the request was successful when the data returned is not wrapped in a valid JSend "envelope".
    /// E.g., the following document: 
    /// <code>
    /// {
    ///   "id": 5468,
    /// }
    /// </code>
    /// 
    /// Will be parsed as if it was:
    /// 
    /// <code>
    /// {
    ///   "status": "success",
    ///   "data": {
    ///     "id": 5468
    ///   }
    /// }
    /// </code>
    /// </summary>
    public class NakedContentParser : IJSendParser
    {
        /// <summary>
        /// Parses the content of a <see cref="HttpResponseMessage"/> and returns a <see cref="JSendResponse{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expected data.</typeparam>
        /// <param name="serializerSettings">The settings used to deserialize the response.</param>
        /// <param name="httpResponse">The HTTP response message to parse.</param>
        /// <returns>A task representings the parsed <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JSendParseException">The HTTP response message could not be parsed.</exception>
        [Pure]
        public async Task<JSendResponse<T>> ParseAsync<T>(JsonSerializerSettings serializerSettings,
            HttpResponseMessage httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                return new JSuccessResponse<T>(httpResponse);

            if (httpResponse.Content == null)
                throw new JSendParseException(StringResources.ResponseWithEmptyBody);

            var content = await httpResponse.Content.ReadAsStringAsync().IgnoreContext();

            if (string.IsNullOrWhiteSpace(content))
                throw new JSendParseException(StringResources.ResponseWithEmptyBody);

            try
            {
                var json = JsonConvert.DeserializeObject<JToken>(content, serializerSettings);

                if (json.IsValid(await GetFailSchemaAsync().IgnoreContext()))
                    return await DefaultJSendParser.ParseFailMessageAsync<T>(json, httpResponse).IgnoreContext();

                if (json.IsValid(await GetErrorSchemaAsync().IgnoreContext()))
                    return await DefaultJSendParser.ParseErrorMessageAsync<T>(json, httpResponse).IgnoreContext();

                if (json.IsValid(await GetSuccessSchemaAsync().IgnoreContext()))
                    return await
                        DefaultJSendParser.ParseSuccessMessageAsync<T>(json, serializerSettings, httpResponse).IgnoreContext();

                var wrapped = new JObject
                {
                    {"status", "success"},
                    {"data", json}
                };

                return await
                    DefaultJSendParser.ParseSuccessMessageAsync<T>(wrapped, serializerSettings, httpResponse).IgnoreContext();
            }
            catch (JsonException ex)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    StringResources.ResponseParseError,
                    typeof (JSendResponse<T>), Environment.NewLine, content);

                throw new JSendParseException(message, ex);
            }
        }
    }
}
