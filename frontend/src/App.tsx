import "./App.css";
import { RootStorePrivider } from "./utils/RootStoreProvider";
import { Router } from "wouter";
import { MapPage } from "./pages/MapPage/MapPage";
import { RootStore } from "./stores/RootStore";
import { IncidentStore } from "./stores/IncidentStore";
import { ToastContainer } from "react-toastify";
import { observer } from "mobx-react-lite";
import { TestPage } from "./pages/TestPage";
import { PatrolStore } from "./stores/PatrolStore";
import { MapPageModel } from "./pages/MapPage/MapPageModel";

export const App = observer(() => {
  const incidentStore = new IncidentStore();
  const patrolStore = new PatrolStore();
  const rootStore = new RootStore(incidentStore, patrolStore);

  return (
    <Router>
      <RootStorePrivider store={rootStore}>
        <ToastContainer />
        <div>
          <MapPage model={new MapPageModel()} />
          <TestPage />
        </div>
      </RootStorePrivider>
    </Router>
  );
});
