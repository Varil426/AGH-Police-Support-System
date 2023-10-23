import { Route } from "wouter";
import { MapViewModel } from "../../components/map/MapViewModel";
import { MapModel } from "../../components/map/MapModel";
import { MonitoringHubClient } from "../../api/MonitoringHubClient";
import { useRootStore } from "../../utils/RootStoreProvider";
import { useEffect } from "react";
import { observer } from "mobx-react-lite";
import { Flex } from "antd";
import { PatrolListViewModel } from "../../components/patrolList/PatrolListViewModel";
import { PatrolListModel } from "../../components/patrolList/PatrolListModel";
import { MapPageModel } from "./MapPageModel";

export interface MapPageProps {
  model: MapPageModel;
}

export const MapPage = observer(({ model }: MapPageProps) => {
  const incidentStore = useRootStore().incidentStore;
  const patrolStore = useRootStore().patrolStore;
  const hub = new MonitoringHubClient(incidentStore, patrolStore);

  useEffect(() => {
    hub.start();
  });

  return (
    <Route path="/map">
      <Flex>
        <MapViewModel model={new MapModel(hub, model.selectedPatrols)} />
        <PatrolListViewModel
          model={new PatrolListModel(model.selectedPatrols)}
        />
      </Flex>
    </Route>
  );
});
