using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace LaserwarTest.UI.Controls
{
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public class IconButton : Control
    {
        protected bool TemplateApplied { set; get; } = false;

        Button _button;

        public event EventHandler<IconButtonCommand> CommandInvoked;

        public IconButton()
        {
            DefaultStyleKey = typeof(IconButton);
            IsEnabledChanged += OnIsEnabledChanged;
        }

        protected override void OnApplyTemplate()
        {
            _button = (Button)GetTemplateChild("PART_Button");
            _button.CommandParameter = this;

            base.OnApplyTemplate();

            ApplyIcon(Icon);
            ApplyCommand(Command ?? new IconButtonCommand());

            TemplateApplied = true;
        }

        #region DependencyProperty

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(ImageSource),
                typeof(IconButton),
                new PropertyMetadata(null, OnIconPropertyChanged));

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IconButton obj = d as IconButton;
            if (obj == null || !obj.TemplateApplied) return;

            obj.ApplyIcon(e.NewValue as ImageSource);
        }

        private void ApplyIcon(ImageSource imageSource)
        {
            _button.Content = imageSource;
        }

        public ImageSource Icon
        {
            set { SetValue(IconProperty, value); }
            get { return (ImageSource)GetValue(IconProperty); }
        }



        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(IconButtonCommand),
                typeof(IconButton),
                new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IconButton obj = d as IconButton;
            if (obj == null || !obj.TemplateApplied) return;

            if (e.OldValue is IconButtonCommand oldCommand)
                oldCommand.Invoked -= obj.OnCommandInvoked;

            obj.ApplyCommand(e.NewValue as IconButtonCommand);
        }

        private void ApplyCommand(IconButtonCommand command)
        {
            _button.Command = command;
            command.Invoked += OnCommandInvoked;
        }

        private void OnCommandInvoked(object sender, IconButtonCommand command) =>
            CommandInvoked?.Invoke(sender, command);

        public IconButtonCommand Command
        {
            set { SetValue(CommandProperty, value); }
            get { return (IconButtonCommand)GetValue(CommandProperty); }
        }

        #endregion DependencyProperty

        protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Command?.RaiseCanExecuteChanged();
        }
    }

    public sealed class IconButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public event EventHandler<IconButtonCommand> Invoked;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            Invoked?.Invoke((parameter as IconButton), this);
        }
    }
}
