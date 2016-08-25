using Xamarin.Forms;

namespace XFGoogleMapSample
{
	public partial class BindingPinView : StackLayout
	{
		private string _display;

		public BindingPinView(string display)
		{
			InitializeComponent();
			_display = display;
			BindingContext = this;
		}

		public string Display
		{
			get { return _display; }
		}
	}
}

