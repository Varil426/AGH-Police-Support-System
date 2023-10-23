import { action, computed, makeObservable } from "mobx";
import { useRootStore } from "../../utils/RootStoreProvider";
import { PatrolStore } from "../../stores/PatrolStore";
import { Patrol } from "../../models/Patrol";

export class PatrolListModel {
  private readonly patrolStore: PatrolStore = useRootStore().patrolStore;
  constructor(readonly selectedPatrols: Patrol[]) {
    makeObservable(this);
  }

  @computed get patrols() {
    return this.patrolStore.patrols;
  }

  @action togglePatrolSelection(patrol: Patrol) {
    this.selectedPatrols.includes(patrol)
      ? this.selectedPatrols.splice(this.selectedPatrols.indexOf(patrol), 1)
      : this.selectedPatrols.push(patrol);
  }
}
