using Microsoft.Win32;

namespace SteamGameVersionSelector;

/// <summary>
/// Provides centralized access to application-wide services, configuration, and global state.
/// </summary>
/// <remarks>AppState acts as a singleton, exposing key services such as configuration management, message
/// writing, mod installation, and game launching. Use the static Instance property to access the current application
/// state after initialization. This class is intended to be used as the main entry point for accessing shared
/// resources and services throughout the application's lifecycle.</remarks>
public class AppState
{
    private static AppState? _instance;

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

    public IMessageWriter MessageWriter { get; }

    public string SteamPath { get; set; } = GetSteamPathFromRegistry();
    public string SteamUsername { get; set; } = string.Empty;
    public string SteamPassword { get; set; } = string.Empty;

    public DepotDatabase DepotDatabase { get; } = new DepotDatabase();

    public AppState(IMessageWriter messageWriter)
    {
        _instance = this;
        MessageWriter = messageWriter;
    }

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
}