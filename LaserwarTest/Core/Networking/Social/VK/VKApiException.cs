using System;

namespace LaserwarTest.Core.Networking.Social.VK
{
    /// <summary>
    /// Происходит при неудачной попытке использования API социальной сети ВКонтакте
    /// </summary>
    public class VKApiException : Exception
    {
        public VKApiException() { }
        public VKApiException(string message) : base(message) { }
        public VKApiException(string message, Exception innerException) : base(message, innerException) { }
    }
}
