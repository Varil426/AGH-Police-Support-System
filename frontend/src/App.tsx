import "./App.css";
import "leaflet/dist/leaflet.css";
import { RootStorePrivider } from "./utils/RootStoreProvider";
import RootStore from "./stores/RootStore";
import { Router } from "wouter";
import { MapPage } from "./pages/MapPage";

export const App = () => {
  const rootStore = new RootStore();
  return (
    <Router>
      <RootStorePrivider store={rootStore}>
        <div>
          <MapPage />
        </div>
      </RootStorePrivider>
    </Router>
  );
};
