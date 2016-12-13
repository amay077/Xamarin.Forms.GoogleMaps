using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    // This is the presentation logic of the model
    public partial class PinModel : IPin
    {
        private void NotifyIPinProperties(string propertyName)
        { // Notify a change if the property depending on is changed
            switch(propertyName)
            {
                case nameof(Name):
                    NotifyPropertyChanged(nameof(PinTitle));
                    break;
                case nameof(Details):
                    NotifyPropertyChanged(nameof(PinSubTitle));
                    break;
                case nameof(Latitude):
                case nameof(Longitude):
                    NotifyPropertyChanged(nameof(PinPosition));
                    break;
            }
        }

        #region IPin computed properties

        public string PinTitle { get { return Name; } }

        public Position PinPosition { get { return new Position(Latitude, Longitude); } }

        public string PinSubTitle { get { return Details; }  }

        #endregion IPin computed properties

        #region IPin NOT computed properties

        private BitmapDescriptor _PinIcon;
        public BitmapDescriptor PinIcon { get { return _PinIcon; } set { bool changed = _PinIcon != value; _PinIcon = value; if (changed) NotifyPropertyChanged(nameof(PinIcon)); } }

        private bool _PinIsDraggable;
        public bool PinIsDraggable { get { return _PinIsDraggable; } set { bool changed = _PinIsDraggable != value; _PinIsDraggable = value; if (changed) NotifyPropertyChanged(nameof(PinIsDraggable)); } }

        private float _PinRotation;
        public float PinRotation { get { return _PinRotation; } set { bool changed = _PinRotation != value; _PinRotation = value; if (changed) NotifyPropertyChanged(nameof(PinRotation)); } }

        private PinType _PinType;
        public PinType PinType { get { return _PinType; } set { bool changed = _PinType != value; _PinType = value; if (changed) NotifyPropertyChanged(nameof(PinType)); } }

        public ICommand SelectedCommand { get; set; }

        public object SelectedCommandParameter { get; set; }

        public ICommand InfoWindowClickedCommand { get; set; }

        public object InfoWindowClickedCommandParameter { get; set; }

        public ICommand PinClickedCommand { get; set; }

        public object PinClickedCommandParameter { get; set; }

        #endregion IPin NOT computed properties
    }
}
