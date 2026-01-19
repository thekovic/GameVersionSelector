using System.IO.Compression;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamGameVersionSelector;

public class Game
{
    public required string FolderName { get; set; }
    public required string AppId { get; set; }
    public required Dictionary<string, Depot[]> Patches { get; set; }
}

public class Depot
{
    public required string DepotId { get; set; }
    public required string ManifestId { get; set; }
}

public class DepotDatabase
{
    private static IMessageWriter MessageWriter { get => AppState.Instance.MessageWriter; }

    private const string DEPOT_DOWNLOADER_URL = "https://github.com/SteamRE/DepotDownloader/releases/download/DepotDownloader_3.4.0/DepotDownloader-windows-x64.zip";
    private const string ONLINE_DATABASE_URL = "https://raw.githubusercontent.com/thekovic/SteamGameVersionSelector/refs/heads/main/DepotDatabase.json";

    public async Task InitDepotDownloader()
    {
        if (File.Exists("DepotDownloader.exe"))
        {
            return;
        }

        MessageWriter.WriteLine("DepotDownloader not found. Downloading...");

        using var httpClient = new HttpClient();
        var archiveData = await httpClient.GetByteArrayAsync(DEPOT_DOWNLOADER_URL);
        using var archiveStream = new ZipArchive(new MemoryStream(archiveData));

        archiveStream.ExtractToDirectory(".");
    }

    private Dictionary<string, Game>? _onlineDatabase;

    public async Task InitOnlineDatabase()
    {
        using var httpClient = new HttpClient();
        _onlineDatabase = await httpClient.GetFromJsonAsync<Dictionary<string, Game>>(ONLINE_DATABASE_URL, JsonOptions);
    }

    public Dictionary<string, Game> Database => _onlineDatabase ?? _offlineDatabase;

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

    private readonly Dictionary<string, Game> _offlineDatabase = new()
    {
        {
            "Indiana Jones and the Great Circle", new Game
            {
                FolderName = "The Great Circle",
                AppId = "2677660",
                Patches = new Dictionary<string, Depot[]>()
                {
                    {
                        "Day One Release (1.0)", [
                            new Depot { DepotId = "2677662", ManifestId = "4874167609916456876" },
                            new Depot { DepotId = "2830501", ManifestId = "5687220090347415343" },
                            new Depot { DepotId = "2677661", ManifestId = "6309402492463546295" }
                        ]
                    },
                    {
                        "Update 2", [
                            new Depot { DepotId = "2677662", ManifestId = "682938447983161558" },
                            new Depot { DepotId = "2830501", ManifestId = "5687220090347415343" },
                            new Depot { DepotId = "2677661", ManifestId = "2469472959766714306" }
                        ]
                    }
                }
            }
        },
        {
            "Quake 2 Enhanced", new Game
            {
                FolderName = "Quake II",
                AppId = "2320",
                Patches = new Dictionary<string, Depot[]>()
                {
                    {
                        "Update 1 Hotfix", [
                            new Depot { DepotId = "2321", ManifestId = "4487921537736026312" }
                        ]
                    }
                }
            }
        }
    };
}
