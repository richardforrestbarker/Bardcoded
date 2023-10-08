using System.Runtime.Serialization;

namespace Bardcoded
{
    [Serializable]
    internal class OfflineException : ApplicationException
    {
        public OfflineException()
        {
        }

        public OfflineException(string? message) : base(message)
        {
        }

        public OfflineException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected OfflineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}