using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class BasicMapPage : ContentPage
    {
        public BasicMapPage()
        {
            InitializeComponent();

            // MapTypes
            var mapTypeValues = new List<MapType>();
            foreach (var mapType in Enum.GetValues(typeof(MapType)))
            {
                mapTypeValues.Add((MapType)mapType);
                pickerMapType.Items.Add(Enum.GetName(typeof(MapType), mapType));
            }

            pickerMapType.SelectedIndexChanged += (sender, e) => 
            {
                map.MapType = mapTypeValues[pickerMapType.SelectedIndex];
            };
            pickerMapType.SelectedIndex = 0;


            // ZoomEnabled
            switchHasZoomEnabled.Toggled += (sender, e) => 
            {
                map.HasZoomEnabled = e.Value;
            };
            switchHasZoomEnabled.IsToggled = map.HasZoomEnabled;

            // ScrollEnabled
            switchHasScrollEnabled.Toggled += (sender, e) =>
            {
                map.HasScrollEnabled = e.Value;
            };
            switchHasScrollEnabled.IsToggled = map.HasScrollEnabled;

            // IsShowingUser
            switchIsShowingUser.Toggled += (sender, e) =>
            {
                map.IsShowingUser = e.Value;
            };
            switchIsShowingUser.IsToggled = map.IsShowingUser;

            // Geocode
            buttonGeocode.Clicked += async (sender, e) => 
            {
                var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
                var positions = await geocoder.GetPositionsForAddressAsync(entryAddress.Text);
                if (positions.Count() > 0)
                {
                    var pos = positions.First();
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(5000)));
                }
                else
                {
                    await this.DisplayAlert("Not found", "Geocoder returns no results", "Close");
                }
            };
        }
    }
}

