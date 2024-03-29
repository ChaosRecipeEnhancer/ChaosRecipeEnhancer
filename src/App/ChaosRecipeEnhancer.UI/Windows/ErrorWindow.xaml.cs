using ChaosRecipeEnhancer.UI.Models.Constants;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class ErrorWindow
{
    private readonly string _messageToCopy;
    private readonly string _preamble;
    public ErrorWindow(string title, string message, string preamble = null)
    {
        InitializeComponent();

        Title = title;
        ExceptionTextBox.Text = message;

        // Setup ReportingTextBlock
        // Set up the message with hyperlinks and emphasized text
        var reportingText = new Run("Feel free to report this issue on ")
        {
            Style = (Style)Resources["EmphasizedTextStyle"]
        };
        ReportTextBlock.Inlines.Add(reportingText);
        ReportTextBlock.Inlines.Add(CreateHyperlink("GitHub", SiteUrls.CreGithubIssuesUrl, (Style)Resources["HyperlinkStyle"]));
        ReportTextBlock.Inlines.Add(new Run(" or ")
        {
            Style = (Style)Resources["EmphasizedTextStyle"]
        });
        ReportTextBlock.Inlines.Add(CreateHyperlink("Discord.", SiteUrls.CreDiscordUrl, (Style)Resources["HyperlinkStyle"]));

        // For external system errors or general messages, configure the dialog as before
        if (!string.IsNullOrEmpty(preamble))
        {
            PreambleTextBlock.Text = preamble;
            PreambleTextBlock.Visibility = Visibility.Visible;
        }

        _preamble = preamble;
        _messageToCopy = message;
    }

    private static Hyperlink CreateHyperlink(string text, string url, Style style)
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

    private void CopyButtonClick(object sender, RoutedEventArgs e)
    {
        // Create a template with placeholders for the version and message
        var template =
            $"Reporting an exception I received while using Chaos Recipe Enhancer {CreAppConstants.VersionText}:\n\n" +
            (_preamble is not null ? $"Error Context:\n```\n{_preamble}\n```\n" : string.Empty) +
            $"Error Details:\n```\n{_messageToCopy}\n```";

        Clipboard.SetText(template);

        MessageBox.Show("Error details copied to clipboard.", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MoreInfoButtonClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(SiteUrls.PathOfExileUrl) { UseShellExecute = true });
    }
}