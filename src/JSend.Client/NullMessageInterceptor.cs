namespace JSend.Client
{
    /// <summary>Message interceptor that does nothing.</summary>
    public sealed class NullMessageInterceptor : MessageInterceptor
    {
        private static readonly NullMessageInterceptor _instance = new NullMessageInterceptor();

        /// <summary>Gets an instance of <see cref="NullMessageInterceptor"/>.</summary>
        public static NullMessageInterceptor Instance
        {
            get { return _instance; }
        }
    }
}
