using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
	public partial class ClusterPage : ContentPage
	{
		private const int ClusterItemsCount = 50;// 10000;
		private const double Extent = 0.2;
		private Position _currentPosition = new Position(31.768319, 35.213710);
		private readonly Random _random = new Random();

		public ClusterPage()
		{
			InitializeComponent();

			this.Map.MoveToRegion(MapSpan.FromCenterAndRadius(this._currentPosition, new Distance(100)), true);

			for (var i = 0; i <= ClusterItemsCount; i++)
			{
				var lat = this._currentPosition.Latitude + Extent * GetRandomNumber(-1.0, 1.0);
				var lng = this._currentPosition.Longitude + Extent * GetRandomNumber(-1.0, 1.0);

				this.Map.ClusteredPins.Add(new Pin()
				{
					Position = new Position(lat, lng),
					Label = $"Item {i}",
					Icon = BitmapDescriptorFactory.FromBundle("pin_red.png")
				});
				this.Map.Pins.Add(new Pin()
				{
					Position = new Position(lat, lng),
					Label = $"Item {i}",
					Icon = BitmapDescriptorFactory.FromBundle("pin_red.png")
				});
			}



			//this.Map.ClusterOptions.SetRenderUsingImage(BitmapDescriptorFactory.FromBundle("image01.png"));

			this.Map.Cluster();
		}


		private double GetRandomNumber(double minimum, double maximum)
		{
			return _random.NextDouble() * (maximum - minimum) + minimum;
		}

	}
}