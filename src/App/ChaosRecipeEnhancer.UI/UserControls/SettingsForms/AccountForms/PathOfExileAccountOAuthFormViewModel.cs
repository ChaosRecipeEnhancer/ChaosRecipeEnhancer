using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms
{
    internal class PathOfExileAccountOAuthFormViewModel : ViewModelBase
    {
        public static void LoginToPoEWebsite()
        {
            GlobalAuthState.Instance.InitializeAuthFlow();
        }

        public static void Logout()
        {
            GlobalAuthState.Instance.PurgeLocalAuthToken();
        }
    }
}
