namespace GameVersionSelector;

public interface IMessageWriter
{
    public void WriteLine(string message);
}

/// <summary>
/// Provides a no-op implementation of the IMessageWriter interface that discards all messages.
/// </summary>
/// <remarks>
/// Use this class when message output is optional or should be suppressed, such as in testing scenarios or when user feedback is not required. All calls to its methods have no effect.
/// </remarks>
public class NullMessageWriter : IMessageWriter
{
    /// <summary>
    /// Print a message to nowhere. Used when no user feedback is needed.
    /// </summary>
    /// <param name="message">Message to be printed.</param>
    public void WriteLine(string message)
    {
        // Do nothing.
    }
}

/// <summary>
/// Writes messages to a WinForms <see cref="RichTextBox"/> control.
/// </summary>
/// <remarks>
/// This implementation is safe to call from background threads. If the target control requires invocation, <see cref="WriteLine(string)"/> marshals the write to the UI thread via <see cref="Control.Invoke(Action)"/>. Each message is appended with <see cref="Environment.NewLine"/> and the control is scrolled to the end so the latest message is visible.
/// </remarks>
/// <param name="messageBox">The <see cref="RichTextBox"/> used to display messages. Must be non-null and created on the UI thread.</param>
public class WinFormsMessageWriter(RichTextBox messageBox) : IMessageWriter
{
    private RichTextBox MessageBox { get; } = messageBox;

    /// <summary>
    /// Appends <paramref name="message"/> to the <see cref="MessageBox"/> and ensures it is scrolled into view. If the call originates from a non-UI thread and <see cref="MessageBox.InvokeRequired"/> is true, the append/scroll operation is executed on the UI thread.
    /// </summary>
    /// <param name="message">The text to append to the message box. The method will append <see cref="Environment.NewLine"/> automatically.</param>
    public void WriteLine(string message)
    {
        // Dispatch action based on what the GUI needs
        if (MessageBox.InvokeRequired)
        {
            MessageBox.Invoke(() => WriteLineAndScroll(message));
        }
        else
        {
            WriteLineAndScroll(message);
        }
    }

    /// <summary>
    /// Performs the actual append and scroll operations on the <see cref="MessageBox"/>. Intended to run on the UI thread.
    /// </summary>
    /// <param name="message">The message to append.</param>
    private void WriteLineAndScroll(string message)
    {
        MessageBox.AppendText($"{message}{Environment.NewLine}");
        // Scroll to end after new message is added.
        MessageBox.SelectionStart = MessageBox.TextLength;
        MessageBox.ScrollToCaret();
    }
}
