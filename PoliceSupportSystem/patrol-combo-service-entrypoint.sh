#!/bin/bash

set -m

pushd /App/NavigationService
dotnet ./NavigationService.Service.dll --environment=Docker &
popd 

sleep 4

pushd /App/PatrolService
dotnet ./PatrolService.Service.dll --environment=Docker &
popd

pushd /App/GunService
# dotnet ./GunService.Service.dll --environment=Docker &
popd

fg %1