#!/bin/bash

set -m

pushd /App/NavigationService
PoliceSupportSystem_ServiceSettings__Id="NavigationService-${PoliceSupportSystem_PatrolSettings__NavAgentId}" \
    dotnet ./NavigationService.Service.dll --environment=Docker &
popd 

pushd /App/GunService
PoliceSupportSystem_ServiceSettings__Id="GunService-${PoliceSupportSystem_PatrolSettings__GunAgentId}" \
    dotnet ./GunService.Service.dll --environment=Docker &
popd

sleep 4

pushd /App/PatrolService
PoliceSupportSystem_ServiceSettings__Id="PatrolService-${PoliceSupportSystem_PatrolSettings__PatrolAgentId}" \
    dotnet ./PatrolService.Service.dll --environment=Docker &
popd

fg %1