using System.Data;
using Npgsql;
using Simulation.Application.Services;

namespace Simulation.Infrastructure.Services;

internal sealed class MapService : IMapService
{
    private readonly NpgsqlDataSource _dataSource;

    public MapService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<string>> GetDistrictNames()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand("select distinct name from planet_osm_line p where name is not null;", connection);
        await using var reader = await command.ExecuteReaderAsync();
        var names = new List<string>();
        while (await reader.ReadAsync())
            names.Add(reader.GetString("name"));
        return names;
    }
}