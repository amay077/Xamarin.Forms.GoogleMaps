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
        public MapSpan MapRegion
        {
            get { return _MapRegion; }
            set
            {
                bool changed = _MapRegion != value; _MapRegion = value; if (changed) NotifyPropertyChanged(nameof(MapRegion));
            }
        }

        private IPin _SelectedItem;
        public IPin SelectedItem
        {
            get { return _SelectedItem; }
            set { bool changed = _SelectedItem != value; if (changed) { NotifyIAmChanging(); _SelectedItem = value; NotifyIChanged(); } }
        }


        const string CATEGORY_MOVABLE = "movable";
        const string CATEGORY_RANDOM = "random";

        public ObservableCollection<ICircle> Circles { get; set; } = new ObservableCollection<ICircle>();
        public ObservableCollection<IPin> Pins { get; set; } = new ObservableCollection<IPin>();
        public PinModel AddPin(string name, string details, double lat, double lng)
        {
            var pin = new PinModel() { Name = name, Details = details, Latitude = lat, Longitude = lng };
            Pins.Add(pin);
            return pin;
        }

        public BindableContext()
        {
            PropertyChanged += BindableContext_PropertyChanged;
        }

        private void BindableContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MapRegion):
                    Status = "MapRegion " + MapRegion.ToString();
                    break;
            }
        }


        #region Bindable PinsSource
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
        #endregion Bindable PinsSource

        #region Bindable CirclesSource

        public void AddNewMovablePinCircle()
        {
            Random r = new Random();
            var pin = new PinModel()
            {
                Name = "Eiffel tower " + (PinNumber++),
                Details = "Click me to move up!",
                Latitude = 48.858391 + 0.1 * r.NextDouble(),
                Longitude = 2.294267 + 0.1 * r.NextDouble(),
                Category = CATEGORY_MOVABLE
            };

            pin.CallOutClickedCommand = new Command((object o) => { if (!Circles.Any()) Circles.Add((new CircleModel(pin, 2))); var circle = Circles.FirstOrDefault() as CircleModel; circle.Center = pin; pin.Move100m(Direction); }, (o) => true);

            Pins.Add(pin);
        }

        internal void RemoveMovablePinCircle()
        {
            var circle = Circles.FirstOrDefault();
            if (circle != null) Circles.Remove(circle);
            var pin = Pins.FirstOrDefault(p => ((p as PinModel)?.Category == CATEGORY_MOVABLE));
            if (pin != null) Pins.Remove(pin);
        }


        #endregion Bindable CirclesSource

        #region Bindable Properties

        internal void RemovePin(IPin pin)
        {
            Pins.Remove(pin);
        }

        internal void SelectPin(IPin pin)
        {
            SelectedItem = pin;
        }

        #endregion Bindable Properties


        internal void ClearPins()
        {
            var circle = Circles.FirstOrDefault();
            if (circle != null) Circles.Remove(circle);
            var pins = Pins.ToList();
            foreach (var p in pins) Pins.Remove(p);
        }

        private string _Status = "Show status when Pin selected.";
        public string Status
        {
            get { return _Status; }
            set { bool changed = _Status != value; _Status = value; if (changed) NotifyPropertyChanged(nameof(Status)); }
        }

    }
}
