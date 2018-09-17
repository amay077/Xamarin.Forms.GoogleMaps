#!/usr/bin/env bash

echo "Replace $(SolutionDir) to .. in .csproj"
perl -pi -e 's/\$\(SolutionDir\)/../g' ./XFGoogleMapSample.iOS.csproj
perl -pi -e 's/\$\(SolutionDir\)/../g' ../XFGoogleMapSample/XFGoogleMapSample.csproj

echo "Remove _dev from bundle id in .plist"
perl -pi -e 's/_dev//g' ./info.plist

echo "Copy Variables_sample.cs to Variables.cs"
cp ../XFGoogleMapSample/Variables_sample.cs ../XFGoogleMapSample/Variables.cs

echo "Inject Google Maps API Keys"
perl -pi -e "s/your_google_maps_ios_sdk_api_key/$GOOGLEMAPS_IOS_API_KEY/g" ../XFGoogleMapSample/Variables.cs
