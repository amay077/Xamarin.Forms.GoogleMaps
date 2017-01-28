using System;
namespace Xamarin.Forms.GoogleMaps.Internals
{
    internal interface IMapRequestDelegate
    {
        void OnMoveToRegion(MoveToRegionMessage m);
    }
}
