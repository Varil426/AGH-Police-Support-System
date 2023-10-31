import { observer } from "mobx-react-lite";
import { IncidentStatsPanelModel } from "./IncidentStatsPanelModel";
import { Flex } from "antd";

export interface IncidentStatsPanelViewModelProps {
  model: IncidentStatsPanelModel;
}

export const IncidentStatsPanelViewModel = observer(
  ({ model }: IncidentStatsPanelViewModelProps) => {
    return (
      <Flex
        vertical
        className="w-50 h-50 align-middle justify-center p-4 border-blue-400 border-2"
      >
        <h1>Incident Stats</h1>
        <span>
          Incident without assignee: {model.incidentsWaitingForResponse.length}
        </span>
        <span>
          Incident under investigation:{" "}
          {model.incidentsUnderInvestigation.length}
        </span>
        <span>Active shootings: {model.activeShootings.length}</span>
      </Flex>
    );
  }
);
