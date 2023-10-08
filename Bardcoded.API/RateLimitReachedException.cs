using System.Runtime.Serialization;

namespace Bardcoded.API
{
    [Serializable]
    internal class RateLimitReachedException : Exception
    {
        public RateLimitReachedException()
        {
        }

        public RateLimitReachedException(string? message) : base(message)
        {
        }

        public RateLimitReachedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RateLimitReachedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}