import { observer } from "mobx-react-lite";
import { CounterModel } from "./CounterModel";

export interface ICounterViewModelProps {
  model: CounterModel;
}

export const CounterViewModel = observer(
  ({ model }: ICounterViewModelProps) => {
    return (
      <div>
        <text>{model.getCounter}</text>
        <button onClick={() => model.increaseCounter()}>CLICK</button>
      </div>
    );
  }
);
