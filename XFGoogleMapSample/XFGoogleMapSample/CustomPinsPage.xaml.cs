using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using System.Reflection;

namespace XFGoogleMapSample
{
    public partial class CustomPinsPage : ContentPage
    {
        bool _dirty;

        // default marker colors
        readonly Tuple<string, Color>[] _colors = 
        {
            new Tuple<string, Color>("Green", Color.Green),
            new Tuple<string, Color>("Pink", Color.Pink),
            new Tuple<string, Color>("Aqua", Color.Aqua)
        };

        // bundle(Android:Asset, iOS:Bundle) images
        readonly string[] _bundles = 
        { 
            "image01.png", 
            "image02.png", 
            "image03.png" 
        };

        // PCL side embedded resources
        readonly string[] _streams =
        {
            "marker01.png",
            "marker02.png",
            "marker03.png"
        };

        // The pin
        readonly Pin _pinTokyo = new Pin()
        {
            Type = PinType.Place,
            Label = "Tokyo SKYTREE",
            Address = "Sumida-ku, Tokyo, Japan",
            Position = new Position(35.71d, 139.81d)
        };

        public CustomPinsPage()
        {
            InitializeComponent();

            // Switch contols as toggle
            var switches = new Switch[] { switchPinColor, switchPinBundle, switchPinStream };
            foreach (var sw in switches)
            {
                sw.Toggled += (sender, e) =>
                {
                    if (!e.Value || _dirty)
                        return;

                    _dirty = true;
                    foreach (var s in switches)
                    {
                        if (!object.ReferenceEquals(s, sender))
                            s.IsToggled = false;
                    }
                    _dirty = false;

                    UpdatePinIcon();
                };
            }

            // Default colors
            foreach (var c in _colors)
            {
                buttonPinColor.Items.Add(c.Item1);
            }

            buttonPinColor.SelectedIndexChanged += (_, e) =>
            {
                buttonPinColor.BackgroundColor = _colors[buttonPinColor.SelectedIndex].Item2;
                UpdatePinIcon();
            };
            buttonPinColor.SelectedIndex = 0;

            // Bundle Images
            foreach (var bundle in _bundles)
            {
                buttonPinBundle.Items.Add(bundle);
            }

            buttonPinBundle.SelectedIndexChanged += (_, e) =>
            {
                UpdatePinIcon();
            };
            buttonPinBundle.SelectedIndex = 0;

            // Stream Images
            foreach (var stream in _streams)
            {
                buttonPinStream.Items.Add(stream);
            }

            buttonPinStream.SelectedIndexChanged += (_, e) =>
            {
                UpdatePinIcon();
            };
            buttonPinStream.SelectedIndex = 0;

            // Set to null
            buttonPinSetToNull.Clicked += (sender, e) =>
            {
                _pinTokyo.Icon = null;
                foreach (var sw in switches)
                {
                    sw.IsToggled = false;
                }
            };

            // Pin Draggable
            switchIsDraggable.Toggled += (sender, e) => 
            {
                _pinTokyo.IsDraggable = switchIsDraggable.IsToggled;
            };

            map.PinDragStart += (_, e) => labelDragStatus.Text = $"DragStart - {PrintPin(e.Pin)}";
            map.PinDragging += (_, e) => labelDragStatus.Text = $"Dragging - {PrintPin(e.Pin)}";
            map.PinDragEnd += (_, e) => labelDragStatus.Text = $"DragEnd - {PrintPin(e.Pin)}";

            switchIsDraggable.IsToggled = true;

            switchPinColor.IsToggled = true;

            _pinTokyo.IsDraggable = true;
            map.Pins.Add(_pinTokyo);
            map.SelectedPin = _pinTokyo;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(_pinTokyo.Position, Distance.FromMeters(5000)), true);
        }

        private string PrintPin(Pin pin)
        {
            return $"{pin.Label}({pin.Position.Latitude.ToString("0.000")},{pin.Position.Longitude.ToString("0.000")})";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void UpdatePinIcon()
        {
            if (switchPinColor.IsToggled)
            {
                _pinTokyo.Icon = BitmapDescriptorFactory.DefaultMarker(_colors[buttonPinColor.SelectedIndex].Item2);
            }
            else if (switchPinBundle.IsToggled)
            {
                _pinTokyo.Icon = BitmapDescriptorFactory.FromBundle(buttonPinBundle.Items[buttonPinBundle.SelectedIndex]);
            }
            else if (switchPinStream.IsToggled)
            {
                var assembly = typeof(CustomPinsPage).GetTypeInfo().Assembly;
                var file = buttonPinStream.Items[buttonPinStream.SelectedIndex];
                var stream = assembly.GetManifestResourceStream($"XFGoogleMapSample.{file}");
                _pinTokyo.Icon = BitmapDescriptorFactory.FromStream(stream);
            }
        }
   }
}

