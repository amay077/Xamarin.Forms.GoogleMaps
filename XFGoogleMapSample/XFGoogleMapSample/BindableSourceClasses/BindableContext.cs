using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public class BindableContext : ContextBase
    {
        public MoveDirection Direction { get; set; }


        const string CATEGORY_MOVABLE = "movable";
        const string CATEGORY_RANDOM = "random";

        override protected void BindableContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MapRegion):
                    Status = "MapRegion " + MapRegion.ToString();
                    break;
                case nameof(SelectedItem):
                    var time = DateTime.Now.ToString("hh:mm:ss");
                    Status = $"[{time}]SelectedPin changed - {SelectedItem?.PinTitle ?? "nothing"}";
                    break;
            }
        }

        #region Bindable PinsSource
        public ICommand AddNewMovablePinCommand { get { return new Command(() => AddNewMovablePin()); } }
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

            pin.PinConfig.InfoWindowClickedCommand = new Command((object o) => { pin.Move100m(Direction); }, (o) => true);

            Pins.Add(pin);
        }

        public ICommand RemoveMovablePinCommand { get { return new Command(() => RemoveMovablePin()); } }
        internal void RemoveMovablePin()
        {
            var pin = Pins.FirstOrDefault(p => ((p as PinModel)?.Category == CATEGORY_MOVABLE));
            if (pin != null) Pins.Remove(pin);
        }

        int PinNumber { get; set; } = 1;
        public ICommand AddPinParisCommand { get { return new Command(() => AddPinParis()); } }
        internal void AddPinParis()
        {
            Random r = new Random();
            var lat = 48.855651 + 0.5 * r.NextDouble();
            var lng = 2.347134 + 0.5 * r.NextDouble();
            var pin = new PinModel() { Name = "Random pin " + (PinNumber++).ToString(), Details = "click me to update", Latitude = lat, Longitude = lng };
            pin.Category = CATEGORY_RANDOM;


            if(r.NextDouble()<1d/3)
                pin.PinConfig.AppearAnimation = AppearMarkerAnimation.Fall;
            else if(r.NextDouble()<1d/2)
                pin.PinConfig.AppearAnimation = AppearMarkerAnimation.FadeIn;
            else
                pin.PinConfig.AppearAnimation = AppearMarkerAnimation.Shake;

            pin.PinConfig.InfoWindowClickedCommand = new Command((object o) =>
            {
                pin.Name = "Random pin: " + (PinNumber++);
                pin.Details = "Illustration of non-updating callout. Need to display it again by tapping on the pin.";
            }, (o) => true);

            Pins.Add(pin);
        }

        public ICommand RemovePinParisCommand { get { return new Command(() => RemovePinParis()); } }
        internal void RemovePinParis()
        {
            var pin = Pins.LastOrDefault(p => ((p as PinModel)?.Category == CATEGORY_RANDOM));
            if (pin != null) Pins.Remove(pin);
        }
        #endregion Bindable PinsSource

        #region Bindable CirclesSource

        public ICommand AddNewMovablePinCircleCommand { get { return new Command(() => AddNewMovablePinCircle()); } }
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

            pin.PinConfig.InfoWindowClickedCommand = new Command((object o) => { if (!Circles.Any()) Circles.Add((new CirclePinModel(pin, 2))); var circle = Circles.FirstOrDefault() as CirclePinModel; circle.Center = pin; pin.Move100m(Direction); }, (o) => true);

            Pins.Add(pin);
        }

        public ICommand RemoveMovablePinCircleCommand { get { return new Command(() => RemoveMovablePinCircle()); } }
        internal void RemoveMovablePinCircle()
        {
            var circle = Circles.FirstOrDefault();
            if (circle != null) Circles.Remove(circle);
            var pin = Pins.FirstOrDefault(p => ((p as PinModel)?.Category == CATEGORY_MOVABLE));
            if (pin != null) Pins.Remove(pin);
        }

        #endregion Bindable CirclesSource

        public ICommand DispInfoWindowClickedCommand { get { return new Command<InfoWindowClickedEventArgs>((InfoWindowClickedEventArgs iw) => DispInfoWindowClicked(iw.Item)); } }
        public void DispInfoWindowClicked(IPin pin)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Status = $"[{time}]InfoWindow Clicked - {pin?.PinTitle ?? "nothing"}";
        }

        public ICommand PinClickedCommand { get { return new Command<PinClickedEventArgs>((PinClickedEventArgs arg) => PinClicked(arg.Item)); } }
        public void PinClicked(IPin pin)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Status = $"[{time}]Pin Clicked - {pin?.PinTitle ?? "nothing"}";
        }

    }
}
