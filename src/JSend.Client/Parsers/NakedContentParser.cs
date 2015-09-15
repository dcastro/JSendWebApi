using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using JSend.Client.Properties;
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
        /// <param name="httpResponseMessage">The HTTP response message to parse.</param>
        /// <returns>A task representings the parsed <see cref="JSendResponse{T}"/>.</returns>
        /// <exception cref="JSendParseException">The HTTP response message could not be parsed.</exception>
        public async Task<JSendResponse<T>> ParseAsync<T>(JsonSerializerSettings serializerSettings,
            HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null)
                throw new ArgumentNullException(nameof(httpResponseMessage));

            if (httpResponseMessage.Content == null)
                throw new JSendParseException(StringResources.ResponseWithoutContent);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            try
            {
                var json = JsonConvert.DeserializeObject<JToken>(content, serializerSettings);

                if (json.IsValid(await GetFailSchemaAsync()))
                    return await DefaultJSendParser.ParseFailMessageAsync<T>(json, httpResponseMessage);

                if (json.IsValid(await GetErrorSchemaAsync()))
                    return await DefaultJSendParser.ParseErrorMessageAsync<T>(json, httpResponseMessage);

                if (json.IsValid(await GetSuccessSchemaAsync()))
                    return await
                        DefaultJSendParser.ParseSuccessMessageAsync<T>(json, serializerSettings, httpResponseMessage);

                var wrapped = new JObject
                {
                    {"status", "success"},
                    {"data", json}
                };

                return await
                    DefaultJSendParser.ParseSuccessMessageAsync<T>(wrapped, serializerSettings, httpResponseMessage);
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
