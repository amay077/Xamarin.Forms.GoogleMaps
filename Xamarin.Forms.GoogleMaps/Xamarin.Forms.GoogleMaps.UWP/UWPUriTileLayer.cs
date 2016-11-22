﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;

namespace Xamarin.Forms.GoogleMaps.UWP
{
    internal class UWPUriTileLayer : LocalMapTileDataSource
    {
        private readonly Func<int, int, int, Uri> _makeTileUri;

        public UWPUriTileLayer(Func<int, int, int, Uri> makeTileUri, int tileSize = 256)
        {
            _makeTileUri = makeTileUri;
            this.UriRequested += UWPUriTileLayer_UriRequested;
        }

        private void UWPUriTileLayer_UriRequested(LocalMapTileDataSource sender, MapTileUriRequestedEventArgs args)
        {
            var uri = _makeTileUri(args.X, args.Y, args.ZoomLevel);
            args.Request.Uri = uri;
        }
    }
}
