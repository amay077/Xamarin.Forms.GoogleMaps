// Original code from https://github.com/javiholcman/Wapps.Forms.Map/

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.GoogleMaps.Android;
using System.Threading;
using System.Threading.Tasks;
using Android.Widget;

[assembly: ExportRenderer(typeof(Xamarin.Forms.GoogleMaps.MapAuxiliar), typeof(Xamarin.Forms.GoogleMaps.MapAuxiliarRenderer))]

namespace Xamarin.Forms.GoogleMaps
{
    public class MapAuxiliarRenderer : VisualElementRenderer<StackLayout>
    {
        public static MapAuxiliarRenderer LiveMapRenderer { get; set; }

        private static Mutex mut = new Mutex();

        public MapAuxiliarRenderer()
        {
            LiveMapRenderer = this;
        }

        public FrameLayout GetNativeView(View element)
        {
            mut.WaitOne();
            Element.Children.Add(element);

            global::Android.Views.View targetView = null;

            for (int i = 0; i < ViewGroup.ChildCount; i++)
            {
                var view = ViewGroup.GetChildAt(i);

                var property = view.GetType().GetProperty("Element");
                if (property != null)
                {
                    var elem = property.GetValue(view);
                    if (elem == element)
                    {
                        targetView = view;
                        break;
                    }
                }
            }

            if (targetView == null)
            {
                return null;
            }

            ((global::Android.Views.ViewGroup)targetView.Parent).RemoveView(targetView);

            var container = new FrameLayout(Context);
            container.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            targetView.LayoutParameters = new FrameLayout.LayoutParams(Utils.DpToPx((float)element.WidthRequest), Utils.DpToPx((float)element.HeightRequest));
            container.AddView(targetView);

            mut.ReleaseMutex();

            return container;
        }
    }
}

