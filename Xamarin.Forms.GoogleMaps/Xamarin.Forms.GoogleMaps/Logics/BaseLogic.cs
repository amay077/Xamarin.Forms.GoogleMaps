using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xamarin.Forms.GoogleMaps.Logics
{
    internal abstract class BaseLogic<TNativeMap>
    {
        public float ScaledDensity { get; internal set; }

        public TNativeMap NativeMap { get; private set; }
        public Map Map { get; private set; }

        protected abstract INotifyCollectionChanged GetItemAsNotifyCollectionChanged(Map map);

        internal virtual void Register(TNativeMap oldNativeMap, Map oldMap, TNativeMap newNativeMap, Map newMap)
        {
            this.NativeMap = newNativeMap;
            this.Map = newMap;

            Unregister(oldNativeMap, oldMap);

            var inccItems = GetItemAsNotifyCollectionChanged(newMap);
            if (inccItems != null)
                inccItems.CollectionChanged += OnCollectionChanged;
        }

        internal virtual void Unregister(TNativeMap nativeMap, Map map)
        {
            if (map != null)
            {
                var inccItems = GetItemAsNotifyCollectionChanged(map);
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


        protected abstract void AddItems(IList newItems);

        protected abstract void RemoveItems(IList oldItems);

        protected void ReplaceItems(IList oldItems, IList newItems)
        {
            RemoveItems(oldItems);
            AddItems(newItems);
        }

        protected abstract void ResetItems();

        internal abstract void NotifyReset();

        internal abstract void RestoreItems();

        internal virtual void OnMapPropertyChanged(PropertyChangedEventArgs e)
        {
        }
    }
}