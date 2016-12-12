using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    // This should be the main logic of the model
    // See IPin implementation in the other partial class
    public partial class PinModel : NotifyClass
    {
        public PinModel(string name, string details, double latitude, double longitude) : this()
        {
            Name = name;
            Details = details;
            Latitude = latitude;
            Longitude = longitude;
        }
        public PinModel()
        {
            this.PropertyChanged += PinModel_PropertyChanged;
        }

        private void PinModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyIPinProperties(e.PropertyName);
        }

        #region Properties
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { bool changed = _Name != value; if (changed) { _Name = value; NotifyPropertyChanged(nameof(Name)); } }
        }

        private string _Details;
        public string Details
        {
            get { return _Details; }
            set { bool changed = _Details != value; if (changed) { _Details = value; NotifyPropertyChanged(nameof(Details)); } }
        }


        private double _Latitude;
        public double Latitude
        {
            get { return _Latitude; }
            set { bool changed = _Latitude != value; _Latitude = value; if (changed) NotifyPropertyChanged(nameof(Latitude)); }
        }

        private double _Longitude;
        public double Longitude
        {
            get { return _Longitude; }
            set { bool changed = _Longitude != value; _Longitude = value; if (changed) NotifyPropertyChanged(nameof(Longitude)); }
        }

        public string Category { get; set; }
        #endregion Properties

        public void Move100m(MoveDirection direction)
        {
            Latitude = Latitude + (direction == MoveDirection.North ? 1 : -1) * 0.0091;
        }
    }
    public enum MoveDirection { North, South }
}
