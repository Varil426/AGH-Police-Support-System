import { makeObservable, observable } from "mobx";
import type { IPosition } from "../api/generated/Shared/CommonTypes/Geo/IPosition";
import { IncidentTypeEnum } from "../api/generated/Shared/CommonTypes/Incident/IncidentTypeEnum";
import { IncidentStatusEnum } from "../api/generated/Shared/CommonTypes/Incident/IncidentStatusEnum";

export class Incident {
  readonly id: string;
  @observable location: IPosition;
  @observable type: IncidentTypeEnum;
  @observable status: IncidentStatusEnum;

  constructor(
    id: string,
    position: IPosition,
    type: IncidentTypeEnum,
    status: IncidentStatusEnum
  ) {
    this.id = id;
    this.location = position;
    this.type = type;
    this.status = status;

    makeObservable(this);
  }
}
