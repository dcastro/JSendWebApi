using System;
using Newtonsoft.Json;

namespace JSend.WebApi.Responses
{
    /// <summary>A JSend fail response.</summary>
    public class FailResponse : IJSendResponse
    {
        /// <summary>Initializes a new instance of <see cref="FailResponse"/>.</summary>
        /// <param name="data">A wrapper for the details of why the request failed.</param>
        public FailResponse(object data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Data = data;
        }

        /// <summary>Gets the status of this response, always set to "fail".</summary>
        [JsonProperty("status", Order = 1)]
        public string Status => "fail";

        /// <summary>Gets the wrapper for the details of why the request failed.</summary>
        [JsonProperty("data", Order = 2)]
        public object Data { get; }
    }
}
