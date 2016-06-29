using System;
namespace Xamarin.Forms.GoogleMaps
{
	public interface ITileLayer
	{
		object Tag { get; set; }
	}
	internal interface ITileLayerInternal : ITileLayer
	{
		object Id { get; set; }
	}
}

