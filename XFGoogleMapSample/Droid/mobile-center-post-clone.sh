#!/usr/bin/env bash

echo "Inject Google Maps API Keys"
pwd

perl -pi -e 's/$(SolutionDir)/../g' ./XFGoogleMapSample.Droid.csproj
perl -pi -e 's/$(SolutionDir)/../g' ../XFGoogleMapSample/XFGoogleMapSample.csproj
