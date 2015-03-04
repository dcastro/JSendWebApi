using Newtonsoft.Json;

namespace JSend.WebApi.Responses
{
    /// <summary>A JSend success response.</summary>
    public class SuccessResponse : IJSendResponse
    {
        private readonly object _data;

        /// <summary>Initializes a new instance of <see cref="SuccessResponse"/>.</summary>
        public SuccessResponse()
            : this(null)
        {
        }

        /// <summary>Initializes a new instance of <see cref="SuccessResponse"/>.</summary>
        /// <param name="data">A wrapper for any data to be returned.</param>
        public SuccessResponse(object data)
        {
            _data = data;
        }

        /// <summary>Gets the status of this response, always set to "success".</summary>
        [JsonProperty("status", Order = 1)]
        public string Status
        {
            get { return "success"; }
        }

        /// <summary>Gets the wrapper for any data to be returned; null if no data is to be returned.</summary>
        [JsonProperty("data", Order = 2, NullValueHandling = NullValueHandling.Include)]
        public object Data
        {
            get { return _data; }
        }
    }
}
