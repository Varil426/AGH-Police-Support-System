import { IncidentStore } from "./IncidentStore";

export class RootStore {
  readonly incidentStore: IncidentStore;

  constructor(incidentStore: IncidentStore) {
    this.incidentStore = incidentStore;
  }
}
