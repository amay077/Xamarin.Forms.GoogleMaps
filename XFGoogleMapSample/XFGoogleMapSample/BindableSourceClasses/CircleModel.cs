using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFGoogleMapSample
{
    public partial class CircleModel : NotifyClass
    {
        public CircleModel(PinModel center, double radiusKm)
        {
            PropertyChanged += CircleModel_PropertyChanged;
            PropertyChanging += CircleModel_PropertyChanging;
            Center = center;
            RadiusKm = radiusKm;
            CircleFillColor = Color.FromRgba(0.7, 0.2, 0.2, 0.4);
        }

        private void CircleModel_PropertyChanging(object sender, PropertyChangedEventArgs e)
            => NotifyICirclePropertiesChanging(e.PropertyName);

        private void CircleModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => NotifyICircleProperties(e.PropertyName);

        private PinModel _Center;
        public PinModel Center
        {
            get { return _Center; }
            set { bool changed = _Center != value; if (changed) { NotifyIAmChanging(); _Center = value; NotifyIChanged(); } }
        }

        private double _RadiusKm;
        public double RadiusKm
        {
            get { return _RadiusKm; }
            set { bool changed = _RadiusKm != value; if (changed) { NotifyIAmChanging(); _RadiusKm = value; NotifyIChanged(); } }
        }



    }
}
