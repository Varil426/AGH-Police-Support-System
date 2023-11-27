import "./App.css";
import { RootStorePrivider } from "./utils/RootStoreProvider";
import { Router } from "wouter";
import { MainPage } from "./pages/MainPage/MainPage";
import { RootStore } from "./stores/RootStore";
import { IncidentStore } from "./stores/IncidentStore";
import { ToastContainer } from "react-toastify";
import { observer } from "mobx-react-lite";
import { PatrolStore } from "./stores/PatrolStore";
import { MainPageModel } from "./pages/MainPage/MainPageModel";

export const App = observer(() => {
  const incidentStore = new IncidentStore();
  const patrolStore = new PatrolStore();
  const rootStore = new RootStore(incidentStore, patrolStore);

  return (
    <Router>
      <RootStorePrivider store={rootStore}>
        <ToastContainer />
        <div>
          <MainPage model={new MainPageModel()} />
        </div>
      </RootStorePrivider>
    </Router>
  );
});
