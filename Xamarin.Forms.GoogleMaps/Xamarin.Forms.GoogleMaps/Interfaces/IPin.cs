using System.ComponentModel;
using System.Windows.Input;

namespace Xamarin.Forms.GoogleMaps
{
    public interface IPin : INotifyPropertyChanged
    {
        string PinTitle { get; }

        string PinSubTitle { get; }

        /// <summary>
        /// PinPosition needs a setter to be bindable
        /// </summary>
        Position PinPosition { get; }

        PinType PinType { get; } 

        BitmapDescriptor PinIcon { get; }

        bool PinIsDraggable { get; }

        float PinRotation { get;  }

        ICommand CallOutClickedCommand { get; }

        object CallOutClickedCommandParameter { get; }
    }
    public static class IPinExtensions
    {
        public static Pin ToPin(this IPin iPin)
        {
            var pin = new Pin() { BindingContext = iPin };
            pin.SetBinding(Pin.LabelProperty, nameof(IPin.PinTitle));
            pin.SetBinding(Pin.AddressProperty, nameof(IPin.PinSubTitle));
            pin.SetBinding(Pin.IconProperty, nameof(IPin.PinIcon));
            pin.SetBinding(Pin.IsDraggableProperty, nameof(IPin.PinIsDraggable));
            pin.SetBinding(Pin.PositionProperty, nameof(IPin.PinPosition));
            pin.SetBinding(Pin.RotationProperty, nameof(IPin.PinRotation));
            pin.SetBinding(Pin.TypeProperty, nameof(IPin.PinType));
            // TODO: unregister on pin remove?
            pin.Clicked += (object sender, System.EventArgs e) => { if (iPin?.CallOutClickedCommand?.CanExecute(iPin?.CallOutClickedCommandParameter) ?? false) iPin.CallOutClickedCommand.Execute(iPin.CallOutClickedCommandParameter); };
            return pin;
        }

    }
}
