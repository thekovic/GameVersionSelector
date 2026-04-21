using System.Diagnostics;

namespace GameVersionSelector;

/// <summary>
/// Utility helpers for managing OS resources.
/// </summary>
/// <remarks>
/// This class centralizes methods used by the application to interact with the operating system.
/// </remarks>
public class OsUtils
{
    /// <summary>
    /// The application's global message writer used to display messages to the user.
    /// </summary>
    private static IMessageWriter MessageWriter { get => AppState.Instance.MessageWriter; }

    /// <summary>
    /// Starts an external process, relays its stdout/stderr to the application's <see cref="IMessageWriter"/>, and asynchronously waits for the process to exit.
    /// </summary>
    /// <param name="processName">The executable name or full path to start (for example, "dotnet" or "C:\tools\mytool.exe").</param>
    /// <param name="args">An array of command-line arguments to pass to the process. Each element is added to the process argument list.</param>
    /// <param name="workingDirectory">The working directory for the process. Pass <c>null</c> or an empty string to use the default.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to request termination of the started process. If cancellation is requested the method attempts to kill the process tree and then rethrows an <see cref="OperationCanceledException"/>.
    /// </param>
    /// <returns>A task that completes when the process exits. The task result is the process exit code.</returns>
    /// <remarks>
    /// The process is started without creating a window (<see cref="ProcessStartInfo.CreateNoWindow"/> = <c>true</c>).
    /// Standard output and error streams of the launched process are redirected and forwarded to <see cref="MessageWriter"/>.
    /// If the provided <paramref name="cancellationToken"/> is cancelled, the method will attempt to kill the process (entire process tree) and then wait for termination before rethrowing the cancellation exception to the caller.
    /// The caller is responsible for handling exceptions from <see cref="Process.Start"/>, such as <see cref="System.ComponentModel.Win32Exception"/>, and for interpreting the returned exit code.
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled via <paramref name="cancellationToken"/>.</exception>
    public static async Task<int> LaunchProcess(string processName, string[] args, string workingDirectory, CancellationToken cancellationToken = default)
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

        try
        {
            // Wait for exit; if cancellation requested, WaitForExitAsync will throw OperationCanceledException
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Cancellation requested — try to terminate the process.
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception ex)
            {
                // Best effort: write message, but do not rethrow here — propagate cancellation below.
                MessageWriter.WriteLine($"Failed to kill process '{processName}': {ex.Message}");
            }

            // Wait for it to actually exit (no cancellation token here to ensure cleanup).
            try
            {
                await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                // ignore
            }

            // rethrow so callers know operation was cancelled
            throw;
        }

        return process.ExitCode;
    }
}
