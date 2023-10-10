import { DivIcon } from "leaflet";
import { Patrol } from "../../../models/Patrol";
import { observer } from "mobx-react-lite";
import { Marker } from "react-leaflet";

export interface PatrolMarkerProps {
  patrol: Patrol;
}

const icon = new DivIcon({ html: "👮" });

export const PatrolMarker = observer(({ patrol }: PatrolMarkerProps) => {
  return (
    <Marker
      position={{
        lat: patrol.position.latitude,
        lng: patrol.position.longitude,
      }}
      icon={icon}
    ></Marker>
  );
});
