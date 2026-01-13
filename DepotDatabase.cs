using System.Text.Json;

namespace SteamGameVersionSelector;

public class Game
{
    public required string Name { get; set; }
    public required string FolderName { get; set; }
    public required string AppId { get; set; }
    public required Patch[] Patches { get; set; }
}

public class Patch
{
    public required string Name { get; set; }
    public required Depot[] Depots { get; set; }
}

public class Depot
{
    public required string DepotId { get; set; }
    public required string ManifestId { get; set; }
}

public class DepotDatabase
{
    private Game[]? _onlineDatabase;

    public void InitOnlineDatabase()
    {
        // TODO: Download online database from remote URL.
    }

    public Game[] Database => _onlineDatabase ?? _offlineDatabase;

    private static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    public void ExportOfflineDatabase(string filePath)
    {
        string jsonString = JsonSerializer.Serialize(_offlineDatabase, JsonOptions);
        File.WriteAllText(filePath, jsonString);
    }

    private readonly Game[] _offlineDatabase = [
        new Game
        {
            Name = "HROT",
            FolderName = "HROT",
            AppId = "824600",
            Patches = [
                new Patch
                {
                    Name = "1.3",
                    Depots = [
                        new Depot { DepotId = "824601", ManifestId = "6904601082991261160" }
                    ]
                }
            ]
        }
    ];
}
