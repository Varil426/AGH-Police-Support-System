# Based on https://github.com/pgRouting/docker-pgrouting/tree/master/15-3.3-3.4

FROM postgis/postgis:15-3.3 AS pgrouting-stage

LABEL maintainer="pgRouting Project - https://pgrouting.org"

ENV PGROUTING_VERSION 3.4.1
ENV PGROUTING_SHA256 a4e034efee8cf67582b67033d9c3ff714a09d8f5425339624879df50aff3f642

RUN set -ex \
    && apt update \
    && apt install -y \
    libboost-atomic1.74.0 \
    libboost-chrono1.74.0 \
    libboost-graph1.74.0 \
    libboost-date-time1.74.0 \
    libboost-program-options1.74.0 \
    libboost-system1.74.0 \
    libboost-thread1.74.0 \
    && apt install -y \
    build-essential \
    cmake \
    wget \
    libboost-graph-dev \
    libpq-dev \
    postgresql-server-dev-${PG_MAJOR} \
    && wget -O pgrouting.tar.gz "https://github.com/pgRouting/pgrouting/archive/v${PGROUTING_VERSION}.tar.gz" \
    && echo "$PGROUTING_SHA256 *pgrouting.tar.gz" | sha256sum -c - \
    && mkdir -p /usr/src/pgrouting \
    && tar \
    --extract \
    --file pgrouting.tar.gz \
    --directory /usr/src/pgrouting \
    --strip-components 1 \
    && rm pgrouting.tar.gz \
    && cd /usr/src/pgrouting \
    && mkdir build \
    && cd build \
    && cmake .. \
    && make \
    && make install \
    && cd / \
    && rm -rf /usr/src/pgrouting \
    && apt-mark manual postgresql-15 \
    && apt purge -y --autoremove \
    build-essential \
    cmake \
    wget \
    libpq-dev \
    libboost-graph-dev \
    postgresql-server-dev-${PG_MAJOR} \
    && rm -rf /var/lib/apt/lists/*
RUN rm /docker-entrypoint-initdb.d/10_postgis.sh

# FROM postgis/postgis:12-3.3 AS pgrouting-stage

# LABEL maintainer="pgRouting Project - https://pgrouting.org"

# ENV PGROUTING_VERSION 3.3.5
# ENV PGROUTING_SHA256 7a6c3ed612b6f803d8919a3fa66b8105cd48177ca0c872878e06c93c7b53537f

# RUN set -ex \
#     && apt update \
#     && apt install -y \
#     libboost-atomic1.74.0 \
#     libboost-chrono1.74.0 \
#     libboost-graph1.74.0 \
#     libboost-date-time1.74.0 \
#     libboost-program-options1.74.0 \
#     libboost-system1.74.0 \
#     libboost-thread1.74.0 \
#     && apt install -y \
#     build-essential \
#     cmake \
#     wget \
#     libboost-graph-dev \
#     libpq-dev \
#     postgresql-server-dev-${PG_MAJOR} \
#     && wget -O pgrouting.tar.gz "https://github.com/pgRouting/pgrouting/archive/v${PGROUTING_VERSION}.tar.gz" \
#     && echo "$PGROUTING_SHA256 *pgrouting.tar.gz" | sha256sum -c - \
#     && mkdir -p /usr/src/pgrouting \
#     && tar \
#     --extract \
#     --file pgrouting.tar.gz \
#     --directory /usr/src/pgrouting \
#     --strip-components 1 \
#     && rm pgrouting.tar.gz \
#     && cd /usr/src/pgrouting \
#     && mkdir build \
#     && cd build \
#     && cmake .. \
#     && make \
#     && make install \
#     && cd / \
#     && rm -rf /usr/src/pgrouting \
#     && apt-mark manual postgresql-12 \
#     && apt purge -y --autoremove \
#     build-essential \
#     cmake \
#     wget \
#     libpq-dev \
#     libboost-graph-dev \
#     postgresql-server-dev-${PG_MAJOR} \
#     && rm -rf /var/lib/apt/lists/*
# RUN rm /docker-entrypoint-initdb.d/10_postgis.sh

FROM pgrouting-stage AS osm2pgrouting-stage

ENV OSM2PGROUTING_VERSION 2.3.8

RUN apt update \
    && apt install -y \
    libpqxx-6.4 \
    && apt install -y \
    build-essential \
    cmake \
    wget \
    libboost-program-options-dev \
    libexpat1 \
    libexpat-dev \
    libosmium2-dev \
    libpqxx-dev \
    zlib1g-dev \
    && cd /usr/local/src \
    && wget https://github.com/pgRouting/osm2pgrouting/archive/v${OSM2PGROUTING_VERSION}.tar.gz \
    && tar xvf v${OSM2PGROUTING_VERSION}.tar.gz \
    && cd osm2pgrouting-${OSM2PGROUTING_VERSION} \
    && mkdir build \
    && cd build \
    && cmake .. \
    && make \
    && make install \
    && cd ../tools/osmium/ \
    && mkdir build \
    && cd build \
    && cmake .. \
    && make \
    && make install \
    && cd /usr/local/src \
    && rm -rf ./* \
    && apt purge -y --autoremove \
    build-essential \
    cmake \
    wget \
    libboost-program-options-dev \
    libexpat-dev \
    libosmium2-dev \
    libpqxx-dev \
    zlib1g-dev \
    && apt autoremove -y \
    && rm -rf /var/lib/apt/lists/*

# RUN apt update \
#     && apt install -y osm2pgrouting

# FROM pgrouting-stage as osm2pgrouting-stage

# ENV OSM2PGROUTING_VERSION 2.3.8

# RUN apt update \
#     && apt install -y \
#     libpqxx-6.4 \
#     && apt install -y \
#     build-essential \
#     cmake \
#     wget \
#     libboost-program-options-dev \
#     libexpat1 \
#     libexpat-dev \
#     libosmium2-dev \
#     libpqxx-dev \
#     zlib1g-dev \
#     && cd /usr/local/src \
#     && wget https://github.com/pgRouting/osm2pgrouting/archive/v${OSM2PGROUTING_VERSION}.tar.gz \
#     && tar xvf v${OSM2PGROUTING_VERSION}.tar.gz \
#     && cd osm2pgrouting-${OSM2PGROUTING_VERSION} \
#     && mkdir build \
#     && cd build \
#     && cmake .. \
#     && make \
#     && make install \
#     && cd ../tools/osmium/ \
#     && mkdir build \
#     && cd build \
#     && cmake .. \
#     && make \
#     && make install \
#     && cd /usr/local/src \
#     && rm -rf ./* \
#     && apt purge -y --autoremove \
#     build-essential \
#     cmake \
#     wget \
#     libboost-program-options-dev \
#     libexpat-dev \
#     libosmium2-dev \
#     libpqxx-dev \
#     zlib1g-dev \
#     && apt autoremove -y \
#     && rm -rf /var/lib/apt/lists/*

FROM osm2pgrouting-stage AS osm2pgsql-stage

RUN apt update \
    && apt install -y osm2pgsql

FROM osm2pgsql-stage AS db-init-stage

COPY ./setup.sql /docker-entrypoint-initdb.d/

FROM db-init-stage AS map-config-stage

COPY ./mapconfig.xml /mapconfig.xml

FROM map-config-stage AS script-import-stage

# RUN apt update \
#     && apt install -y sudo

COPY ./start.sh /run/

CMD ["sh", "-c", "/run/start.sh"]