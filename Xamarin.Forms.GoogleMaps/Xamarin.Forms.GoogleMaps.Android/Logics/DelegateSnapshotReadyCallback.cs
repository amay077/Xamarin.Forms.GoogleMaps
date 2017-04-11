using System;
using Android.Gms.Maps;
using Android.Graphics;

namespace Xamarin.Forms.GoogleMaps.Android.Logics
{
    internal sealed class DelegateSnapshotReadyCallback : Java.Lang.Object, GoogleMap.ISnapshotReadyCallback
    {
        private readonly Action<Bitmap> _handler;

        public DelegateSnapshotReadyCallback(Action<Bitmap> handler)
        {
            _handler = handler;
        }

        public void OnSnapshotReady(Bitmap snapshot)
        {
            _handler?.Invoke(snapshot);
        }
    }
}