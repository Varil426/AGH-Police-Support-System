import { action, computed, makeObservable, observable } from "mobx";

export class CounterModel {
  @observable count: number = 0;

  constructor() {
    makeObservable(this);
  }

  @computed get getCounter() {
    console.log("getCounter TEST");
    return this.count;
  }

  @action increaseCounter() {
    this.count++;
  }
}
