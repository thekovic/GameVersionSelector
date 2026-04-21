using System.IO.Compression;
using System.Net.Http.Json;
using System.Text.Json;

namespace GameVersionSelector;

/// <summary>
/// Represents a game's metadata including its install folder name, Steam App ID, and a mapping of human-readable patch names to arrays of depot manifests.
/// </summary>
public class Game
{
    /// <summary>
    /// The folder name used for the game's installation directory.
    /// </summary>
    public required string FolderName { get; set; }

    /// <summary>
    /// The Steam App ID associated with this game.
    /// </summary>
    public required string AppId { get; set; }

    /// <summary>
    /// A dictionary mapping patch names (for example "Update 2") to arrays of <see cref="Depot"/> entries that represent the depots/manifests required for that patch/version.
    /// </summary>
    public required Dictionary<string, Depot[]> Patches { get; set; }
}

/// <summary>
/// Represents a single Steam depot and the specific manifest ID to fetch.
/// </summary>
public class Depot
{
    /// <summary>
    /// The Steam depot identifier.
    /// </summary>
    public required string DepotId { get; set; }

    /// <summary>
    /// The manifest identifier for this depot which points to a specific content snapshot.
    /// </summary>
    public required string ManifestId { get; set; }
}

/// <summary>
/// Manages the depot database used by the application.
/// </summary>
public class DepotDatabase
{
    private static IMessageWriter MessageWriter { get => AppState.Instance.MessageWriter; }

    /// <summary>
    /// URL to download the prebuilt Windows x64 version of DepotDownloader used to fetch Steam depots.
    /// </summary>
    private const string DEPOT_DOWNLOADER_WIN64_URL = "https://github.com/SteamRE/DepotDownloader/releases/download/DepotDownloader_3.4.0/DepotDownloader-windows-x64.zip";

    /// <summary>
    /// URL to the online JSON file containing the database of games and their depot manifests.
    /// </summary>
    private const string ONLINE_DATABASE_URL = "https://raw.githubusercontent.com/thekovic/GameVersionSelector/refs/heads/main/DepotDatabase.json";

    /// <summary>
    /// Common JSON serializer options used for both reading and writing the depot database.
    /// </summary>
    private static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Ensures that a local copy of <c>DepotDownloader.exe</c> exists. If not found, the method downloads the official zip release and extracts it into the current working directory.
    /// </summary>
    /// <returns>A task that completes once the tool has been downloaded and extracted.</returns>
    public async Task InitDepotDownloader()
    {
        if (File.Exists("DepotDownloader.exe"))
        {
            return;
        }

        MessageWriter.WriteLine("DepotDownloader not found. Downloading...");

        using var httpClient = new HttpClient();
        var archiveData = await httpClient.GetByteArrayAsync(DEPOT_DOWNLOADER_WIN64_URL);
        using var archiveStream = new ZipArchive(new MemoryStream(archiveData));

        archiveStream.ExtractToDirectory(".");
    }

    /// <summary>
    /// Holds the database loaded from the online JSON source. Null when not yet loaded or if loading failed.
    /// </summary>
    private Dictionary<string, Game>? _onlineDatabase;

    /// <summary>
    /// Attempts to fetch and deserialize the online depot database JSON into memory. Uses <see cref="JsonOptions"/> during deserialization.
    /// </summary>
    /// <returns>A task that completes when the database has been fetched and parsed.</returns>
    public async Task InitOnlineDatabase()
    {
        using var httpClient = new HttpClient();
        _onlineDatabase = await httpClient.GetFromJsonAsync<Dictionary<string, Game>>(ONLINE_DATABASE_URL, JsonOptions);
    }

    /// <summary>
    /// Returns the currently available database. If an online database was successfully loaded via <see cref="InitOnlineDatabase"/>, that is returned; otherwise the embedded offline database is used as a fallback.
    /// </summary>
    public Dictionary<string, Game> Database => _onlineDatabase ?? _offlineDatabase;

    /// <summary>
    /// Serializes the embedded offline database to JSON and writes it to the given file path. This is primarily intended for exporting or debugging the built-in dataset.
    /// </summary>
    /// <param name="filePath">Destination file path where the exported JSON will be written.</param>
    public void ExportOfflineDatabase(string filePath)
    {
        string jsonString = JsonSerializer.Serialize(_offlineDatabase, JsonOptions);
        File.WriteAllText(filePath, jsonString);
    }

    /// <summary>
    /// Embedded database containing a curated set of games and patch manifests. This dataset is used as a fallback when the online database is not available.
    /// </summary>
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
