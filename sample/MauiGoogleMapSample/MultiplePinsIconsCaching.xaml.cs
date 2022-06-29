using Maui.GoogleMaps;
using System.Reflection;

namespace MauiGoogleMapSample
{
    public partial class MultiplePinsIconsCaching : ContentPage
    {
        bool _dirty;

        // bundle(Resources/Images)
        readonly string[] _bundles =
        {
            "image01",
            "image02",
            "image03"
        };

        // PCL side embedded resources
        readonly string[] _streams =
        {
            "marker01.png",
            "marker02.png",
            "marker03.png"
        };

        public MultiplePinsIconsCaching()
        {
            InitializeComponent();

            List<Pin> pins = new List<Pin>();

            Position startPos = new Position(41.9027835, 12.4963655);

            double fromLong = -0.5;
            double toLong = 0.5;
            double increaseBy = 0.025;
            double fromLat = -0.5;
            double toLat = 0.5;
            double increaseByLat = 0.025;

            int count = 0;
            List<BitmapDescriptor> bitmaps = LoadBitmapDescriptors();

            for (double lon = fromLong; lon <= toLong; lon += increaseBy)
            {
                for (double lat = fromLat; lat <= toLat; lat += increaseByLat)
                {
                    var pin = new Pin();

                    pin.Label = count.ToString();
                    pin.Icon = bitmaps[count % bitmaps.Count];
                    pin.Position = new Position(startPos.Latitude + lat, startPos.Longitude + lon);

                    map.Pins.Add(pin);

                    ++count;
                }
            }

            lblPinCount.Text = count.ToString();
            map.MoveToRegion(MapSpan.FromCenterAndRadius(startPos, Distance.FromMeters(200000)), true);
        }

        private List<BitmapDescriptor> LoadBitmapDescriptors()
        {
            var assembly = typeof(CustomPinsPage).GetTypeInfo().Assembly;
            var descriptors = new List<BitmapDescriptor>();

            descriptors.AddRange(
                _streams.Select(fileName =>
                    BitmapDescriptorFactory.FromStream(
                        GetResourceStream(fileName, assembly),
                        id: fileName) // for caching bitmaps from streams providing id is necessary
                    )
            );

            descriptors.AddRange(
                _bundles.Select(bundle =>
                    BitmapDescriptorFactory.FromBundle(bundle)
                )
            );

            return descriptors;
        }

        private static System.IO.Stream GetResourceStream(string fileName, Assembly assembly)
        {
            return assembly.GetManifestResourceStream($"MauiGoogleMapSample.{fileName}") ?? assembly.GetManifestResourceStream($"MauiGoogleMapSample.local.{fileName}");
        }
    }
}

