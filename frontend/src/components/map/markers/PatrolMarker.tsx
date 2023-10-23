import { DivIcon, DivIconOptions } from "leaflet";
import { Patrol } from "../../../models/Patrol";
import { observer } from "mobx-react-lite";
import { Marker } from "react-leaflet";

export interface PatrolMarkerProps {
  patrol: Patrol;
  isSelected?: boolean;
}

export const PatrolMarker = observer(
  ({ patrol, isSelected = false }: PatrolMarkerProps) => {
    return (
      <Marker
        position={{
          lat: patrol.position.latitude,
          lng: patrol.position.longitude,
        }}
        icon={
          new DivIcon({
            html: "🚓",
            className: `marker-icon ${isSelected ? "selected" : ""}`,
          })
        }
      ></Marker>
    );
  }
);
