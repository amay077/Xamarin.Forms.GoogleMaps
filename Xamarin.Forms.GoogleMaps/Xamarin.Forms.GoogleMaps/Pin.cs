﻿using System;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class Pin : BindableObject
    {
        public static readonly BindableProperty TypeProperty = BindableProperty.Create("Type", typeof(PinType), typeof(Pin), default(PinType));

        public static readonly BindableProperty PositionProperty = BindableProperty.Create("Position", typeof(Position), typeof(Pin), default(Position));

        public static readonly BindableProperty LabelProperty = BindableProperty.Create("Label", typeof(string), typeof(Pin), default(string));

        public static readonly BindableProperty AddressProperty = BindableProperty.Create("Address", typeof(string), typeof(Pin), default(string));

        public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(BitmapDescriptor), typeof(Pin), default(BitmapDescriptor));

        public static readonly BindableProperty IsDraggableProperty = BindableProperty.Create("IsDraggable", typeof(bool), typeof(Pin), false);

        public static readonly BindableProperty RotationProperty = BindableProperty.Create("Rotation", typeof(float), typeof(Pin), 0f);

        public static readonly BindableProperty AppearAnimationProperty = BindableProperty.Create(nameof(AppearAnimation), typeof(AppearMarkerAnimation), typeof(Pin), default(AppearMarkerAnimation));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public Position Position
        {
            get { return (Position)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public PinType Type
        {
            get { return (PinType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public BitmapDescriptor Icon
        {
            get { return (BitmapDescriptor)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public bool IsDraggable
        {
            get { return (bool)GetValue(IsDraggableProperty); }
            set { SetValue(IsDraggableProperty, value); }
        }

        public float Rotation
        {
            get { return (float)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        public AppearMarkerAnimation AppearAnimation
        {
            get { return (AppearMarkerAnimation)GetValue(AppearAnimationProperty); }
            set { SetValue(AppearAnimationProperty, value); }
        }



        public object Tag { get; set; }

        public object NativeObject { get; internal set; }

        [Obsolete("Please use Map.PinClicked instead of this")]
        public event EventHandler Clicked;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Pin)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Label?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (Address?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Pin left, Pin right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Pin left, Pin right)
        {
            return !Equals(left, right);
        }

        internal bool SendTap()
        {
            EventHandler handler = Clicked;
            if (handler == null)
                return false;

            handler(this, EventArgs.Empty);
            return true;
        }

        bool Equals(Pin other)
        {
            return string.Equals(Label, other.Label) && Equals(Position, other.Position) && Type == other.Type && string.Equals(Address, other.Address);
        }
    }
}