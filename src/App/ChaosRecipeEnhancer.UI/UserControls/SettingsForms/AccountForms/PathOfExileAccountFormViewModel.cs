using System;
using System.Net.Http;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

internal class PathOfExileAccountFormViewModel : ViewModelBase
{
    // private readonly IApiService _apiService = Ioc.Default.GetService<IApiService>();
    private readonly IApiService _apiService = new ApiService();

    public async void TestConnectionToPoEServers()
    {
        try
        {
            var accountName = Settings.PathOfExileAccountName;
            var secret = Settings.PathOfExileWebsiteSessionId;

            // simple 'health check' that will ping for your account's stash metadata in standard league
            // todo: can we call something lighter for a simple auth request to test connection?
            var tabs = await _apiService.GetAllPersonalStashTabMetadataAsync(
                accountName,
                AppInfo.DefaultLeagueForHealthCheck,
                secret
            );

            if (tabs is null)
            {
                Settings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionError;
            }
            else
            {
                Settings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ValidatedConnection; // 1 = validated connection
            }
        }
        catch (InvalidOperationException)
        {
            Settings.PoEAccountConnectionStatus =
                (int)ConnectionStatusTypes.ConnectionNotValidated; // 0 = connection not validated (malformed user credentials - maybe typo in username or session id)
        }
        catch (HttpRequestException e)
        {
            Settings.PoEAccountConnectionStatus =
                e.Message.Contains("400") || e.Message.Contains("401") || e.Message.Contains("403")
                    ? (int)ConnectionStatusTypes.ConnectionNotValidated // 0 = connection not validated (invalid user credentials - maybe expired session id)
                    : (int)ConnectionStatusTypes.ConnectionError; // 2 = connection validation error (poe server issues)
        }
        catch
        {
            Settings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionNotValidated; // 0 = connection not validated (some other cre error)
        }
        finally
        {
            Settings.Save();
        }
    }
}