const IS_DEVELOPMENT = process.env.NODE_ENV === "development";

const HTTP_PROTOCOL = IS_DEVELOPMENT ? "http" : "https";
// const WSS_PROTOCOL = IS_DEVELOPMENT ? "ws" : "wss";

const HOST = IS_DEVELOPMENT ? "localhost:5299" : "";

const BASE_URL = `${HTTP_PROTOCOL}://${HOST}`;
const BASE_API_URL = `${BASE_URL}/api`;

// const BASE_WEBSOCKET_URL = `${WSS_PROTOCOL}://${HOST}`;

export const Routes = {
  MONITORING_HUB: `${BASE_API_URL}/monitoring`,
};
