"use client";

import dynamic from "next/dynamic";
const DynamicMapView = dynamic(
  () => import("../../components/map/map-view"),
  {}
);

const MapPage = () => <MapView />;

export default MapPage;
