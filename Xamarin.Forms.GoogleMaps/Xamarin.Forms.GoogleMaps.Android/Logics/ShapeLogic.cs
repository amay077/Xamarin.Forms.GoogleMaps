using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal abstract class ShapeLogic<TOuter, TNative> : BaseLogic
        where TOuter : class 
        where TNative : class
    {
        readonly IList<TOuter> _outerItems = new List<TOuter>(); // Only for ResetItems.

        public ShapeLogic()
        {
        }

        protected abstract IList<TOuter> GetItems(Map map);

        protected override INotifyCollectionChanged GetItemAsNotifyCollectionChanged(Map map) =>
            GetItems(map) as INotifyCollectionChanged;

        protected abstract TNative CreateNativeItem(TOuter outerItem);

        protected abstract TNative DeleteNativeItem(TOuter outerItem);

        protected override void AddItems(IList newItems)
        {
            if (NativeMap == null)
                return;

            foreach (TOuter outerItem in newItems)
            {
                if (CreateNativeItem(outerItem) != null)
                    _outerItems.Add(outerItem);
            }
        }

        protected override void RemoveItems(IList oldItems)
        {
            var map = NativeMap;
            if (map == null)
                return;

            foreach (TOuter outerShape in oldItems)
            {
                if (DeleteNativeItem(outerShape) != null)
                    _outerItems.Remove(outerShape);
            }
        }

        protected override void ResetItems()
        {
            foreach (var outerShape in _outerItems)
                DeleteNativeItem(outerShape);

            _outerItems.Clear();
        }

        internal override void NotifyReset() => 
            OnCollectionChanged(GetItems(Map), new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    }
}

