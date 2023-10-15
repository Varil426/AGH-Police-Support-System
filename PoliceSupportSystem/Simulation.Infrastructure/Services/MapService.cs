using System.Data;
using Npgsql;
using Shared.CommonTypes.Geo;
using Simulation.Application.Services;
using Path = Shared.CommonTypes.Geo.Path;

namespace Simulation.Infrastructure.Services;

internal sealed class MapService : IMapService
{
    private readonly NpgsqlDataSource _dataSource;

    private const int DefaultSrid = 4326;

    public MapService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Route> GetRoute(Position from, Position to)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand($"""
                                                    select
                                                    	x1,
                                                    	y1,
                                                    	x2,
                                                    	y2,
                                                    	length_m
                                                    from
                                                    	(
                                                    	select
                                                    		*
                                                    	from
                                                    		pgr_dijkstra('select gid as id, source, target, cost, reverse_cost from ways',
                                                    		(
                                                    		select
                                                    			id
                                                    		from
                                                    			ways_vertices_pgr
                                                    		order by
                                                    			the_geom <-> ST_GeometryFromText('POINT({from.Longitude} {from.Latitude})',
                                                    			{DefaultSrid})
                                                    		limit 1),
                                                    		(
                                                    		select
                                                    			id
                                                    		from
                                                    			ways_vertices_pgr
                                                    		order by
                                                    			the_geom <-> ST_GeometryFromText('POINT({to.Longitude} {to.Latitude})',
                                                    			{DefaultSrid})
                                                    		limit 1),
                                                    		false)) as a
                                                    join ways on
                                                    	a.edge = ways.gid
                                                    order by
                                                    	a.path_seq;
                                                    
                                                    """, connection);
        await using var reader = await command.ExecuteReaderAsync();
        var paths = new List<Path>();
        
        while (await reader.ReadAsync())
        {
	        var sourceLongitude = reader.GetDouble("x1");
	        var sourceLatitude = reader.GetDouble("y1");
	        
	        var targetLongitude = reader.GetDouble("x2");
	        var targetLatitude = reader.GetDouble("y2");

	        var distance = reader.GetDouble("length_m");
	        
	        paths.Add(new Path(new Position(sourceLatitude, sourceLongitude), new Position(targetLatitude, targetLongitude), distance));
        }
        
        return new Route(paths);
    }

    public async Task<IEnumerable<string>> GetDistrictNames()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand("select distinct name from planet_osm_line where name is not null;", connection);
        await using var reader = await command.ExecuteReaderAsync();
        var names = new List<string>();
        while (await reader.ReadAsync())
            names.Add(reader.GetString("name"));
        return names;
    }

    public async Task<IEnumerable<Position>> GetRandomPositionsInDistrict(string districtName, int numberOfPositions = 1)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand($"select ST_X(the_geom) as longitude, ST_Y(the_geom) as latitude from osm_nodes where ST_Contains((select st_makepolygon(st_transform(way, {DefaultSrid})) from planet_osm_line where name = '{districtName}'), the_geom) order by random() limit {numberOfPositions};", connection);
        await using var reader = await command.ExecuteReaderAsync();
        var positions = new List<Position>();
        while (await reader.ReadAsync())
            positions.Add(new Position(reader.GetDouble("latitude"), reader.GetDouble("longitude")));
        return positions;
    }
}