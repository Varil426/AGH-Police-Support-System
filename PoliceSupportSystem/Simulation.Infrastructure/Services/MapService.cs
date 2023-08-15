using System.Data;
using Npgsql;
using Shared.Domain.Geo;
using Simulation.Application.Services;

namespace Simulation.Infrastructure.Services;

internal sealed class MapService : IMapService
{
    private readonly NpgsqlDataSource _dataSource;

    private const int DefaultSrid = 4326;

    public MapService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
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