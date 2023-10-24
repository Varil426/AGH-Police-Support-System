import { computed, makeObservable } from "mobx";
import { IncidentStore } from "../../stores/IncidentStore";
import { useRootStore } from "../../utils/RootStoreProvider";
import { IncidentStatusEnum } from "../../api/generated/Shared/CommonTypes/Incident/IncidentStatusEnum";

export class IncidentStatsPanelModel {
  private readonly incidentStore: IncidentStore = useRootStore().incidentStore;

  constructor() {
    makeObservable(this);
  }

  @computed get incidents() {
    return this.incidentStore.incidents;
  }

  @computed get incidentsWaitingForResponse() {
    return this.incidents.filter(
      (x) => x.status === IncidentStatusEnum.WaitingForResponse
    );
  }

  @computed get incidentsUnderInvestigation() {
    return this.incidents.filter(
      (x) =>
        x.status === IncidentStatusEnum.OnGoingNormal ||
        x.status === IncidentStatusEnum.AwaitingPatrolArrival
    );
  }

  @computed get activeShootings() {
    return this.incidents.filter(
      (x) =>
        x.status === IncidentStatusEnum.OnGoingShooting ||
        x.status === IncidentStatusEnum.AwaitingBackup
    );
  }
}
