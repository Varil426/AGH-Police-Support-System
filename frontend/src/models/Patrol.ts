import { makeObservable, observable } from "mobx";
import { IPosition } from "../api/generated/Shared/CommonTypes/Geo/IPosition";
import { PatrolStatusEnum } from "../api/generated/Shared/CommonTypes/Patrol/PatrolStatusEnum";

export class Patrol {
  readonly id: string;
  readonly patrolId: string;
  @observable position: IPosition;
  @observable status: PatrolStatusEnum;

  constructor(
    id: string,
    patrolId: string,
    position: IPosition,
    status: PatrolStatusEnum
  ) {
    this.id = id;
    this.patrolId = patrolId;
    this.position = position;
    this.status = status;

    makeObservable(this);
  }
}
