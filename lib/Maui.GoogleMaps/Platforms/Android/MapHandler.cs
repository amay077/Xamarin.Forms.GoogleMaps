using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Maui.GoogleMaps.Android;
using Maui.GoogleMaps.Android.Callbacks;
using Maui.GoogleMaps.Android.Extensions;
using Maui.GoogleMaps.Internals;
using Maui.GoogleMaps.Logics;
using Maui.GoogleMaps.Logics.Android;
using Maui.GoogleMaps.Platforms.Android.Listeners;
using Math = System.Math;

namespace Maui.GoogleMaps.Handlers;

public partial class MapHandler
{
    private readonly OnMapClickListener onMapClickListener = new();
    private readonly OnMapLongClickListener onMapLongClickListener = new();
    private readonly OnMyLocationButtonClickListener onMyLocationButtonClickListener = new();

    private CameraLogic _cameraLogic;
    private UiSettingsLogic _uiSettingsLogic = new();

    internal float ScaledDensity;

    internal IList<BaseLogic<GoogleMap>> Logics;

    static Bundle s_bundle;
    internal static Bundle Bundle { set { s_bundle = value; } }

    protected internal static PlatformConfig Config { protected get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    protected virtual GoogleMap NativeMap { get; private set; }

    // ReSharper disable once MemberCanBePrivate.Global
    protected Map Map => VirtualView;

    private bool _ready = false;
    private bool _onLayout = false;

    bool _disposed;

    public override void PlatformArrange(Microsoft.Maui.Graphics.Rect frame)
    {
        base.PlatformArrange(frame);

        _onLayout = true;

        if (_ready && _onLayout)
        {
            InitializeLogic();
        }
        else if (NativeMap != null)
        {
            UpdateVisibleRegion(NativeMap.CameraPosition.Target);
        }
    }

    protected override MapView CreatePlatformView()
    {
        var mapView = new MapView(Context);
        mapView.OnCreate(s_bundle);
        mapView.OnResume();

        return mapView;
    }

    protected override async void ConnectHandler(MapView platformView)
    {
        _cameraLogic = new CameraLogic(UpdateVisibleRegion);

        Logics = new List<BaseLogic<GoogleMap>>
        {
            new PolylineLogic(),
            new PolygonLogic(),
            new CircleLogic(),
            new PinLogic(Config.GetBitmapdescriptionFactory(), OnMarkerCreating, OnMarkerCreated, OnMarkerDeleting, OnMarkerDeleted),
            new TileLayerLogic(),
            new GroundOverlayLogic(Config.GetBitmapdescriptionFactory())
        };

        var activity = Platform.CurrentActivity;

        if (activity != null)
        {
            ScaledDensity = activity.GetScaledDensity();
            _cameraLogic.ScaledDensity = ScaledDensity;
            foreach (var logic in Logics)
            {
                logic.ScaledDensity = ScaledDensity;
            }
        }

        NativeMap = await platformView.GetGoogleMapAsync();

        foreach (var logic in Logics)
        {
            logic.Register(null, null, NativeMap, Map, this);
            logic.ScaledDensity = ScaledDensity;
        }

        OnMapReady();

        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(MapView platformView)
    {
        if (!_disposed)
        {
            _disposed = true;
            Uninitialize(NativeMap, Map);

            if (NativeMap != null)
            {
                NativeMap.Dispose();
                NativeMap = null;
            }

            platformView?.OnDestroy();
        }

        base.DisconnectHandler(platformView);
    }

    protected virtual void OnMapReady()
    {
        var nativeMap = NativeMap;

        if (nativeMap != null)
        {
            _cameraLogic.Register(Map, nativeMap);

            _uiSettingsLogic.Register(Map, nativeMap);
            Map.OnSnapshot += OnSnapshot;

            Map.OnFromScreenLocation = point =>
            {
                var latLng = nativeMap.Projection.FromScreenLocation(new global::Android.Graphics.Point((int)point.X, (int)point.Y));
                return latLng.ToPosition();
            };

            Map.OnToScreenLocation = position =>
            {
                var pt = nativeMap.Projection.ToScreenLocation(position.ToLatLng());
                return new Microsoft.Maui.Graphics.Point(pt.X, pt.Y);
            };

            onMapClickListener.MapHandler = this;
            onMapLongClickListener.MapHandler = this;
            onMyLocationButtonClickListener.MapHandler = this;

            nativeMap.SetOnMapClickListener(onMapClickListener);
            nativeMap.SetOnMapLongClickListener(onMapLongClickListener);
            nativeMap.SetOnMyLocationButtonClickListener(onMyLocationButtonClickListener);

            _uiSettingsLogic.Initialize();

            MapMapper.UpdateProperties(this, VirtualView);
        }

        _ready = true;
        if (_ready && _onLayout)
        {
            InitializeLogic();
        }
    }
    private void OnSnapshot(TakeSnapshotMessage snapshotMessage)
    {
        NativeMap.Snapshot(new DelegateSnapshotReadyCallback(snapshot =>
        {
            var stream = new MemoryStream();
            snapshot.Compress(Bitmap.CompressFormat.Png, 0, stream);
            stream.Position = 0;
            snapshotMessage?.OnSnapshot?.Invoke(stream);
        }));
    }

    private void InitializeLogic()
    {
        _cameraLogic.MoveCamera(Map.InitialCameraUpdate);

        foreach (var logic in Logics)
        {
            if (logic.Map != null)
            {
                logic.RestoreItems();
                logic.OnMapPropertyChanged(Map.SelectedPinProperty.PropertyName);
            }
        }

        _ready = false;
        _onLayout = false;
    }

    public static void MapMapType(MapHandler handler, Map map)
    {
        var nativeMap = handler.NativeMap;
        if (nativeMap == null)
        {
            return;
        }

        nativeMap.MapType = map.MapType switch
        {
            MapType.Street => GoogleMap.MapTypeNormal,
            MapType.Satellite => GoogleMap.MapTypeSatellite,
            MapType.Hybrid => GoogleMap.MapTypeHybrid,
            MapType.Terrain => GoogleMap.MapTypeTerrain,
            MapType.None => GoogleMap.MapTypeNone,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static void MapPadding(MapHandler handler, Map map)
    {
        var density = handler.ScaledDensity;
        handler.NativeMap?.SetPadding(
            (int)(map.Padding.Left * density),
            (int)(map.Padding.Top * density),
            (int)(map.Padding.Right * density),
            (int)(map.Padding.Bottom * density));
    }

    public static void MapIsTrafficEnabled(MapHandler handler, Map map)
    {
        if (handler.NativeMap != null)
        {
            handler.NativeMap.TrafficEnabled = map.IsTrafficEnabled;
        }    
    }

    public static void MapIsIndoorEnabled(MapHandler handler, Map map)
    {
        handler.NativeMap?.SetIndoorEnabled(map.IsIndoorEnabled);
    }

    public static void MapMyLocationEnabled(MapHandler handler, Map map)
    {
        if (handler.NativeMap != null)
        {
            handler.NativeMap.MyLocationEnabled = map.MyLocationEnabled;
        }
    }
    public static void MapMapStyle(MapHandler handler, Map map)
    {
        handler.NativeMap?.SetMapStyle(map.MapStyle != null ?
            new MapStyleOptions(map.MapStyle.JsonStyle) :
            null);
    }

    public static void MapSelectedPin(MapHandler handler, Map map)
    {
        if (handler.NativeMap != null)
        {
            foreach (var logic in handler.Logics)
            {
                logic.OnMapPropertyChanged(Map.SelectedPinProperty.PropertyName);
            }
        }
    }

    private void Uninitialize(GoogleMap nativeMap, Map map)
    {
        try
        {
            if (nativeMap == null)
            {
                System.Diagnostics.Debug.WriteLine($"Uninitialize failed - {nameof(nativeMap)} is null");
                return;
            }

            if (map == null)
            {
                System.Diagnostics.Debug.WriteLine($"Uninitialize failed - {nameof(map)} is null");
                return;
            }

            _uiSettingsLogic.Unregister();

           Map.OnSnapshot -= OnSnapshot;
            _cameraLogic.Unregister();

            foreach (var logic in Logics)
            {
                logic.Unregister(nativeMap, map);
            }

            nativeMap.SetOnMapClickListener(null);
            nativeMap.SetOnMapLongClickListener(null);
            nativeMap.SetOnMyLocationButtonClickListener(null);

            nativeMap.MyLocationEnabled = false;
            nativeMap.Dispose();
        }
        catch (Exception ex)
        {
            var message = ex.Message;
            System.Diagnostics.Debug.WriteLine($"Uninitialize failed. - {message}");
        }
    }

    #region Overridable Members

    /// <summary>
    /// Call when before marker create.
    /// You can override your custom renderer for customize marker.
    /// </summary>
    /// <param name="outerItem">the pin.</param>
    /// <param name="innerItem">the marker options.</param>
    protected virtual void OnMarkerCreating(Pin outerItem, MarkerOptions innerItem)
    {
    }

    /// <summary>
    /// Call when after marker create.
    /// You can override your custom renderer for customize marker.
    /// </summary>
    /// <param name="outerItem">the pin.</param>
    /// <param name="innerItem">thr marker.</param>
    protected virtual void OnMarkerCreated(Pin outerItem, Marker innerItem)
    {
    }

    /// <summary>
    /// Call when before marker delete.
    /// You can override your custom renderer for customize marker.
    /// </summary>
    /// <param name="outerItem">the pin.</param>
    /// <param name="innerItem">thr marker.</param>
    protected virtual void OnMarkerDeleting(Pin outerItem, Marker innerItem)
    {
    }

    /// <summary>
    /// Call when after marker delete.
    /// You can override your custom renderer for customize marker.
    /// </summary>
    /// <param name="outerItem">the pin.</param>
    /// <param name="innerItem">thr marker.</param>
    protected virtual void OnMarkerDeleted(Pin outerItem, Marker innerItem)
    {
    }

    #endregion

    void UpdateVisibleRegion(LatLng pos)
    {
        var map = NativeMap;
        if (map == null)
            return;
        var projection = map.Projection;
        var width = PlatformView.Width;
        var height = PlatformView.Height;
        var ul = projection.FromScreenLocation(new global::Android.Graphics.Point(0, 0));
        var ur = projection.FromScreenLocation(new global::Android.Graphics.Point(width, 0));
        var ll = projection.FromScreenLocation(new global::Android.Graphics.Point(0, height));
        var lr = projection.FromScreenLocation(new global::Android.Graphics.Point(width, height));
        var dlat = Math.Max(Math.Abs(ul.Latitude - lr.Latitude), Math.Abs(ur.Latitude - ll.Latitude));
        var dlong = Math.Max(Math.Abs(ul.Longitude - lr.Longitude), Math.Abs(ur.Longitude - ll.Longitude));
#pragma warning disable 618
        VirtualView.VisibleRegion = new MapSpan(
                new Position(
                    pos.Latitude,
                    pos.Longitude
                ),
            dlat,
            dlong
        );
#pragma warning restore 618
        VirtualView.Region = projection.VisibleRegion.ToRegion();
    }
}