using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevTools.Core
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void ChangeProperty<T>(T value, ref T target, [CallerMemberName] string caller = "")
        {
            if (!value.Equals(target))
            {
                target = value;
                OnPropertyChanged(caller);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
