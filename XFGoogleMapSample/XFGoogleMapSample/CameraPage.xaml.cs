using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class CameraPage : ContentPage
    {
        public CameraPage()
        {
            InitializeComponent();

            buttonMoveToLatLng.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewLatLng(new Position(35.7104, 139.8093)));
            };
        }
    }
}
