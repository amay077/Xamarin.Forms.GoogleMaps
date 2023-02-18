using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android;
using Android.App;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Android;
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Xamarin.Forms.GoogleMaps.Android")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// Add some common permissions, these can be removed if not needed

[assembly: UsesPermission(Manifest.Permission.Internet)]
[assembly: ExportRenderer(typeof(Map), typeof(MapRenderer))]
[assembly: Preserve]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
[assembly: AssemblyCompany(ProductInformation.Author)]
[assembly: AssemblyProduct(ProductInformation.Name)]
[assembly: AssemblyCopyright(ProductInformation.Copyright)]
[assembly: AssemblyTrademark(ProductInformation.Trademark)]
[assembly: AssemblyVersion(ProductInformation.Version)]
[assembly: AssemblyFileVersion(ProductInformation.Version)]