import { List } from "antd";
import { observer } from "mobx-react-lite";
import { PatrolListModel } from "./PatrolListModel";
import { PatrolListTile } from "./PatrolListTile";

export interface PatrolListViewModelProps {
  model: PatrolListModel;
}

export const PatrolListViewModel = observer(
  ({ model }: PatrolListViewModelProps) => {
    return (
      <List itemLayout="vertical" className="w-75 m-4">
        {model.patrols
          .slice()
          .sort((a, b) => (a.patrolId < b.patrolId ? -1 : 1))
          .map((x) => (
            <PatrolListTile
              patrol={x}
              onClick={() => model.togglePatrolSelection(x)}
              isSelected={model.selectedPatrols.includes(x)}
              key={x.id}
            />
          ))}
      </List>
    );
  }
);
