using System;
using System.Net.Http;
using System.Windows;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

internal partial class PathOfExileAccountForm
{
    private readonly StashTabGetter _stashTabGetter = new();

    public PathOfExileAccountForm()
    {
        DataContext = new PathOfExileAccountFormViewModel();
        InitializeComponent();
        TestConnectionToPoEServers();
    }

    private void OnTestConnectionClicked(object sender, RoutedEventArgs e)
    {
        TestConnectionToPoEServers();
    }

    private async void TestConnectionToPoEServers()
    {
        try
        {
            // simple 'health check' that will ping for your account's stash metadata in standard league
            // todo: can we call something lighter for a simple auth request to test connection?
            var tabs = await _stashTabGetter.GetStashPropsAsync(Settings.Default.PathOfExileAccountName.Trim(), "Standard");

            if (tabs is null)
            {
                Settings.Default.PoEAccountConnectionStatus = 2;
            }
            else
            {
                Settings.Default.PoEAccountConnectionStatus = 1; // 1 = validated connection
            }
        }
        catch (InvalidOperationException)
        {
            Settings.Default.PoEAccountConnectionStatus =
                0; // 0 = connection not validated (malformed user credentials - maybe typo in username or session id)
        }
        catch (HttpRequestException e)
        {
            Settings.Default.PoEAccountConnectionStatus =
                e.Message.Contains("400") || e.Message.Contains("401") || e.Message.Contains("403")
                    ? 0 // 0 = connection not validated (invalid user credentials - maybe expired session id)
                    : 2; // 2 = connection validation error (poe server issues)
        }
        catch
        {
            Settings.Default.PoEAccountConnectionStatus = 0; // 0 = connection not validated (some other cre error)
        }
        finally
        {
            Settings.Default.Save();
        }
    }
}