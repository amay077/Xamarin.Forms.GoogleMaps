#!/usr/bin/env bash

echo "Inject Google Maps API Keys"

ls ./

perl -pi -e "s/your_google_maps_ios_sdk_api_key/$GOOGLEMAPS_IOS_API_KEY/g" ./XFGoogleMapSample/Variables_sample.cs
