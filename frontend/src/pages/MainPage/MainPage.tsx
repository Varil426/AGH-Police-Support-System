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
import { MainPageModel } from "./MainPageModel";
import { IncidentStatsPanelViewModel } from "../../components/incidentStatsPanel/IncidentStatsPanelViewModel";
import { IncidentStatsPanelModel } from "../../components/incidentStatsPanel/IncidentStatsPanelModel";

export interface MainPageProps {
  model: MainPageModel;
}

export const MainPage = observer(({ model }: MainPageProps) => {
  const incidentStore = useRootStore().incidentStore;
  const patrolStore = useRootStore().patrolStore;
  const hub = new MonitoringHubClient(incidentStore, patrolStore);

  useEffect(() => {
    hub.start();
  });

  return (
    <Route path="/">
      <Flex>
        <MapViewModel model={new MapModel(hub, model.selectedPatrols)} />
        <Flex vertical>
          <IncidentStatsPanelViewModel model={new IncidentStatsPanelModel()} />
          <PatrolListViewModel
            model={new PatrolListModel(model.selectedPatrols)}
          />
        </Flex>
      </Flex>
    </Route>
  );
});
