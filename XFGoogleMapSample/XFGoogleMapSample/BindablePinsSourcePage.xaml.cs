using System;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace XFGoogleMapSample
{
    public partial class BindablePinsSourcePage : ContentPage
    {
        public BindableContext Context { get; set; }

        public BindablePinsSourcePage()
        {
            Context = new XFGoogleMapSample.BindableContext();
            this.BindingContext = Context;

            InitializeComponent();

            buttonAddMovablePin.Clicked += (sender, e) =>
            {
                Context.AddNewMovablePin();
            };

            buttonRemoveMovablePin.Clicked += (sender, e) =>
            {
                Context.RemoveMovablePin();
            };

            buttonAddPinParis.Clicked += (sender, e) =>
            {
                Context.AddPinParis();
            };

            buttonRemovePinParis.Clicked += (sender, e) =>
            {
                Context.RemovePinParis();
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
            labelStatus.Text = $"[{time}]InfoWindow Clicked - {e?.Pin?.Label.ToString() ?? "nothing"}";
        }

        void SelectedPin_Changed(object sender, SelectedPinChangedEventArgs e)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            labelStatus.Text = $"[{time}]SelectedPin changed - {e?.SelectedPin?.Label ?? "nothing"}";
        }

    }

}

