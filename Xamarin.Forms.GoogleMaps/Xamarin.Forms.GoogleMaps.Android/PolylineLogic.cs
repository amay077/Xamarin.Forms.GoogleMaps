using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Platform.Android;
using NativePolyline = Android.Gms.Maps.Model.Polyline;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class PolylineLogic
    {
        List<NativePolyline> _polylines;

        public GoogleMap NativeMap { get; private set; }
        public Map Map { get; private set; }

        public float ScaledDensity { get; internal set; }

        public PolylineLogic()
        {
        }

        internal void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            this.NativeMap = newNativeMap;
            this.Map = newMap;

            Unregister(oldNativeMap, oldMap);

            if (newNativeMap != null)
            {
                newNativeMap.PolylineClick += MapOnPolylineClick;
            }

            var inccPolyline = newMap.Polylines as INotifyCollectionChanged;
            if (inccPolyline != null)
                inccPolyline.CollectionChanged += OnPolylineCollectionChanged;
        }

        internal void Unregister(GoogleMap nativeMap, Map map)
        {
            if (map != null)
            {
                ((ObservableCollection<Polyline>)map.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
            }

            if (nativeMap != null)
            {
                nativeMap.PolylineClick -= MapOnPolylineClick;
            }
        }

        internal void NotifyReset()
        {
            OnPolylineCollectionChanged(Map.Polylines, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnPolylineCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(notifyCollectionChangedEventArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceItems(notifyCollectionChangedEventArgs.OldItems, notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetItems();
                    break;
                case NotifyCollectionChangedAction.Move:
                    //do nothing
                    break;
            }
        }

        void AddItems(IList polylines)
        {
            var map = NativeMap;
            if (map == null)
                return;

            if (_polylines == null)
                _polylines = new List<NativePolyline>();

            _polylines.AddRange(polylines.Cast<Polyline>().Select(line =>
            {
                var polyline = (Polyline)line;
                var opts = new PolylineOptions();

                foreach (var p in polyline.Positions)
                    opts.Add(new LatLng(p.Latitude, p.Longitude));

                opts.InvokeWidth(polyline.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
                opts.InvokeColor(polyline.StrokeColor.ToAndroid());
                opts.Clickable(polyline.IsClickable);

                var nativePolyline = map.AddPolyline(opts);

                // associate pin with marker for later lookup in event handlers
                polyline.Id = nativePolyline;
                return nativePolyline;
            }));
        }

        void RemoveItems(IList polylines)
        {
            var map = NativeMap;
            if (map == null)
                return;
            if (_polylines == null)
                return;

            foreach (Polyline polyline in polylines)
            {
                var apolyline = _polylines.FirstOrDefault(m => ((NativePolyline)polyline.Id).Id == m.Id);
                if (apolyline == null)
                    continue;
                apolyline.Remove();
                _polylines.Remove(apolyline);
            }
        }

        void ReplaceItems(IList oldItems, IList newItems)
        {
            RemoveItems(oldItems);
            AddItems(newItems);
        }

        void ResetItems()
        {
            _polylines?.ForEach(line => line.Remove());
            _polylines = null;
            AddItems((IList)Map.Polylines);
        }

        void MapOnPolylineClick(object sender, GoogleMap.PolylineClickEventArgs eventArgs)
        {
            // clicked polyline
            var clickedPolyline = eventArgs.Polyline;

            // lookup pin
            Polyline targetPolyline = null;
            for (var i = 0; i < Map.Polylines.Count; i++)
            {
                var line = Map.Polylines[i];
                if (((NativePolyline)line.Id).Id != clickedPolyline.Id)
                    continue;

                targetPolyline = line;
                break;
            }

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPolyline?.SendTap();
        }
   }
}

