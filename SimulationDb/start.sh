#!/bin/bash

# service postgresql start
# postgres 
# /usr/lib/postgresql/13/bin/postgres -D /etc/postgresql/13/main

set -m

# postgres &
docker-entrypoint.sh -c 'shared_buffers=256MB' -c 'max_connections=200' &

# Import map data
sleep 10

FILES="/maps/*"
for f in $FILES
do
  osm2pgrouting --f $f --conf /mapconfig.xml --dbname ${POSTGRES_DB} --username ${POSTGRES_USER} --password ${POSTGRES_PASSWORD} --addnodes --clean
done

fg %1