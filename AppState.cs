using Microsoft.Win32;

namespace GameVersionSelector;

/// <summary>
/// Provides centralized access to application-wide services, configuration, and global state.
/// </summary>
/// <remarks>
/// AppState acts as a singleton, exposing key application data and services. Use the static Instance property to access the current application state after initialization. This class is intended to be used as the main entry point for accessing shared resources and services throughout the application's lifecycle.
/// </remarks>
public class AppState
{
    private static AppState? _instance;

    /// <summary>
    /// Gets the current <see cref="AppState"/> singleton instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the application services have not been initialized and <see cref="Instance"/> is accessed.
    /// </exception>
    public static AppState Instance
    {
        get
        {
            // This should never happen.
            if (_instance == null)
            {
                throw new InvalidOperationException("ERROR: Application services not initialized.");
            }

            return _instance;
        }
    }

    /// <summary>
    /// Writes user-facing messages.
    /// </summary>
    public IMessageWriter MessageWriter { get; }

    /// <summary>
    /// Resolved path to Steam's "common" directory (typically "...Steam/steamapps/common").
    /// </summary>
    /// <remarks>
    /// This value is initialized from the registry at startup. If Steam is not found in the registry, this property will be an empty string.
    /// </remarks>
    public string SteamPath { get; set; } = GetSteamPathFromRegistry();

    /// <summary>
    /// Steam username used by the application for any non-persistent operations.
    /// </summary>
    public string SteamUsername { get; set; } = string.Empty;

    /// <summary>
    /// Steam password used by the application for any non-persistent operations.
    /// </summary>
    public string SteamPassword { get; set; } = string.Empty;

    /// <summary>
    /// The identifier or friendly name of the currently selected game.
    /// </summary>
    public string SelectedGame { get; set; } = string.Empty;

    /// <summary>
    /// The patch/version currently selected for installation.
    /// </summary>
    public string SelectedPatch { get; set; } = string.Empty;

    /// <summary>
    /// In-memory database containing depot and patch metadata.
    /// </summary>
    public DepotDatabase DepotDatabase { get; } = new DepotDatabase();

    /// <summary>
    /// Creates a new <see cref="AppState"/> and sets the global <see cref="Instance"/>.
    /// </summary>
    /// <param name="messageWriter">The message writer used to produce logs and UI messages.</param>
    public AppState(IMessageWriter messageWriter)
    {
        _instance = this;
        MessageWriter = messageWriter;
    }

    /// <summary>
    /// Attempts to read the Steam installation path from the registry in order to create the path to Steam's root game installation folder (steamapps/common).
    /// </summary>
    /// <returns>
    /// Absolute path to the "Steam/steamapps/common" directory, or an empty string if Steam is not found.
    /// </returns>
    private static string GetSteamPathFromRegistry()
    {
        object? registryValue = Registry.GetValue(
            @"HKEY_CURRENT_USER\Software\Valve\Steam",
            "SteamPath",
            null
        );
        if (registryValue is null)
        {
            return string.Empty;
        }

        string fixedPath = Path.GetFullPath(registryValue.ToString()!);
        return Path.Combine(fixedPath, "steamapps", "common");
    }

    /// <summary>
    /// Launches the external DepotDownloader process to download the depots required for the currently selected game and patch.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if required runtime state is missing (for example, an empty <see cref="SteamPath"/> or missing Steam credentials when required).
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the external tool <c>DepotDownloader.exe</c> exits with a non-zero exit code indicating a failure to download a depot.
    /// </exception>
    /// <remarks>
    /// For each depot referenced by the selected patch, this method builds the appropriate command-line arguments and invokes <see cref="OsUtils.LaunchProcess(string, string[], string, CancellationToken)"/>.
    /// Credentials supplied via <see cref="SteamUsername"/> and <see cref="SteamPassword"/> are used only for the current session and are not persisted.
    /// On successful completion a user-visible success message is written using <see cref="MessageWriter"/>.
    /// </remarks>
    public async Task LaunchDepotDownloader(CancellationToken cancellationToken)
    {
        var game = DepotDatabase.Database[SelectedGame];
        var depots = game.Patches[SelectedPatch];
        foreach (var depot in depots)
        {
            string[] args = [
                // Specify the app, depot, and manifest to download.
                "-app", $"{game.AppId}",
                "-depot", $"{depot.DepotId}",
                "-manifest", $"{depot.ManifestId}",
                // Pass Steam credentials to authenticate with Steam. These credentials are not stored persistently and are only used for the current session. If two-factor authentication is enabled on the account, the user must also confirm the login through the Steam Guard app before DepotDownloader can download the selected depot.
                "-username", $"{SteamUsername}",
                "-password", $"{SteamPassword}",
                // Specify the output directory for the downloaded depot. By default, the app encourages the user to select their "Steam/steamapps/common" directory which means the downloaded version of the game replaces the existing installation in-place. However, users can select any directory they want if they prefer to preserve their existing installation.
                "-dir", $"{Path.Combine(SteamPath, game.FolderName)}"
            ];

            int errorCode = await OsUtils.LaunchProcess("DepotDownloader.exe", args, ".", cancellationToken);
            if (errorCode != 0)
            {
                throw new Exception($"ERROR: DepotDownloader exited with code {errorCode}. Installation may not be complete.");
            }
        }

        MessageWriter.WriteLine($"{Environment.NewLine}Installation of {SelectedGame} version {SelectedPatch} completed successfully. You may close the app now.{Environment.NewLine}");
    }
}
