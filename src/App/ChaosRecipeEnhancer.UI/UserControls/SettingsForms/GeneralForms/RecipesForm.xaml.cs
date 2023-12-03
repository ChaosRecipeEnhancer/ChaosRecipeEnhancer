using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class RecipesForm
{
    public RecipesForm()
    {
        DataContext = new RecipesFormViewModel();
        InitializeComponent();
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);

        // Check if the new text is within the range
        if (sender is TextBox textBox)
        {
            if (int.TryParse(textBox.Text + e.Text, out var newValue))
            {
                if (newValue is < 1 or > 100)
                {
                    e.Handled = true;
                }
            }
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (int.TryParse(textBox.Text, out int value))
            {
                if (value < 1) textBox.Text = "1";
                else if (value > 100) textBox.Text = "100";
            }
            else
            {
                textBox.Text = "1"; // Reset to 1 if it's not a valid number
            }
        }
    }

}