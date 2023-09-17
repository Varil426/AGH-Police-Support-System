import { observable } from "mobx";
import { Incident } from "../models/Incident";
import { IIncidentDto } from "../api/generated/Shared/Application/DTOs/IIncidentDto";

export class IncidentStore {
  @observable readonly incidents: Incident[] = [];

  addIncident(dto: IIncidentDto) {
    if (this.incidents.find((x) => x.id === dto.id)) return;

    this.incidents.push();
  }

  removeIncident(id: string) {
    const f = this.incidents.find((x) => x.id === id);
    if (f) {
      const index = this.incidents.indexOf(f);
      this.incidents.splice(index, 1);
    }
  }
}
