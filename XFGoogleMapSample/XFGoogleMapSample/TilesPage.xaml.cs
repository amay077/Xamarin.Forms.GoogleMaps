using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
	public partial class TilesPage : ContentPage
	{
		public TilesPage()
		{
			InitializeComponent();

			ITileLayer osmTile  = null;
			ITileLayer jgsiTile = null;

			// OSM Tile
			buttonAddOSMTile.Clicked += (sender, e) =>
			{
				osmTile = new UrlTileLayer((int x, int y, int zoom) => {
					var uriString = String.Format("https://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png", "abc".Substring(new Random().Next(3),1), zoom, x, y);
					return new Uri(uriString);
				});
				osmTile.Tag = "OSMTILE"; // Can set any object

				map.TileLayers.Add(osmTile);

				((Button)sender).IsEnabled = false;
				buttonRemoveOSMTile.IsEnabled = true;
			};

			buttonRemoveOSMTile.Clicked += (sender, e) =>
			{
				map.TileLayers.Remove(osmTile);
				osmTile = null;

				((Button)sender).IsEnabled = false;
				buttonAddOSMTile.IsEnabled = true;
			};
			buttonRemoveOSMTile.IsEnabled = false;

			// Japan GSI Tile
			buttonAddJGSITile.Clicked += (sender, e) =>
			{
				jgsiTile = new UrlTileLayer((int x, int y, int zoom) => {
					var uriString = String.Format("https://cyberjapandata.gsi.go.jp/xyz/std/{0}/{1}/{2}.png", zoom, x, y);
					return new Uri(uriString);
				});
				jgsiTile.Tag = "JGSITILE"; // Can set any object

				map.TileLayers.Add(jgsiTile);

				((Button)sender).IsEnabled = false;
				buttonRemoveJGSITile.IsEnabled = true;
			};

			buttonRemoveJGSITile.Clicked += (sender, e) =>
			{
				map.TileLayers.Remove(jgsiTile);
				jgsiTile = null;

				((Button)sender).IsEnabled = false;
				buttonAddJGSITile.IsEnabled = true;
			};
			buttonRemoveJGSITile.IsEnabled = false;



		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(35.69d, 139.75d), Distance.FromMeters(10000)), false);
		}

	}
}

