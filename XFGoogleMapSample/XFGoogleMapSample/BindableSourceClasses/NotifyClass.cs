using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XFGoogleMapSample
{
    public class NotifyClass : INotifyPropertyChanged
    {
        protected void NotifyPropertyChanging(string prop) { PropertyChanging?.Invoke(this, new PropertyChangedEventArgs(prop)); }
        protected void NotifyPropertyChanged(string prop) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); }
        protected void NotifyIAmChanging([CallerMemberName] string propertyName = null) => NotifyPropertyChanging(propertyName);
        protected void NotifyIChanged([CallerMemberName] string propertyName = null) => NotifyPropertyChanged(propertyName);
        public event PropertyChangedEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
