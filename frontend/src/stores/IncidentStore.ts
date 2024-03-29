import { action, makeObservable, observable } from "mobx";
import { Incident } from "../models/Incident";
import { IIncidentDto } from "../api/generated/Shared/Application/Integration/DTOs/IIncidentDto";

export class IncidentStore {
  @observable readonly incidents: Incident[] = [];

  constructor() {
    makeObservable(this);
  }

  @action
  addIncident(dto: IIncidentDto) {
    if (this.incidents.find((x) => x.id === dto.id)) return;
    this.incidents.push(dto);
  }

  @action
  removeIncident(id: string) {
    const f = this.incidents.find((x) => x.id === id);
    if (f) {
      const index = this.incidents.indexOf(f);
      this.incidents.splice(index, 1);
    }
  }

  @action
  updateIncident(dto: IIncidentDto) {
    const u = this.incidents.find((x) => x.id === dto.id);
    if (!u) return;
    u.location = dto.location;
    u.status = dto.status;
    u.type = dto.type;
  }

  @action
  updateOrCreateIncident(dto: IIncidentDto) {
    if (this.incidents.some((x) => x.id === dto.id))
      return this.updateIncident(dto);
    this.addIncident(dto);
  }

  @action
  removeNotPresent(ids: string[]) {
    const toBeDeleted = this.incidents.filter(
      (x) => !ids.find((e) => e === x.id)
    );
    toBeDeleted.forEach((x) => this.removeIncident(x.id));
  }
}
