import { Marker } from "react-leaflet";
import { Incident } from "../../../models/Incident";
import { observer } from "mobx-react-lite";
import { DivIcon } from "leaflet";

export interface IncidentMarkerProps {
  incident: Incident;
}

const icon = new DivIcon({ html: "❗" });

export const IncidentMarker = observer(({ incident }: IncidentMarkerProps) => {
  return (
    <Marker
      position={{
        lat: incident.location.latitude,
        lng: incident.location.longitude,
      }}
      icon={icon}
    ></Marker>
  );
});
