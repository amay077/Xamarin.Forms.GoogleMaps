using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xamarin.Forms.GoogleMaps.Logics
{
    internal abstract class DefaultLogic<TOuter, TNative, TNativeMap> : BaseLogic<TNativeMap>
        where TOuter : BindableObject
        where TNative : class
        where TNativeMap : class
    {
        readonly IList<TOuter> _outerItems = new List<TOuter>(); // Only for ResetItems.

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
                outerItem.PropertyChanged += OnItemPropertyChanged;
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
                {
                    _outerItems.Remove(outerShape);
                    outerShape.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }

        protected override void ResetItems()
        {
            foreach (var outerShape in _outerItems)
                DeleteNativeItem(outerShape);

            _outerItems.Clear();
        }

        internal override void RestoreItems()
        {
            var items = GetItems(Map);

            // Delete native items if exists
            foreach (var item in items ?? new List<TOuter>())
            {
                try
                {
                    DeleteNativeItem(item);
                }
                catch (Exception)
                {
                    // TODO printf in DebugMode
                }
            }

            AddItems((IList)items);
        }

        internal override void NotifyReset() =>
            OnCollectionChanged(GetItems(Map), new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        protected virtual void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }
    }
}

