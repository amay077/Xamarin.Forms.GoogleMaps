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
    public class BindableCirclesContext : ContextBase
    {
        public MoveDirection Direction { get; set; }

        const string CATEGORY_MOVABLE = "movable";
        const string CATEGORY_MOVING = "moving";
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


        #region Bindable CirclesSource

        int PinNumber = 0;
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

            pin.InfoWindowClickedCommand = new Command((object o) => { if (!Circles.Any()) Circles.Add((new CirclePinModel(pin, 2))); var circle = Circles.FirstOrDefault() as CirclePinModel; circle.Center = pin; pin.Move100m(Direction); }, (o) => true);

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

        public ICommand AddNewMovingCircleCommand { get { return new Command(() => AddNewMovingCircle()); } }
        public void AddNewMovingCircle()
        {
            var circle = new MovingCircleModel(MapRegion.Center, 10, 30);
            Circles.Add(circle);
            circle.Moving = true;
        }

        public ICommand RemoveMovingCircleCommand { get { return new Command(() => RemoveMovingCircle()); } }
        internal void RemoveMovingCircle()
        {
            var circle = Circles.FirstOrDefault();
            if (circle != null) { if (circle is MovingCircleModel) (circle as MovingCircleModel).Moving = false; Circles.Remove(circle); }
        }

        public ICommand StartStopCirclesCommand { get { return new Command(() => StartStopCircles()); } }
        internal void StartStopCircles()
        {
            foreach (var circle in Circles.OfType<MovingCircleModel>())
                circle.Moving = !circle.Moving;
        }




        #endregion Bindable CirclesSource

        public ICommand DispInfoWindowClickedCommand { get { return new Command<IPin>((IPin pin) => DispInfoWindowClicked(pin)); } }
        public void DispInfoWindowClicked(IPin pin)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Status = $"[{time}]InfoWindow Clicked - {pin?.PinTitle ?? "nothing"}";
        }

        public ICommand PinClickedCommand { get { return new Command<IPin>((IPin pin) => PinClicked(pin)); } }
        public void PinClicked(IPin pin)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Status = $"[{time}]Pin Clicked - {pin?.PinTitle ?? "nothing"}";
        }

    }
}
