using System;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Interfaces.SimpleImplementations;

namespace XFGoogleMapSample
{
    public partial class MovingCircleModel : NotifyClass, ICircle
    {
        public MovingCircleModel(Position center, double radiusKm, double movingRadiusKm) : this()
        {
            CircleCenter = center;
            CircleRadius = Distance.FromKilometers(radiusKm);
            MovingRadiusKm = movingRadiusKm; InitialCenter = center; NextTarget = center;
        }

        public MovingCircleModel()
        {
            CircleFillColor = Color.FromRgba(0.7, 0.2, 0.2, 0.4);
            PropertyChanged += MovingCircle_PropertyChanged;
        }

        private void MovingCircle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Moving))
                if (Moving) StartTimer();
        }

        private void StartTimer()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(100), (Func<bool>)(() => { if (!this.Moving) return false; MakeMove(); return (bool)this.Moving; }));
        }

        private void MakeMove()
        {
        if (NextTarget == CircleCenter)
            {
                Random r = new Random();
                var span = MapSpan.FromCenterAndRadius(InitialCenter, Distance.FromKilometers(MovingRadiusKm));
                var lat = (-0.5 + r.NextDouble()) * span.LatitudeDegrees+InitialCenter.Latitude;
                var lng = (-0.5 + r.NextDouble()) * span.LongitudeDegrees + InitialCenter.Longitude;
                CircleFillColor = Color.FromRgba(r.NextDouble(), r.NextDouble(), r.NextDouble(), 0.4);
                NextTarget = new Position(lat, lng);
            }
            var speed = MovingRadiusKm / 20; // 2 seconds for a radius move
            var dist = CircleCenter.KmTo(NextTarget);
            if (dist < speed)
            {
                CircleCenter = NextTarget;
                return;
            }
            CircleCenter = CircleCenter.TranslateToDirectionDistance(NextTarget, speed);
        }

        private bool _Moving;
        public bool Moving { get { return _Moving; } set { bool changed = _Moving != value; if (changed) { NotifyIAmChanging(); _Moving = value; NotifyIChanged(); } } }

        private double _MovingRadiusKm;
        public double MovingRadiusKm { get { return _MovingRadiusKm; } set { bool changed = _MovingRadiusKm != value; if (changed) { NotifyIAmChanging(); _MovingRadiusKm = value; NotifyIChanged(); } } }

        private Position _InitialCenter;
        public Position InitialCenter { get { return _InitialCenter; } set { bool changed = _InitialCenter != value; if (changed) { NotifyIAmChanging(); _InitialCenter = value; NotifyIChanged(); } } }

        private Position _NextTarget;
        public Position NextTarget { get { return _NextTarget; } set { bool changed = _NextTarget != value; if (changed) { NotifyIAmChanging(); _NextTarget = value; NotifyIChanged(); } } }
    }
}
