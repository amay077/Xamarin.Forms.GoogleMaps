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
        public ContextBase Context { get; set; }

        public BindableCirclesSourcePage()
        {
            Context = new XFGoogleMapSample.BindableCirclesContext();
            this.BindingContext = Context;

            InitializeComponent();
        }
    }

}

