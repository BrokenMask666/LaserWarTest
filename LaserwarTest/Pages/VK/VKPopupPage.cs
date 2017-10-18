using LaserwarTest.Core.Networking.Social.VK;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace LaserwarTest.Pages.VK
{
    /// <summary>
    /// Базовая страница для размещения внутри всплывающих диалоговых окон,
    /// представляющаяя некоторый функционал, связанный с социальной сетью ВКонтакте
    /// </summary>
    public abstract class VKPopupPage : Page
    {
        /// <summary>
        /// Используется для сообщения диалоговому окну о необходимости
        /// выполнения на странице длительной операции загрузки
        /// </summary>
        public event EventHandler LoadingSent;
        /// <summary>
        /// Используется для сообщения диалоговому окну о прекращении
        /// выполнения на странице длительной операции загрузки
        /// </summary>
        public event EventHandler LoadedSent;

        /// <summary>
        /// Используется для сообщения диалоговому окну о возникшей ошибке
        /// </summary>
        public event EventHandler<VKError> ErrorOccured;

        /// <summary>
        /// Получает заголовок страницы
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Уведомляет слушателей о необходимости длительной операции
        /// </summary>
        protected async Task SendLoading(int millisecondsDelay = 60)
        {
            LoadingSent?.Invoke(this, EventArgs.Empty);
            if (millisecondsDelay > 0) await Task.Delay(millisecondsDelay);
        }
        /// <summary>
        /// Уведомляет слушателей о завершении длительной операции
        /// </summary>
        protected void SendLoaded() => LoadedSent?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Уведомляет слушателей об ошибке на странице
        /// </summary>
        /// <param name="error">Содержит информацию об ошибке</param>
        public void SendError(VKError error) => ErrorOccured?.Invoke(this, error);
    }
}
