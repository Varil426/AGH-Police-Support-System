#!/bin/bash

# service postgresql start
# postgres 
# /usr/lib/postgresql/13/bin/postgres -D /etc/postgresql/13/main

set -m

# postgres &
docker-entrypoint.sh -c 'shared_buffers=256MB' -c 'max_connections=200' &

# Wait for DB to start
# https://github.com/docker-library/postgres/issues/146
until pg_isready -U ${POSTGRES_USER} -d ${POSTGRES_DB}; do sleep 1; done
sleep 4
until pg_isready -U ${POSTGRES_USER} -d ${POSTGRES_DB}; do sleep 1; done

# Import map data
osm2pgrouting --f /maps/${MAP_FILE} --conf /mapconfig.xml --dbname ${POSTGRES_DB} --username ${POSTGRES_USER} --password ${POSTGRES_PASSWORD} --addnodes --clean

fg %1