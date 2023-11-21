import { DivIcon } from "leaflet";
import { Patrol } from "../../../models/Patrol";
import { observer } from "mobx-react-lite";
import { Marker } from "react-leaflet";
import { PatrolStatusEnum } from "../../../api/generated/Shared/CommonTypes/Patrol/PatrolStatusEnum";

export interface PatrolMarkerProps {
  patrol: Patrol;
  isSelected?: boolean;
}

export const PatrolMarker = observer(
  ({ patrol, isSelected = false }: PatrolMarkerProps) => {
    const bgColor = selectBackgroundColor(patrol.status, isSelected);

    return (
      <Marker
        position={{
          lat: patrol.position.latitude,
          lng: patrol.position.longitude,
        }}
        icon={
          new DivIcon({
            html: "",
            className: `marker-icon patrol ${bgColor} bg-opacity-75`,
          })
        }
      ></Marker>
    );
  }
);

const selectBackgroundColor = (
  state: PatrolStatusEnum,
  isSelected: boolean
): string => {
  if (isSelected) return "selected";

  switch (state) {
    case PatrolStatusEnum.Patrolling:
      return "bg-blue-400";
    case PatrolStatusEnum.ResolvingIncident:
      return "bg-green-400";
    case PatrolStatusEnum.InShooting:
      return "bg-red-400";
  }

  return "";
};
