﻿using System;
namespace Xamarin.Forms.GoogleMaps.Internals
{
    public interface IMapRequestDelegate
    {
        void OnMoveToRegionRequest(MoveToRegionMessage m);
        void OnMoveCameraRequest(CameraUpdateMessage m);
    }
}
