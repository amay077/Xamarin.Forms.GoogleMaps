
namespace Maui.GoogleMaps;

public sealed class BitmapDescriptor
{
    private BitmapDescriptor()
    {
    }

    public string Id { get; private set; }
    public BitmapDescriptorType Type { get; private set; }
    public Color Color { get; private set; }
    public string BundleName { get; private set; }
    public Func<Stream> Stream { get; private set; }
    public string AbsolutePath { get; private set; }
    public Func<View> View { get; private set; }

    internal static BitmapDescriptor DefaultMarker(Color color, string id)
    {
        return new BitmapDescriptor()
        {
            Id = id,
            Type = BitmapDescriptorType.Default,
            Color = color
        };
    }

    internal static BitmapDescriptor FromBundle(string bundleName, string id)
    {
        return new BitmapDescriptor()
        {
            Id = id,
            Type = BitmapDescriptorType.Bundle,
            BundleName = bundleName
        };
    }

    internal static BitmapDescriptor FromStream(Func<Stream> stream, string id)
    {
        return new BitmapDescriptor()
        {
            Id = id,
            Type = BitmapDescriptorType.Stream,
            Stream = stream
        };
    }

    internal static BitmapDescriptor FromStream(Stream stream, string id)
    {
        return new BitmapDescriptor()
        {
            Id = id,
            Type = BitmapDescriptorType.Stream,
            Stream = () => stream
        };
    }

    internal static BitmapDescriptor FromPath(string absolutePath, string id)
    {
        return new BitmapDescriptor()
        {
            Id = id,
            Type = BitmapDescriptorType.AbsolutePath,
            AbsolutePath = absolutePath
        };
    }

    internal static BitmapDescriptor FromView(Func<View> view, string id)
    {
        return new BitmapDescriptor()
        {
            Id = id,
            Type = BitmapDescriptorType.View,
            View = view
        };
    }
}

