# Police Support System

This project was implemented as part of [my Master's Thesis](https://github.com/Varil426/AGH-Police-Support-System-Thesis). The system was designed to improve the decision-making process when managing police units in a smart city. It's achieved by using contextual data provided by agents such as a _Police Patrol_, a _Navigation Agent_, and a _Smart Gun_. A concept known as MAMS[^1] was used to achieve independence between agents. In order to test the system, a simulation was implemented. To summarise, 6 different microservices were implemented (_WebApp_, _Simulation_, _Patrol Service_, _Navigation Service_, _Gun Service_, _HQ Service_), and also a front-end application was created using [React](https://react.dev/). The microservices send messages between themselves using [RabbitMQ](https://www.rabbitmq.com/). The simulation uses [OSM](https://www.openstreetmap.org/) maps imported to a [PostgreSQL](https://www.postgresql.org/) database to simulate routes on a real city map. Every microservice was dockerized and the whole infrastructure can be set up using a single `docerk run` (see `docker-compose.yaml`).

[^1]: Multi-Agent MicroServices

# Technologies

Technologies used in this project:

- [.NET 7](https://dotnet.microsoft.com/)
  - [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
  - [Autofac](https://www.nuget.org/packages/autofac/)
  - [Npgsql](https://www.nuget.org/packages/Npgsql)
  - [RabbitMQ.MessageBus](https://www.nuget.org/packages/RabbitMQ.MessageBus)
  - [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client)
  - [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr)
  - [Serilog](https://www.nuget.org/packages/Serilog)
  - [Reinforced.Typings](https://www.nuget.org/packages/Reinforced.Typings)
  - [Riok.Mapperly](https://www.nuget.org/packages/Riok.Mapperly)
  - [Nito.AsyncEx](https://www.nuget.org/packages/Nito.AsyncEx)
  - [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)
  - [DotNetZip](https://www.nuget.org/packages/DotNetZip)
  - [BAMCIS.GeoCoordinate](https://www.nuget.org/packages/BAMCIS.GeoCoordinate)
- [React](https://react.dev/)
  - [MobX](https://mobx.js.org/)
  - [Leaflet](https://leafletjs.com/)
    - [React Leaflet](https://react-leaflet.js.org/)
  - [SignalR](https://www.npmjs.com/package/@microsoft/signalr)
  - [Ant Design](https://ant.design/)
  - [Tailwind CSS](https://tailwindcss.com/)
- [RabbitMQ](https://www.rabbitmq.com/)
- [PostgreSQL](https://www.postgresql.org/)
  - [PostGIS](https://postgis.net/)
  - [pgRouting](https://pgrouting.org/)
  - [osm2pgrouting](https://github.com/pgRouting/osm2pgrouting)
  - [osm2pgsql](https://osm2pgsql.org/)
- [Grafana](https://grafana.com/)
  - [Grafana Loki](https://grafana.com/oss/loki/)
- [Docker](https://www.docker.com/)
