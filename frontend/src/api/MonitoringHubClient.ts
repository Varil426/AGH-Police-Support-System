import { HubConnectionBuilder } from "@microsoft/signalr";
import { ICityStateMessage } from "./generated/WebApp/API/Hubs/MonitoringHub/ICityStateMessage";
import { IMonitoringHubClient } from "./generated/WebApp/API/Hubs/MonitoringHub/IMonitoringHubClient";
import { IncidentStore } from "../stores/IncidentStore";
import { Routes } from "./ApiRoutes";
import { toast } from "react-toastify";

// export type UpdateAction = (cityStateMessage: ICityStateMessage) => void;

export class MonitoringHubClient implements IMonitoringHubClient {
  private readonly connection;
  //   private readonly updateActions: UpdateAction[] = [];

  constructor(private readonly incidentStore: IncidentStore) {
    this.connection = new HubConnectionBuilder()
      .withUrl(Routes.MONITORING_HUB)
      .withAutomaticReconnect()
      .configureLogging("Debug")
      .build();

    this.connection.on(
      this.ReceiveUpdate.name.toLowerCase(),
      this.ReceiveUpdate
    );
  }

  ReceiveUpdate(cityStateMessage: ICityStateMessage): void {
    console.log(cityStateMessage); // TODO
    console.log("TEST");
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
