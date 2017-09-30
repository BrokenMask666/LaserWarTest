using System;
using Windows.UI.Xaml;

namespace LaserwarTest.Core.UI.Popups.Animations
{
    /// <summary>
    /// Представляет анимацию, выполняющуюся над содержимым всплывающего окна
    /// </summary>
    public interface IPopupContentAnimation
    {
        /// <summary>
        /// Происходит по завершении анимации
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// Устанавливает элемент, являющийся целью анимации
        /// </summary>
        /// <param name="animatingElement">Анимируемый элемент</param>
        void SetTarget(UIElement animatingElement);

        /// <summary>
        /// Запускает анимацию
        /// </summary>
        void Start();
        /// <summary>
        /// Вызывает остановку текущей анимации
        /// </summary>
        void Stop();
    }
}
