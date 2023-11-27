import { MapContainer, TileLayer } from "react-leaflet";
import { MapModel } from "./MapModel";
import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { v4 as uuidv4 } from "uuid";
import { IncidentMarker } from "./markers/IncidentMarker";
import { PatrolMarker } from "./markers/PatrolMarker";

export interface IMapViewModelProps {
  model: MapModel;
}

export const MapViewModel = observer(({ model }: IMapViewModelProps) => {
  const [mapKey, setMapKey] = useState(uuidv4());
  useEffect(() => setMapKey(uuidv4()), [model.center]);

  return (
    <MapContainer
      center={[model.center.latitude, model.center.longitude]}
      zoom={13}
      scrollWheelZoom={false}
      style={{ height: 800, width: 800 }}
      key={mapKey}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {model.incidents.map((x) => (
        <IncidentMarker incident={x} key={x.id} />
      ))}
      {model.patrols.map((x) => (
        <PatrolMarker
          patrol={x}
          key={x.id}
          isSelected={model.selectedPatrols.includes(x)}
        />
      ))}
    </MapContainer>
  );
});
