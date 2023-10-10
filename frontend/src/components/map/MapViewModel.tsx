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
  // TODO Fix using https://stackoverflow.com/questions/62182987/is-the-react-way-really-to-re-render-the-whole-react-leaflet-component-regular
  // Maybe there's no need to fix anything. It loads just once, and then updates, when markers are updated.

  return (
    <MapContainer
      center={[model.center.latitude, model.center.longitude]}
      zoom={13}
      scrollWheelZoom={false}
      style={{ height: 500, width: 500 }}
      key={mapKey}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {model.incidents.map((x) => (
        <IncidentMarker incident={x} />
      ))}
      {model.patrols.map((x) => (
        <PatrolMarker patrol={x} />
      ))}
    </MapContainer>
  );
});
