using LaserwarTest.Core.UI.Popups;
using LaserwarTest.Core.UI.Popups.Animations;
using LaserwarTest.UI.Dialogs;
using LaserwarTest.UI.Popups.Animations;

namespace LaserwarTest.UI.Popups
{
    /// <summary>
    /// Предоставляет возможность отображения диалога ошибки во всплывающем окне
    /// </summary>
    public sealed class ErrorPopupContent : PopupContent
    {
        ErrorDialog Dialog => Content as ErrorDialog;

        /// <summary>
        /// Получает или задает текст заголовка окна
        /// </summary>
        public string Title
        {
            set => Dialog.Title = value;
            get => Dialog.Title;
        }

        /// <summary>
        /// Получает или задает отображаемый текст ошибки
        /// </summary>
        public string Message
        {
            set => Dialog.Message = value;
            get => Dialog.Message;
        }

        public ErrorPopupContent(IPopupContentAnimation openAnimation = null, IPopupContentAnimation closeAnimation = null)
            : this("", openAnimation, closeAnimation)
        { }

        public ErrorPopupContent(string message, IPopupContentAnimation openAnimation = null, IPopupContentAnimation closeAnimation = null) 
            : this("Произошла ошибка", message, openAnimation, closeAnimation)
        {
        }

        public ErrorPopupContent(string title, string message, IPopupContentAnimation openAnimation = null, IPopupContentAnimation closeAnimation = null) : base(
            new ErrorDialog() { Title = title, Message = message, Width = 600, Height = 400 },
            openAnimation ?? new ScalePopupOpenAnimation(),
            closeAnimation ?? new ScalePopupCloseAnimation())
        {
        }
    }
}
