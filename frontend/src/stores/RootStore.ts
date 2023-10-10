import { IncidentStore } from "./IncidentStore";
import { PatrolStore } from "./PatrolStore";

export class RootStore {
  readonly incidentStore: IncidentStore;
  readonly patrolStore: PatrolStore;

  constructor(incidentStore: IncidentStore, patrolStore: PatrolStore) {
    this.incidentStore = incidentStore;
    this.patrolStore = patrolStore;
  }
}
