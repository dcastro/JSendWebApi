namespace JSend.Client
{
    /// <summary>A JSend response.</summary>
    public interface IJSendResponse
    {
        /// <summary>Gets the status of this response.</summary>
        JSendStatus Status { get; }
    }
}
