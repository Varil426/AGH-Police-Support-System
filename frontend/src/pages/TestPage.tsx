import { observer } from "mobx-react-lite";
import { Route } from "wouter";
import { CounterViewModel } from "../components/testCounter/CounterViewModel";
import { CounterModel } from "../components/testCounter/CounterModel";
// import { useState } from "react";

// TODO Remove
export const TestPage = observer(() => {
  //   const [counter] = useState(new CounterModel());
  const counter = new CounterModel();

  return (
    <Route path="/test">
      <CounterViewModel model={counter} />
    </Route>
  );
});
