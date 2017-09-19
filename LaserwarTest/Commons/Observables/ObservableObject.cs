using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LaserwarTest.Commons.Observables
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T valueHolder, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(valueHolder, value))
                return false;

            valueHolder = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }
    }
}
