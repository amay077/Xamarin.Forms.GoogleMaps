using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Android.Gms.Maps;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public abstract class ShapeLogic<TOuter, TNative> 
        where TOuter : class 
        where TNative : class
    {
        public float ScaledDensity { get; internal set; }

        public GoogleMap NativeMap { get; private set; }
        public Map Map { get; private set; }

        private List<TNative> _nativeShapes;

        public ShapeLogic()
        {
        }

        protected abstract IList<TOuter> GetItems(Map map);

        protected abstract TNative CreateNativeItem(TOuter outerShape);

        protected abstract TNative DeleteNativeItem(TOuter outerShape);

        internal virtual void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            this.NativeMap = newNativeMap;
            this.Map = newMap;

            Unregister(oldNativeMap, oldMap);

            var inccItems = GetItems(newMap) as INotifyCollectionChanged;
            if (inccItems != null)
                inccItems.CollectionChanged += OnCollectionChanged;
        }

        internal virtual void Unregister(GoogleMap nativeMap, Map map)
        {
            if (map != null)
            {
                var inccItems = GetItems(map) as INotifyCollectionChanged;
                if (inccItems != null)
                    inccItems.CollectionChanged -= OnCollectionChanged;
            }
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
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

        protected void AddItems(IList newItems)
        {
            if (NativeMap == null)
                return;

            if (_nativeShapes == null)
                _nativeShapes = new List<TNative>();

            _nativeShapes.AddRange(newItems.Cast<TOuter>().Select(outerShape =>
                CreateNativeItem(outerShape)));
        }

        protected void RemoveItems(IList oldItems)
        {
            var map = NativeMap;
            if (map == null)
                return;
            if (_nativeShapes == null)
                return;

            foreach (TOuter outerShape in oldItems)
            {
                var deletedShape = DeleteNativeItem(outerShape);
                if (deletedShape != null)
                {
                    _nativeShapes.Remove(deletedShape);
                }
            }
        }

        protected virtual void ReplaceItems(IList oldItems, IList newItems)
        {
            RemoveItems(oldItems);
            AddItems(newItems);
        }

        protected void ResetItems()
        {
            foreach (var outerShape in GetItems(Map))
            {
                DeleteNativeItem(outerShape);
            }

            _nativeShapes = null;
            AddItems((IList)GetItems(Map));
        }

        internal void NotifyReset()
        {
            OnCollectionChanged(GetItems(Map), new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }
}

