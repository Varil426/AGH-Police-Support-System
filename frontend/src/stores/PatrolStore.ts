import { action, observable } from "mobx";
import { Patrol } from "../models/Patrol";
import { IPatrolDto } from "../api/generated/Shared/Application/Integration/DTOs/IPatrolDto";

export class PatrolStore {
  @observable readonly patrols: Patrol[] = [];

  @action
  addPatrol(dto: IPatrolDto) {
    if (this.patrols.find((x) => x.id === dto.id)) return;
    this.patrols.push(dto);
  }

  @action
  removePatrol(id: string) {
    const f = this.patrols.find((x) => x.id === id);
    if (f) {
      const index = this.patrols.indexOf(f);
      this.patrols.splice(index, 1);
    }
  }

  @action
  updatePatrol(dto: IPatrolDto) {
    const u = this.patrols.find((x) => x.id === dto.id);
    if (!u) return;
    u.position = dto.position;
  }

  @action
  updateOrCreatePatrol(dto: IPatrolDto) {
    if (this.patrols.some((x) => x.id === dto.id))
      return this.updatePatrol(dto);
    this.addPatrol(dto);
  }
}