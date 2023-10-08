using Bardcoded.API.Data.Requests;
using System.Runtime.Serialization;

namespace Bardcoded
{
    [Serializable]
    internal class DataConflictException : InvalidOperationException
    {
        public BardcodeInjestRequest? Actual { get; private set; }

        public DataConflictException(string message, BardcodeInjestRequest? actual) : base(message) {
            this.Actual = actual;
        }
    }
}