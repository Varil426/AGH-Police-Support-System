import { Route } from "wouter";
import { MapViewModel } from "../components/map/MapViewModel";
import { MapModel } from "../components/map/MapModel";
import { MonitoringHubClient } from "../api/MonitoringHubClient";
import { useRootStore } from "../utils/RootStoreProvider";
import { useEffect } from "react";
import { observer } from "mobx-react-lite";

export const MapPage = observer(() => {
  const incidentStore = useRootStore().incidentStore;
  const hub = new MonitoringHubClient(incidentStore);

  useEffect(() => {
    hub.start();
  });

  return (
    <Route path="/map">
      <MapViewModel model={new MapModel(hub)} />
    </Route>
  );
});
