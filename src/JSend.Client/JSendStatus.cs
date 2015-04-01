namespace JSend.Client
{
    /// <summary>Defines the possible statuses for a <see cref="JSendResponse"/>.</summary>
    public enum JSendStatus
    {
        /// <summary>Indicates that all went well, and (usually) some data was returned.</summary>
        Success,
        /// <summary>
        /// Indicates that there was a problem with the data submitted,
        /// or some pre-condition of the API call wasn't satisfied.
        /// </summary>
        Fail,
        /// <summary>An error occurred in processing the request, i.e. an exception was thrown.</summary>
        Error
    }
}
