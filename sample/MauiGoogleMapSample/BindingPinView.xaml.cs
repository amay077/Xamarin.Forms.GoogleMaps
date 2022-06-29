
namespace MauiGoogleMapSample
{
    public partial class BindingPinView : Grid
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

