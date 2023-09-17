import "./App.css";
import { RootStorePrivider } from "./utils/RootStoreProvider";
import { Router } from "wouter";
import { MapPage } from "./pages/MapPage";
import { RootStore } from "./stores/RootStore";
import { IncidentStore } from "./stores/IncidentStore";
import { ToastContainer } from "react-toastify";

export const App = () => {
  const incidentStore = new IncidentStore();
  const rootStore = new RootStore(incidentStore);

  return (
    <Router>
      <RootStorePrivider store={rootStore}>
        <ToastContainer />
        <div>
          <MapPage />
        </div>
      </RootStorePrivider>
    </Router>
  );
};
