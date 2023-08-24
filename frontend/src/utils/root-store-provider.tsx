"use client";

import { ReactNode, createContext, useContext } from "react";
import RootStore from "../stores/root-store";

export const StoreContext = createContext<RootStore | undefined>(undefined);

export interface RootStorePrividerProps {
  store: RootStore;
  children: ReactNode;
}

export function RootStorePrivider({ store, children }: RootStorePrividerProps) {
  return (
    <StoreContext.Provider value={store}>{children}</StoreContext.Provider>
  );
}

function useRootStore() {
  const context = useContext(StoreContext);
  if (context === undefined) {
    throw new Error("useRootStore must be used within RootStoreProvider");
  }

  return context;
}
