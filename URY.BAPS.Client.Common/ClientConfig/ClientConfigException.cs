using System;
using System.Runtime.Serialization;

namespace URY.BAPS.Client.Common.ClientConfig
{
    /// <summary>
    ///     Exception thrown when loading client configuration fails.
    /// </summary>
    [Serializable]
    public class ClientConfigException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ClientConfigException()
        {
        }

        public ClientConfigException(string message) : base(message)
        {
        }

        public ClientConfigException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ClientConfigException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
