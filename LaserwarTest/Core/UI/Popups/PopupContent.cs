using LaserwarTest.Core.UI.Popups.Animations;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace LaserwarTest.Core.UI.Popups
{
    /// <summary>
    /// Обеспечивает возможность отображения содержимого во всплывающем окне
    /// </summary>
    public class PopupContent
    {
        protected Size CurrentWindowSize => new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height);

        /// <summary>
        /// Происходит каждый раз, когда какое-либо всплывающее окно запустило процесс открытия
        /// </summary>
        public static event EventHandler PopupOpening;
        /// <summary>
        /// Происходит каждый раз, когда какое-либо всплывающее окно открылось
        /// </summary>
        public static event EventHandler PopupOpened;
        /// <summary>
        /// Происходит каждый раз, когда какое-либо всплывающее окно запустило процесс закрытия
        /// </summary>
        public static event EventHandler PopupClosing;
        /// <summary>
        /// Происходит каждый раз, когда какое-либо всплывающее окно закрылось
        /// </summary>
        public static event EventHandler PopupClosed;

        /// <summary>
        /// Происходит, когда окно запустило процесс открытия
        /// </summary>
        public event EventHandler Opening;
        /// <summary>
        /// Происходит, когда окно открылось
        /// </summary>
        public event EventHandler Opened;
        /// <summary>
        /// Происходит, когда окно запустило процесс закрытия
        /// </summary>
        public event EventHandler Closing;
        /// <summary>
        /// Происходит, когда окно закрылось
        /// </summary>
        public event EventHandler Closed;

        PopupContentState _state;

        /// <summary>
        /// Получает внутренне содержимое всплывающего окна
        /// </summary>
        protected UIElement Content { get; }
        Size ContentSize { set; get; }

        /// <summary>
        /// Анимация появления всплывающего окна
        /// </summary>
        IPopupContentAnimation OpenAnimation { get; }
        /// <summary>
        /// Анимация закрытия всплывающего окна
        /// </summary>
        IPopupContentAnimation CloseAnimation { get; }

        /// <summary>
        /// Получает или задает выплывающее окно
        /// </summary>
        Popup Popup { set; get; }
        /// <summary>
        /// Получает или задает, может ли вслывающий элемент быть закрытым
        /// </summary>
        bool IsLightDismissEnabled { set;  get; }

        /// <summary>
        /// Получает или задает состояние всплывающего элемента
        /// </summary>
        PopupContentState State
        {
            set
            {
                if (_state == value) return;

                _state = value;
                switch (_state)
                {
                    case PopupContentState.Opening:
                        PopupOpening?.Invoke(this, EventArgs.Empty);
                        Opening?.Invoke(this, EventArgs.Empty);
                        break;

                    case PopupContentState.Opened:
                        PopupOpened?.Invoke(this, EventArgs.Empty);
                        Opened?.Invoke(this, EventArgs.Empty);
                        break;

                    case PopupContentState.Closing:
                        PopupClosing?.Invoke(this, EventArgs.Empty);
                        Closing?.Invoke(this, EventArgs.Empty);
                        break;

                    case PopupContentState.Closed:
                        PopupClosed?.Invoke(this, EventArgs.Empty);
                        Closed?.Invoke(this, EventArgs.Empty);
                        break;
                }
            }
            get { return _state; }
        }

        public PopupContent(UIElement content, IPopupContentAnimation openAnimation = null, IPopupContentAnimation closeAnimation = null)
        {
            Content = content;

            OpenAnimation = openAnimation;
            if (openAnimation != null)
            {
                openAnimation.SetTarget(content);
                openAnimation.Completed += (s, e) => OpeningCompleted();
            }

            CloseAnimation = closeAnimation;
            if (closeAnimation != null)
            {
                closeAnimation.SetTarget(content);
                closeAnimation.Completed += (s, e) => ClosingCompleted();
            }
        }

        private void OnElementCanClose(object sender, EventArgs e)
        {
            Close();
        }

        private void OnPopupLoaded(object sender, RoutedEventArgs e)
        {
            ContentSize = Content.RenderSize;

            OnSizeChanged(CurrentWindowSize);
        }

        public void Open(bool isModal = false)
        {
            if (State != PopupContentState.Closed) return;

            IsLightDismissEnabled = !isModal;
            Popup = new Popup { Child = Content };

            Popup.Loaded += OnPopupLoaded;
            Popup.Opened += OnPopupOpened;
            Popup.Closed += OnPopupClosed;

            if (Content is IPopupUIElement popupUIElement)
            {
                popupUIElement.CanClose += OnElementCanClose;
            }

            Popup.IsOpen = true;
        }

        private void OnPopupOpened(object sender, object e)
        {
            /// Поддержка LightDismiss
            Popup.Opened -= OnPopupOpened;

            State = PopupContentState.Opening;
            if (OpenAnimation == null)
            {
                OpeningCompleted();
                return;
            }

            Popup.IsLightDismissEnabled = false;
            OpenAnimation.Start();
        }

        private void OpeningCompleted()
        {
            State = PopupContentState.Opened;

            Popup.IsLightDismissEnabled = IsLightDismissEnabled;
            OnSizeChanged(CurrentWindowSize);

            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        public void Close()
        {
            if (State != PopupContentState.Opened) return;

            Window.Current.SizeChanged -= OnWindowSizeChanged;

            State = PopupContentState.Closing;
            if (CloseAnimation == null)
            {
                ClosingCompleted();
                return;
            }

            Popup.IsLightDismissEnabled = false;
            CloseAnimation.Start();
        }

        private void ClosingCompleted()
        {
            State = PopupContentState.Closed;

            Popup.IsOpen = false;
        }

        private void OnPopupClosed(object sender, object e)
        {
            /// Поддержка LightDissmiss
            if (State == PopupContentState.Opened)
            {
                Popup.IsOpen = true;
                OnSizeChanged(CurrentWindowSize);
                Close();
                return;
            }

            Popup.Loaded -= OnPopupLoaded;
            Popup.Closed -= OnPopupClosed;

            if (Content is IPopupUIElement popupUIElement)
            {
                popupUIElement.CanClose += OnElementCanClose;
            }

            Popup = null;
        }

        private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (State != PopupContentState.Opened) return;

            OnSizeChanged(e.Size);
        }

        protected void OnSizeChanged(Size size)
        {
            Popup.VerticalOffset = (size.Height - ContentSize.Height) * 0.5;
            Popup.HorizontalOffset = (size.Width - ContentSize.Width) * 0.5;
        }
    }

    /// <summary>
    /// Определяет состояние всплывающего элемента
    /// </summary>
    public enum PopupContentState
    {
        Closed = 0,
        Opening,
        Opened,
        Closing
    }
}
