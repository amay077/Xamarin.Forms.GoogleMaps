using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class MovingCircleModel
    {
        private Position _CircleCenter;
        public Position CircleCenter
        {
            get { return _CircleCenter; }
            set { bool changed = _CircleCenter != value; if (changed) { OnPropertyChanging(); _CircleCenter = value; OnPropertyChanged(); } }
        }

        private Color _CircleFillColor;
        public Color CircleFillColor
        {
            get { return _CircleFillColor; }
            set { bool changed = _CircleFillColor != value; if (changed) { OnPropertyChanging(); _CircleFillColor = value; OnPropertyChanged(); } }
        }

        private Distance _CircleRadius;
        public Distance CircleRadius
        {
            get { return _CircleRadius; }
            set { bool changed = _CircleRadius != value; if (changed) { OnPropertyChanging(); _CircleRadius = value; OnPropertyChanged(); } }
        }

        private Color _CircleStrokeColor;
        public Color CircleStrokeColor
        {
            get { return _CircleStrokeColor; }
            set { bool changed = _CircleStrokeColor != value; if (changed) { OnPropertyChanging(); _CircleStrokeColor = value; OnPropertyChanged(); } }
        }

        private float _CircleStrokeWidth;
        public float CircleStrokeWidth
        {
            get { return _CircleStrokeWidth; }
            set { bool changed = _CircleStrokeWidth != value; if (changed) { OnPropertyChanging(); _CircleStrokeWidth = value; OnPropertyChanged(); } }
        }
    }
}
