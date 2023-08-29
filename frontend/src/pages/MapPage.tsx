import { Route } from "wouter";
import { MapView } from "../components/map/MapView";

export const MapPage = () => {
  return (
    <Route path="/map">
      <MapView />
    </Route>
  );
};
