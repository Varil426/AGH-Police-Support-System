import { computed, makeObservable } from "mobx";
import { MonitoringHubClient } from "../../api/MonitoringHubClient";
import { IncidentStore } from "../../stores/IncidentStore";
import { useRootStore } from "../../utils/RootStoreProvider";
import { PatrolStore } from "../../stores/PatrolStore";

export class MapModel {
  private readonly incidentStore: IncidentStore = useRootStore().incidentStore;
  private readonly patrolStore: PatrolStore = useRootStore().patrolStore;

  constructor(private readonly hubClient: MonitoringHubClient) {
    makeObservable(this);
  }

  @computed get center() {
    console.log(this.hubClient.hqLocation);
    return this.hubClient.hqLocation ?? { latitude: 0, longitude: 0 };
  }

  @computed get incidents() {
    return this.incidentStore.incidents;
  }

  @computed get patrols() {
    return this.patrolStore.patrols;
  }
}
