using System.Reflection;
using System.Runtime.InteropServices;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.UWP;
using Xamarin.Forms.Platform.UWP;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Xamarin.Forms.GoogleMaps.WindowsUniversal")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("1.8.0.4")]
[assembly: AssemblyFileVersion("1.8.0.4")]
[assembly: ComVisible(false)]
[assembly: ExportRenderer(typeof(Map), typeof(MapRenderer))]
