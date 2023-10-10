import { HubConnectionBuilder } from "@microsoft/signalr";
import { ICityStateMessageDto } from "./generated/WebApp/API/Hubs/MonitoringHub/ICityStateMessageDto";
import { IMonitoringHubClient } from "./generated/WebApp/API/Hubs/MonitoringHub/IMonitoringHubClient";
import { IncidentStore } from "../stores/IncidentStore";
import { Routes } from "./ApiRoutes";
import { toast } from "react-toastify";
import { IPosition } from "./generated/Shared/CommonTypes/Geo/IPosition";
import { action, computed, makeObservable, observable } from "mobx";
import { PatrolStore } from "../stores/PatrolStore";

// export type UpdateAction = (cityStateMessage: ICityStateMessage) => void;

export class MonitoringHubClient implements IMonitoringHubClient {
  private readonly connection;
  //   private readonly updateActions: UpdateAction[] = [];

  @observable private _hqLocation: IPosition | undefined;

  constructor(
    private readonly incidentStore: IncidentStore,
    private readonly patrolStore: PatrolStore
  ) {
    this.connection = new HubConnectionBuilder()
      .withUrl(Routes.MONITORING_HUB)
      .withAutomaticReconnect()
      .configureLogging("Debug")
      .build();

    this.connection.on(
      this.receiveUpdate.name.toLowerCase(),
      (cityStateMessage: ICityStateMessageDto) =>
        this.receiveUpdate(cityStateMessage)
    );

    makeObservable(this);
  }

  @action receiveUpdate(cityStateMessage: ICityStateMessageDto): void {
    if (!this._hqLocation) {
      this._hqLocation = cityStateMessage.hqLocation;
    }

    cityStateMessage.incidents.map((x) =>
      this.incidentStore.updateOrCreateIncident(x)
    );

    cityStateMessage.patrols.map((x) =>
      this.patrolStore.updateOrCreatePatrol(x)
    );
  }

  @computed get hqLocation(): IPosition | undefined {
    return this._hqLocation;
  }

  async start() {
    if (
      this.connection.state === "Connected" ||
      this.connection.state === "Connecting"
    )
      return;

    try {
      await this.connection.start();
      console.log("SignalR Connected.");
    } catch (err) {
      console.log(err);
      toast("Hub Connection Error");
    }
  }

  //   OnUpdate(action: UpdateAction) {
  //     this.updateActions.push(action);
  //   }
}
