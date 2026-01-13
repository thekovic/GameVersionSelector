using System.Diagnostics;

namespace SteamGameVersionSelector;

public class OsUtils
{
    private static IMessageWriter MessageWriter { get => AppState.Instance.MessageWriter; }

    public static async Task LaunchProcess(string processName, string[] args, string workingDirectory)
    {
        using var process = new Process();

        process.StartInfo.FileName = processName;
        process.StartInfo.WorkingDirectory = workingDirectory;

        // Pass arguments to the process
        foreach (string arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);
        }

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        // Enable capture of stdout of the process
        process.StartInfo.RedirectStandardOutput = true;
        process.OutputDataReceived += new DataReceivedEventHandler((sender, output) =>
        {
            if (!string.IsNullOrEmpty(output.Data))
            {
                MessageWriter.WriteLine(output.Data);
            }
        });

        // Enable capture of stderr of the process
        process.StartInfo.RedirectStandardError = true;
        process.ErrorDataReceived += new DataReceivedEventHandler((sender, output) =>
        {
            if (!string.IsNullOrEmpty(output.Data))
            {
                MessageWriter.WriteLine(output.Data);
            }
        });

        // PER MICROSOFT:
        // This code assumes the process you are starting will terminate itself.
        // Given that it is started without a window so you cannot terminate it
        // on the desktop, it must terminate itself or you can do it programmatically
        // from this application using the Kill method.
        process.Start();
        // Start capturing stdout
        process.BeginOutputReadLine();
        // Start capturing stderr
        process.BeginErrorReadLine();

        // Keep method alive until the process exits so the Process is not disposed early
        await process.WaitForExitAsync().ConfigureAwait(false);
    }
}
