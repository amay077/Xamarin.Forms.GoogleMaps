using System;
using System.IO;

namespace Xamarin.Forms.GoogleMaps.Internals
{
    internal sealed class TakeSnapshotMessage
    {
        public Action<Stream> OnSnapshot { get; }

        public TakeSnapshotMessage(Action<Stream> onSnapshot)
        {
            OnSnapshot = onSnapshot;
        }
    }
}