namespace Simulation.Infrastructure.Settings;

public record DbSettings(string Host, uint Port, string Username, string Password, string DbName = "postgres");