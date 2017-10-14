using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LaserwarTest.Commons.Observables
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T valueHolder, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(valueHolder, value))
                return false;

            valueHolder = value;
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
