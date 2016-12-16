using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    // See ICircle implementation in the other partial class
    public partial class CirclePinModel : ICircle
    {
        private void NotifyICircleProperties(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(Center):
                    NotifyPropertyChanged(nameof(CircleCenter));
                    if (Center != null) Center.PropertyChanged += Center_PropertiesChanged;
                    break;
                case nameof(RadiusKm):
                    NotifyPropertyChanged(nameof(CircleRadius));
                    break;
            }
        }

        private void NotifyICirclePropertiesChanging(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Center):
                    if(Center!=null) Center.PropertyChanged -= Center_PropertiesChanged;
                    break;
            }
        }

        private void Center_PropertiesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPin.PinPosition))
                NotifyPropertyChanged(nameof(CircleCenter));
        }

        #region ICircle computed properties

        public Position CircleCenter{get { return Center.PinPosition; }}

        public Distance CircleRadius { get { return Distance.FromKilometers(RadiusKm); } }

        #endregion ICircle computed properties

        #region ICircle NOT computed properties

        public Color CircleFillColor{ get; set; }

        public Color CircleStrokeColor { get; set; }

        public float CircleStrokeWidth { get; set; }

        #endregion ICircle NOT computed properties
    }
}
