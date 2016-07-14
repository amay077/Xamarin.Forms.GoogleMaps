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

        readonly Tuple<string, Color>[] _colors = 
        {
            new Tuple<string, Color>("Green", Color.Green),
            new Tuple<string, Color>("Pink", Color.Pink),
            new Tuple<string, Color>("Aqua", Color.Aqua)
        };

        readonly string[] _bundles = 
        { 
            "image01.png", 
            "image02.png", 
            "image03.png" 
        };

        readonly string[] _streams =
        {
            "marker01.png",
            "marker02.png",
            "marker03.png"
        };

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

            foreach (var bundle in _bundles)
            {
                buttonPinBundle.Items.Add(bundle);
            }

            buttonPinBundle.SelectedIndexChanged += (_, e) =>
            {
                UpdatePinIcon();
            };
            buttonPinBundle.SelectedIndex = 0;

            foreach (var stream in _streams)
            {
                buttonPinStream.Items.Add(stream);
            }

            buttonPinStream.SelectedIndexChanged += (_, e) =>
            {
                UpdatePinIcon();
            };
            buttonPinStream.SelectedIndex = 0;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            switchPinColor.IsToggled = true;

            await Task.Delay(1000); // workaround for #30 [Android]Map.Pins.Add doesn't work when page OnAppearing

            map.Pins.Add(_pinTokyo);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(_pinTokyo.Position, Distance.FromMeters(5000)));

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

