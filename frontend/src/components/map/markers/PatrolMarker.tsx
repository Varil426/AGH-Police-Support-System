import { DivIcon } from "leaflet";
import { Patrol } from "../../../models/Patrol";
import { observer } from "mobx-react-lite";
import { Marker } from "react-leaflet";
import { useEffect, useState } from "react";

export interface PatrolMarkerProps {
  patrol: Patrol;
}

const icon = new DivIcon({ html: "👮" });

export const PatrolMarker = observer(({ patrol }: PatrolMarkerProps) => {
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
        lat: patrol.position.latitude,
        lng: patrol.position.longitude,
      }}
      icon={icon}
    ></Marker>
  );
});
