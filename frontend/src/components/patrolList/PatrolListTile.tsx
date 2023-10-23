import { observer } from "mobx-react-lite";
import { Patrol } from "../../models/Patrol";
import { Button, Flex, List, Space } from "antd";
import { PatrolStatusEnum } from "../../api/generated/Shared/CommonTypes/Patrol/PatrolStatusEnum";

export interface PatrolListTileProps {
  patrol: Patrol;
  onClick?: () => void;
  isSelected: boolean;
}

export const PatrolListTile = observer(
  ({
    patrol,
    onClick = undefined,
    isSelected = false,
  }: PatrolListTileProps) => {
    return (
      <List.Item style={{ padding: 0 }}>
        <Button
          onClick={onClick}
          className={`h-20 align-middle border-blue-400 border-2 p-2 m-0 ${
            isSelected ? "bg-blue-400" : ""
          }`}
        >
          <Flex>
            <Space className="text-6xl block w-25">🚓</Space>
            <Space className="block w-50 h-full">
              <span>Patrol ID: {patrol.patrolId}</span>
              <span>Status: {PatrolStatusEnum[patrol.status]}</span>
            </Space>
          </Flex>
        </Button>
      </List.Item>
    );
  }
);
