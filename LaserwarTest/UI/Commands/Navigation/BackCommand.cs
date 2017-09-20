using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LaserwarTest.UI.Commands.Navigation
{
    public sealed class BackCommand : NavigationCommandBase
    {
        public BackCommand(Frame navigationFrame) : base(navigationFrame) { }

        public override bool CanExecute(object parameter) => NavigationFrame.CanGoBack;

        public override void Execute(object parameter) => NavigationFrame.GoBack();
    }
}
