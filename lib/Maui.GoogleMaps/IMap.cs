
namespace Maui.GoogleMaps
{
    public interface IMap : IView
    {
        public MapType MapType { get; set; }
        public Thickness Padding { get; set; }
        public bool IsTrafficEnabled { get; set; }
        public bool IsIndoorEnabled { get; set; }
        public bool MyLocationEnabled { get; set; }
        public MapStyle MapStyle { get; set; }
        public Pin SelectedPin { get; set; }
    }
}
