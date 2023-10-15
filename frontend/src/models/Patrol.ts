import { makeObservable, observable } from "mobx";
import { IPosition } from "../api/generated/Shared/CommonTypes/Geo/IPosition";

export class Patrol {
  readonly id: string;
  readonly patrolId: string;
  @observable position: IPosition;

  constructor(id: string, patrolId: string, position: IPosition) {
    this.id = id;
    this.patrolId = patrolId;
    this.position = position;

    makeObservable(this);
  }
}
