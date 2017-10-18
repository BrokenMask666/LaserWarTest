namespace LaserwarTest.Core.Networking.Social.VK
{
    /// <summary>
    /// Представляет ошибку, возникающу в ходе взаимодействия с API социальной сети ВКонтакте
    /// </summary>
    public class VKError
    {
        /// <summary>
        /// Заголовок ошибки
        /// </summary>
        public string Error { get; }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Descriprion { get; }
        /// <summary>
        /// Указывает, необходимо ли провести перенавигацию на страницу
        /// </summary>
        public bool Renavigate { get; }

        public VKError(string error, string description) : this(error, description, true) {}
        public VKError(string error, string description, bool renavigate)
        {
            Error = error;
            Descriprion = description;
            Renavigate = renavigate;
        }
    }

    /// <summary>
    /// Представляет ошибку авторизации в социальной сети ВКонтакте.
    /// Данная ошибка указывает на необходимость перенавигации на страницу авторизации
    /// </summary>
    public class VKAuthorizationError : VKError
    {
        public VKAuthorizationError(string error, string description) : base(error, description) { }
    }
}
