using System.Diagnostics;

namespace SteamGameVersionSelector;

public partial class SteamGameVersionSelectorForm : Form
{
    /// <summary>
    /// Default margin value used for all the controls in the GUI. Used when calculating window size during resizing.
    /// </summary>
    private const int MARGIN_COMMON = 6;
    /// <summary>
    /// Double the margin value to account for two sides (left and right or up and down) of the control.
    /// </summary>
    private const int MARGIN_DOUBLE = 2 * MARGIN_COMMON;
    /// <summary>
    /// Default margin value elements use when placed next to the edge of the window.
    /// </summary>
    private const int MARGIN_EDGE = 12;

    private GuiMessageWriter MessageWriter { get; }

    private AppState App { get; }

    private CancellationTokenSource? _launchCts;

    public SteamGameVersionSelectorForm()
    {
        InitializeComponent();

        var applicationVersion = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        Text += $" v{applicationVersion}";

        MessageWriter = new GuiMessageWriter(richTextBoxInstallMessages);
        App = new AppState(MessageWriter);

        ResizeGui();
        StopProgressBar();

        textBoxSteamPath.Text = App.SteamPath;
        folderBrowserDialogSteamPath.InitialDirectory = App.SteamPath;

        UpdateGameComboBox();
        UpdatePatchComboBox();
    }

    private void ResizeGui()
    {
        // Resize Steam path selector.
        textBoxSteamPath.Width = panelContentWrapper.Width - buttonSteamPath.Width - MARGIN_COMMON - (2 * MARGIN_EDGE);
        buttonSteamPath.Location = new Point(panelContentWrapper.Width - buttonSteamPath.Width - MARGIN_EDGE, buttonSteamPath.Location.Y);
        // Resize username / password text boxes and game / patch selectors.
        splitContainer1.Width = panelContentWrapper.Width - (2 * MARGIN_EDGE);
        splitContainer1.SplitterDistance = (splitContainer1.Width - splitContainer1.SplitterWidth) / 2;
        textBoxSteamUsername.Width = splitContainer1.Panel1.Width - labelSteamUsername.Width - (3 * MARGIN_COMMON);
        textBoxSteamPassword.Width = splitContainer1.Panel1.Width - labelSteamUsername.Width - (3 * MARGIN_COMMON);
        comboBoxGameSelector.Width = splitContainer1.Panel2.Width - labelPatchSelector.Width - (3 * MARGIN_COMMON);
        comboBoxPatchSelector.Width = splitContainer1.Panel2.Width - labelPatchSelector.Width - (3 * MARGIN_COMMON);
        // Resize message box.
        richTextBoxInstallMessages.Width = panelContentWrapper.Width - (2 * MARGIN_EDGE);
        richTextBoxInstallMessages.Height = panelContentWrapper.Height - richTextBoxInstallMessages.Location.Y - progressBar1.Height - buttonInstall.Height - MARGIN_COMMON - (2 * MARGIN_EDGE);
        // Position progress bar and install button.
        progressBar1.Location = new Point(progressBar1.Location.X, panelContentWrapper.Height - progressBar1.Height - buttonInstall.Height - (2 * MARGIN_EDGE));
        progressBar1.Width = panelContentWrapper.Width - (2 * MARGIN_EDGE);
        buttonInstall.Location = new Point(panelContentWrapper.Width - buttonInstall.Width - MARGIN_COMMON - buttonCancel.Width - MARGIN_EDGE, panelContentWrapper.Height - buttonInstall.Height - MARGIN_EDGE);
        buttonCancel.Location = new Point(panelContentWrapper.Width - buttonCancel.Width - MARGIN_EDGE, panelContentWrapper.Height - buttonInstall.Height - MARGIN_EDGE);
    }

    private void UpdateGameComboBox()
    {
        // Sort game list alphabetically.
        var gameNames = App.DepotDatabase.Database.Keys.OrderBy(key => key).ToList();
        comboBoxGameSelector.DataSource = gameNames;
    }

    private void UpdatePatchComboBox()
    {
        // DON'T sort patch list alphabetically because we assume it's semantically ordered in the database.
        var patchNames = App.DepotDatabase.Database[App.SelectedGame].Patches.Keys.ToList();
        comboBoxPatchSelector.DataSource = patchNames;
    }

    private void StartProgressBar()
    {
        progressBar1.Style = ProgressBarStyle.Marquee;
        progressBar1.MarqueeAnimationSpeed = 30;
    }

    private void StopProgressBar()
    {
        progressBar1.Style = ProgressBarStyle.Continuous;
        progressBar1.MarqueeAnimationSpeed = 0;
        progressBar1.Value = 0;
    }

    private async void Gui_window_Shown(object sender, EventArgs e)
    {
        try
        {
            await App.DepotDatabase.InitOnlineDatabase();
            MessageWriter.WriteLine("Depot database initialized successfully.");
            // Update combo box data sources again if we switched to online database.
            UpdateGameComboBox();
            UpdatePatchComboBox();
        }
        catch(HttpRequestException httpEx)
        {
            MessageWriter.WriteLine($"WARNING: Failed to initialize depot database because remote resources could not be reached.{Environment.NewLine}{httpEx.Message}{Environment.NewLine}Using offline depot database instead.");
        }
        catch (Exception ex)
        {
            MessageWriter.WriteLine(ex.Message);
        }

        try
        {
            await App.DepotDatabase.InitDepotDownloader();
            MessageWriter.WriteLine("DepotDownloader initialized successfully.");
        }
        catch (Exception ex)
        {
            MessageWriter.WriteLine(ex.Message);
            MessageWriter.WriteLine("ERROR: Failed to initialize DepotDownloader. Game versions cannot be installed.");
            buttonInstall.Enabled = false;
        }
        finally
        {
            MessageWriter.WriteLine("");
        }
    }

    private void Gui_window_Resize(object sender, EventArgs e)
    {
        ResizeGui();
    }

    private void Gui_textBoxSteamPath_TextChanged(object sender, EventArgs e)
    {
        App.SteamPath = textBoxSteamPath.Text;
    }

    private void Gui_buttonSteamPath_Click(object sender, EventArgs e)
    {
        if (folderBrowserDialogSteamPath.ShowDialog() == DialogResult.OK)
        {
            string selectedPath = folderBrowserDialogSteamPath.SelectedPath;
            textBoxSteamPath.Text = selectedPath;
            App.SteamPath = selectedPath;
        }
    }

    private void Gui_textBoxSteamUsername_TextChanged(object sender, EventArgs e)
    {
        App.SteamUsername = textBoxSteamUsername.Text;
    }

    private void Gui_textBoxSteamPassword_TextChanged(object sender, EventArgs e)
    {
        App.SteamPassword = textBoxSteamPassword.Text;
    }

    private void Gui_comboBoxGameSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedGame = comboBoxGameSelector.SelectedItem as string;
        if (selectedGame is null)
        {
            MessageWriter.WriteLine($"ERROR: Failed to select game.");
            return;
        }

        App.SelectedGame = selectedGame;
        UpdatePatchComboBox();
    }

    private void Gui_comboBoxPatchSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedPatch = comboBoxPatchSelector.SelectedItem as string;
        if (selectedPatch is null)
        {
            return;
        }

        App.SelectedPatch = selectedPatch;
    }

    private async void Gui_buttonInstall_Click(object sender, EventArgs e)
    {
        // Prevent double-start.
        if (_launchCts != null)
        {
            MessageWriter.WriteLine("ERROR: DepotDownloader instance is already running.");
            return;
        }

        _launchCts = new CancellationTokenSource();
        try
        {
            StartProgressBar();
            buttonInstall.Enabled = false;

            // TODO: Add process arguments
            var game = App.DepotDatabase.Database[App.SelectedGame]!;
            var depots = game.Patches[App.SelectedPatch]!;
            foreach (var depot in depots)
            {
                string[] args = [
                    "-app",
                    $"{game.AppId}",
                    "-depot",
                    $"{depot.DepotId}",
                    "-manifest",
                    $"{depot.ManifestId}",
                    "-username",
                    $"{App.SteamUsername}",
                    "-password",
                    $"{App.SteamPassword}",
                    "-dir",
                    $"{Path.Combine(App.SteamPath, game.FolderName)}"
                ];

                await OsUtils.LaunchProcess("DepotDownloader.exe", args, ".", _launchCts.Token);
            }

            MessageWriter.WriteLine($"{Environment.NewLine}Installation completed successfully. You may close the app now.{Environment.NewLine}");
        }
        catch (OperationCanceledException)
        {
            MessageWriter.WriteLine("Operation cancelled by user.");
        }
        catch (Exception ex)
        {
            MessageWriter.WriteLine(ex.Message);
        }
        finally
        {
            StopProgressBar();
            buttonInstall.Enabled = true;
            _launchCts?.Dispose();
            _launchCts = null;
        }
    }

    private void Gui_buttonCancel_Click(object sender, EventArgs e)
    {
        // If a process is running, request cancellation and give it time to terminate.
        if (_launchCts != null && !_launchCts.IsCancellationRequested)
        {
            MessageWriter.WriteLine("Shutting down DepotDownloader...");
            _launchCts.Cancel();
        }

        Close();
    }
}
