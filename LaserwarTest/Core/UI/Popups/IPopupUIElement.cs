using System;

namespace LaserwarTest.Core.UI.Popups
{
    /// <summary>
    /// Представляет UI-элемент, поддерживающий возможности отображения во всплывающих окнах
    /// </summary>
    public interface IPopupUIElement
    {
        /// <summary>
        /// Происходит, когда элемент готов к закрытию
        /// </summary>
        event EventHandler CanClose;
    }
}
