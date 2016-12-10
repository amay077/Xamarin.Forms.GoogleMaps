using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFGoogleMapSample
{
    public class NotifyClass : INotifyPropertyChanged
    {
        protected void NotifyPropertyChanged(string prop) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
