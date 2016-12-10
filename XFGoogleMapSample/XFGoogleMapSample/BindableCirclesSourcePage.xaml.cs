using System;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace XFGoogleMapSample
{
    public partial class BindableCirclesSourcePage : ContentPage
    {
        public BindableContext Context { get; set; }

        public BindableCirclesSourcePage()
        {
            Context = new XFGoogleMapSample.BindableContext();
            this.BindingContext = Context;

            InitializeComponent();

            buttonAddMovablePin.Clicked += (sender, e) =>
            {
                Context.AddNewMovablePinCircle();
            };

            buttonRemoveMovablePin.Clicked += (sender, e) =>
            {
                Context.RemoveMovablePinCircle();
            };

            // Clear Pins
            buttonClearPins.Clicked += (sender, e) =>
            {
                Context.ClearPins();
            };


            map.SelectedPinChanged += SelectedPin_Changed;

            map.InfoWindowClicked += InfoWindow_Clicked;
        }

        private void InfoWindow_Clicked(object sender, InfoWindowClickedEventArgs e)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Context.Status= $"[{time}]InfoWindow Clicked - {e?.Pin?.Label.ToString() ?? "nothing"}";
        }

        void SelectedPin_Changed(object sender, SelectedPinChangedEventArgs e)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Context.Status = $"[{time}]SelectedPin changed - {e?.SelectedPin?.Label ?? "nothing"}";
        }

    }

}

