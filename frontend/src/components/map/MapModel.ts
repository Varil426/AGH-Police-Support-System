import { computed, makeObservable } from "mobx";
import { MonitoringHubClient } from "../../api/MonitoringHubClient";

export class MapModel {
  constructor(private hubClient: MonitoringHubClient) {
    makeObservable(this);
  }

  @computed get center() {
    console.log(this.hubClient.hqLocation);
    return this.hubClient.hqLocation ?? { latitude: 0, longitude: 0 };
  }
}
