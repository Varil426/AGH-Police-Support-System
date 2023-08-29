"use client";

import RootStore from "../stores/root-store";
import { RootStorePrivider } from "../utils/root-store-provider";
import "./globals.css";
import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Police Support System",
  description: "Police Support System",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const rootStore = new RootStore();

  return (
    <RootStorePrivider store={rootStore}>
      <html lang="en">
        <body>{children}</body>
      </html>
    </RootStorePrivider>
  );
}
