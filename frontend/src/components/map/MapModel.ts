import { computed, makeObservable } from "mobx";
import { MonitoringHubClient } from "../../api/MonitoringHubClient";
import { IncidentStore } from "../../stores/IncidentStore";
import { useRootStore } from "../../utils/RootStoreProvider";

export class MapModel {
  private readonly incidentStore: IncidentStore = useRootStore().incidentStore;

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
}
