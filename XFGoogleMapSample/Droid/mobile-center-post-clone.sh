#!/usr/bin/env bash

echo "Inject Google Maps API Keys"
pwd

perl -pi -e 's/$(SolutionDir)/./g' ./XFGoogleMapSample.Droid.csproj
