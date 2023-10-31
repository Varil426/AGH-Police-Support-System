import { Marker } from "react-leaflet";
import { Incident } from "../../../models/Incident";
import { observer } from "mobx-react-lite";
import { DivIcon } from "leaflet";
import { IncidentStatusEnum } from "../../../api/generated/Shared/CommonTypes/Incident/IncidentStatusEnum";

export interface IncidentMarkerProps {
  incident: Incident;
}

const className = "marker-icon bg-black bg-opacity-50";

const incidentIcon = new DivIcon({
  html: "❕",
  className: className,
});

const shootingIcon = new DivIcon({
  html: "❗",
  className: className,
});

export const IncidentMarker = observer(({ incident }: IncidentMarkerProps) => {
  return (
    <Marker
      position={{
        lat: incident.location.latitude,
        lng: incident.location.longitude,
      }}
      icon={
        incident.status === IncidentStatusEnum.OnGoingShooting
          ? shootingIcon
          : incidentIcon
      }
    ></Marker>
  );
});
