import { makeObservable, observable } from "mobx";
import { Patrol } from "../../models/Patrol";

export class MainPageModel {
  @observable public readonly selectedPatrols: Patrol[] = [];

  constructor() {
    makeObservable(this);
  }
}
