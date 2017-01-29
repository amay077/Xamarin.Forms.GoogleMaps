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

            map.CameraChanged += (sender, e) => 
            {
                var p = e.Position;
                labelStatus.Text = $"Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
            };

            buttonMoveToLatLng.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewLatLng(new Position(35.7104, 139.8093)));
            };
        }
    }
}
