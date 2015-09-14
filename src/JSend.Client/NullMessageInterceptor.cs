namespace JSend.Client
{
    /// <summary>Message interceptor that does nothing.</summary>
    public sealed class NullMessageInterceptor : MessageInterceptor
    {
        /// <summary>Gets an instance of <see cref="NullMessageInterceptor"/>.</summary>
        public static NullMessageInterceptor Instance { get; } = new NullMessageInterceptor();
    }
}
