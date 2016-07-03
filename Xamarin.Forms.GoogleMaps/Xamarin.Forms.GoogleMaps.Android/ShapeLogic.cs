using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Android.Gms.Maps;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public abstract class ShapeLogic<T>
    {
        public float ScaledDensity { get; internal set; }

        public GoogleMap NativeMap { get; private set; }
        public Map Map { get; private set; }

        public ShapeLogic()
        {
        }

        internal virtual void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            this.NativeMap = newNativeMap;
            this.Map = newMap;

            Unregister(oldNativeMap, oldMap);

            var inccItems = GetItems(newMap) as INotifyCollectionChanged;
            if (inccItems != null)
                inccItems.CollectionChanged += OnCollectionChanged;
        }

        internal virtual void Unregister(GoogleMap oldNativeMap, Map oldMap)
        {
            if (oldMap != null)
            {
                ((ObservableCollection<T>)GetItems(oldMap)).CollectionChanged -= OnCollectionChanged;
            }
        }

        protected abstract IList<T> GetItems(Map map);

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

        protected abstract void AddItems(IList newItems);

        protected abstract void RemoveItems(IList oldItems);

        protected virtual void ReplaceItems(IList oldItems, IList newItems)
        {
            RemoveItems(oldItems);
            AddItems(newItems);
        }

        protected abstract void ResetItems();

        internal void NotifyReset()
        {
            OnCollectionChanged(GetItems(Map), new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }
}

