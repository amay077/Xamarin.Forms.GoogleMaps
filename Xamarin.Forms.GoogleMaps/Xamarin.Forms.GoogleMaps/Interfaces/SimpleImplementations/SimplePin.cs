﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xamarin.Forms.GoogleMaps.Interfaces.SimpleImplementations
{
    public class SimplePin : IPin
    {
        public ICommand SelectedCommand { get; set; }

        public object SelectedCommandParameter { get; set; }

        public ICommand InfoWindowClickedCommand { get; set; }

        public object InfoWindowClickedCommandParameter { get; set; }

        public ICommand PinClickedCommand { get; set; }

        public object PinClickedCommandParameter { get; set; }

        public BitmapDescriptor PinIcon { get; set; }

        public bool PinIsDraggable { get; set; }

        public Position PinPosition { get; set; }

        public float PinRotation { get; set; }

        public string PinSubTitle { get; set; }

        public string PinTitle { get; set; }

        public PinType PinType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
