using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using ChaosRecipeEnhancer.UI.Constants;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class CustomDialog
{
    private readonly string _messageToCopy;

    public CustomDialog(string title, string message)
    {
        InitializeComponent();

        InitializeComponent();
        Title = title;
        _messageToCopy = message; // Store the message for copying

        // Set up the message with hyperlinks and emphasized text
        var reportingText = new Run("Please report this issue on ")
        {
            Style = (Style)Resources["EmphasizedTextStyle"]
        };
        ReportTextBlock.Inlines.Add(reportingText);
        ReportTextBlock.Inlines.Add(CreateHyperlink("GitHub", AppInfo.GithubIssuesUrl, (Style)Resources["HyperlinkStyle"]));
        ReportTextBlock.Inlines.Add(new Run(" or ")
        {
            Style = (Style)Resources["EmphasizedTextStyle"]
        });
        ReportTextBlock.Inlines.Add(CreateHyperlink("Discord.", AppInfo.DiscordUrl, (Style)Resources["HyperlinkStyle"]));

        // Add the exception message as normal text
        var exceptionText = new Run(message);
        ExceptionTextBlock.Inlines.Add(exceptionText);
    }

    private void CopyButtonClick(object sender, RoutedEventArgs e)
    {
        // Create a template with placeholders for the version and message
        var template =
            $"Reporting an exception I received while using Chaos Recipe Enhancer {AppInfo.VersionText}:\n" +
            $"```\n{_messageToCopy}\n```";

        Clipboard.SetText(template);

        MessageBox.Show("Error details copied to clipboard.", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private Hyperlink CreateHyperlink(string text, string url, Style style)
    {
        var link = new Hyperlink(new Run(text))
        {
            NavigateUri = new Uri(url),
            Style = style
        };
        link.RequestNavigate += (sender, e) =>
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        };
        return link;
    }
}