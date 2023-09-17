import { Route } from "wouter";
import { MapViewModel } from "../components/map/MapViewModel";
import { MapModel } from "../components/map/MapModel";
import { MonitoringHubClient } from "../api/MonitoringHubClient";
import { useRootStore } from "../utils/RootStoreProvider";
import { useEffect, useState } from "react";

export const MapPage = () => {
  const incidentStore = useRootStore().incidentStore;
  const [hub] = useState(new MonitoringHubClient(incidentStore));

  useEffect(() => {
    hub.start();
  });

  return (
    <Route path="/map">
      <MapViewModel model={new MapModel(hub)} />
    </Route>
  );
};
