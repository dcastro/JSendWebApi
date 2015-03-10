using Newtonsoft.Json;

namespace JSend.Client
{
    /// <summary>A JSend success response.</summary>
    /// <typeparam name="T">The type of the data contained wrapped by this response.</typeparam>
    public class SuccessResponse<T> : IJSendResponse
    {
        private readonly T _data;

        /// <summary>Initializes a new instance of <see cref="SuccessResponse{T}"/>.</summary>
        /// <param name="data">The data returned by the API.</param>
        public SuccessResponse(T data)
        {
            _data = data;
        }

        /// <summary>Gets the status of this response, always set to <see cref="JSendStatus.Success"/>.</summary>
        [JsonProperty("status")]
        public JSendStatus Status
        {
            get { return JSendStatus.Success; }
        }

        /// <summary>Gets the wrapper for any data returned by the API; null if no data was returned.</summary>
        [JsonProperty("data")]
        public T Data
        {
            get { return _data; }
        }

        /// <summary>Returns a string that represents the current response.</summary>
        /// <returns>A string that represents the current response.</returns>
        public override string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
