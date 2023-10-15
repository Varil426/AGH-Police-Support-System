import { Marker } from "react-leaflet";
import { Incident } from "../../../models/Incident";
import { observer } from "mobx-react-lite";
import { DivIcon } from "leaflet";
import { useEffect, useState } from "react";

export interface IncidentMarkerProps {
  incident: Incident;
}

const icon = new DivIcon({ html: "❗" });

export const IncidentMarker = observer(({ incident }: IncidentMarkerProps) => {
  // TODO Workaround for now
  const [, setTime] = useState(Date.now());

  useEffect(() => {
    const interval = setInterval(() => setTime(Date.now()), 1000 / 60);
    return () => {
      clearInterval(interval);
    };
  }, []);
  //

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
