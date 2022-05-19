using System.Windows.Controls;

namespace EnhancePoE.UI.UserControls
{
    public interface IDynamicGridControl
    {
        Button GetButtonFromCell(object cell);
    }
}
