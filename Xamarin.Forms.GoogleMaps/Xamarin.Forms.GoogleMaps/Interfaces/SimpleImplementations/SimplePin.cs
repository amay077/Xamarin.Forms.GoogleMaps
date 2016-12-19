using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xamarin.Forms.GoogleMaps.Interfaces.SimpleImplementations
{
    public class SimplePin : BindableObject, IPin
    {
        private string _PinId = Guid.NewGuid().ToString();
        public string PinId { get { return _PinId; } set { bool changed = _PinId != value; if (changed) { OnPropertyChanging(); _PinId = value; OnPropertyChanged(); } } }

        private string _PinTitle;
        public string PinTitle { get { return _PinTitle; } set { bool changed = _PinTitle != value; if (changed) { OnPropertyChanging(); _PinTitle = value; OnPropertyChanged(); } } }

        private string _PinSubTitle;
        public string PinSubTitle { get { return _PinSubTitle; } set { bool changed = _PinSubTitle != value; if (changed) { OnPropertyChanging(); _PinSubTitle = value; OnPropertyChanged(); } } }

        private Position _PinPosition;
        public Position PinPosition { get { return _PinPosition; } set { bool changed = _PinPosition != value; if (changed) { OnPropertyChanging(); _PinPosition = value; OnPropertyChanged(); } } }

        private float _PinRotation;
        public float PinRotation { get { return _PinRotation; } set { bool changed = _PinRotation != value; if (changed) { OnPropertyChanging(); _PinRotation = value; OnPropertyChanged(); } } }

        private bool _PinIsDraggable;
        public bool PinIsDraggable { get { return _PinIsDraggable; } set { bool changed = _PinIsDraggable != value; if (changed) { OnPropertyChanging(); _PinIsDraggable = value; OnPropertyChanged(); } } }

        private BitmapDescriptor _PinIcon;
        public BitmapDescriptor PinIcon { get { return _PinIcon; } set { bool changed = _PinIcon != value; if (changed) { OnPropertyChanging(); _PinIcon = value; OnPropertyChanged(); } } }

        private IPinConfig _PinConfig = new SimplePinConfig();
        public IPinConfig PinConfig { get { return _PinConfig; } set { bool changed = _PinConfig != value; if (changed) { OnPropertyChanging(); _PinConfig = value; OnPropertyChanged(); } } }

        public int CompareTo(IPin other)
            => ((IPin)this).CompareTo(other);
    }

}
