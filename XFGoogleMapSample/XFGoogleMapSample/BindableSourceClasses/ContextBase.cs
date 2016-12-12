using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Interfaces.SimpleImplementations;

namespace XFGoogleMapSample
{
    public class ContextBase : NotifyClass
    {
        public ContextBase()
        {
            PropertyChanged += BindableContext_PropertyChanged;
        }

        protected MapSpan _MapRegion = new MapSpan(new Position(48.858, 2.294), 1, 1);
        public MapSpan MapRegion { get { return _MapRegion; } set { bool changed = _MapRegion != value; if (changed) { NotifyIAmChanging(); _MapRegion = value; NotifyIChanged(); } } }

        private IPin _SelectedItem;
        public IPin SelectedItem { get { return _SelectedItem; } set { bool changed = _SelectedItem != value; if (changed) { NotifyIAmChanging(); _SelectedItem = value; NotifyIChanged(); } } }

        private string _Status = "Shows status here";
        public string Status { get { return _Status; } set { bool changed = _Status != value; if (changed) { NotifyIAmChanging(); _Status = value; NotifyIChanged(); } } }

        public ObservableCollection<ICircle> Circles { get; set; } = new ObservableCollection<ICircle>();
        public ObservableCollection<IPin> Pins { get; set; } = new ObservableCollection<IPin>();

        public PinModel AddPin(string name, string details, double lat, double lng)
        {
            var pin = new PinModel() { Name = name, Details = details, Latitude = lat, Longitude = lng };
            Pins.Add(pin);
            return pin;
        }

        public ICircle AddCircle(Position center, double radiusKm)
        {
            var circle = new SimpleCircle(center, radiusKm);
            Circles.Add(circle);
            return circle;
        }

        protected virtual void BindableContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MapRegion):
                    Status = "MapRegion " + MapRegion.ToString();
                    break;
            }
        }

        public ICommand RemovePinCommand { get { return new Command<IPin>((IPin pin) => RemovePin(pin)); } }
        public void RemovePin(IPin pin)
            => Pins.Remove(pin);

        public ICommand SelectPinCommand { get { return new Command<IPin>((IPin pin) => SelectPin(pin)); } }
        public void SelectPin(IPin pin)
            => SelectedItem = pin;

        public ICommand ClearPinsCommand { get { return new Command(() => ClearPins()); } }
        internal void ClearPins()
        {
            var circle = Circles.FirstOrDefault();
            if (circle != null) Circles.Remove(circle);
            var pins = Pins.ToList();
            foreach (var p in pins) Pins.Remove(p);
        }

    }
}
