using System.ComponentModel;
using System.Windows.Input;

namespace Xamarin.Forms.GoogleMaps
{
    public interface ICircle : INotifyPropertyChanged
    {
        float CircleStrokeWidth { get; }

        Color CircleStrokeColor { get; }

        Color CircleFillColor { get; }

        Position CircleCenter { get; }

        Distance CircleRadius { get; }
    }
    public static class ICircleExtensions
    {
        public static Circle ToCircle(this ICircle iCircle)
        {
            var circle = new Circle() { BindingContext = iCircle };
            circle.SetBinding(Circle.CenterProperty, nameof(ICircle.CircleCenter));
            circle.SetBinding(Circle.FillColorProperty, nameof(ICircle.CircleFillColor));
            circle.SetBinding(Circle.RadiusProperty, nameof(ICircle.CircleRadius));
            circle.SetBinding(Circle.StrokeColorProperty, nameof(ICircle.CircleStrokeColor));
            circle.SetBinding(Circle.StrokeWidthProperty, nameof(ICircle.CircleStrokeWidth));
            return circle;
        }

    }
}
