using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.UI.Commands.Navigation
{
    public abstract class NavigationCommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        protected Frame NavigationFrame { get; }

        public NavigationCommandBase(Frame navigationFrame)
        {
            NavigationFrame = navigationFrame;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}
