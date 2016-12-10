using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public class BindableContext : NotifyClass
    {
        public MoveDirection Direction { get; set; }

        private MapSpan _MapRegion = new MapSpan(new Position(48.858, 2.294), 1, 1);
        public MapSpan MapRegion { get { return _MapRegion; } set { bool changed = _MapRegion != value; _MapRegion = value; if (changed) NotifyPropertyChanged(nameof(MapRegion)); } }

        const string CATEGORY_MOVABLE = "movable";
        const string CATEGORY_RANDOM = "random";

        public ObservableCollection<IPin> Pins { get; set; } = new ObservableCollection<IPin>();
        public PinModel AddPin(string name, string details, double lat, double lng)
        {
            var pin = new PinModel() { Name = name, Details = details, Latitude = lat, Longitude = lng };
            Pins.Add(pin);
            return pin;
        }

        public void AddNewMovablePin()
        {
            var pin = new PinModel()
            {
                Name = "Eiffel tower",
                Details = "Click me to move up!",
                Latitude = 48.858391,
                Longitude = 2.294267,
                Category = CATEGORY_MOVABLE
            };

            pin.CallOutClickedCommand = new Command((object o) => { pin.Move100m(Direction); }, (o) => true);

            Pins.Add(pin);
        }

        internal void RemoveMovablePin()
        {
            var pin = Pins.FirstOrDefault(p => ((p as PinModel)?.Category == CATEGORY_MOVABLE));
            if (pin != null) Pins.Remove(pin);
        }

        int PinNumber { get; set; } = 1;
        internal void AddPinParis()
        {
            Random r = new Random();
            var lat = 48.855651 + 0.5 * r.NextDouble();
            var lng = 2.347134 + 0.5 * r.NextDouble();
            var pin = AddPin("Random pin " + (PinNumber++).ToString(), "click me to update", lat, lng);
            pin.Category = CATEGORY_RANDOM;
            pin.CallOutClickedCommand = new Command((object o) =>
            {
                pin.Name = "Random pin: " + (PinNumber++);
                pin.Details = "Illustration of non-updating callout. Need to display it again by tapping on the pin.";
            }, (o) => true);
        }

        internal void RemovePinParis()
        {
            var pin = Pins.LastOrDefault(p => ((p as PinModel)?.Category == CATEGORY_RANDOM));
            if (pin != null) Pins.Remove(pin);
        }


        internal void ClearPins()
        {
            var pins = Pins.ToList();
            foreach (var p in pins) Pins.Remove(p);
        }
    }
}
