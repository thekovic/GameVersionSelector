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
        buttonInstall.Location = new Point(buttonInstall.Location.X, panelContentWrapper.Height - buttonInstall.Height - MARGIN_EDGE);
        buttonCancel.Location = new Point(buttonCancel.Location.X, panelContentWrapper.Height - buttonInstall.Height - MARGIN_EDGE);
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

    }

    private void Gui_comboBoxPatchSelector_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private async void Gui_buttonInstall_Click(object sender, EventArgs e)
    {
        try
        {
            //await OsUtils.LaunchProcess("C:\\Code\\PatchCreator\\bin\\Debug\\net10.0\\PatchCreator.exe", [], "C:\\Code\\PatchCreator\\bin\\Debug\\net10.0");
            App.DepotDatabase.ExportOfflineDatabase("DepotDatabase.json");
        }
        catch (Exception ex)
        {
            MessageWriter.WriteLine(ex.Message);
        }
    }

    private void Gui_buttonCancel_Click(object sender, EventArgs e)
    {
        // TODO: Implement proper cancellation of ongoing background process.
        Close();
    }
}
