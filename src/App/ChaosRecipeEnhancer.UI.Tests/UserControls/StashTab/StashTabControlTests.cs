using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.UserControls.StashTab;
using System.Windows.Media;
using Xunit.Sdk;

namespace ChaosRecipeEnhancer.UI.Tests.UserControls.StashTab;

[Collection("Settings")]
public class StashTabControlTests
{
    #region Constructor — TabHeaderColor preserves alpha from StashTabOverlayTabDefaultBackgroundColor

    [WpfFact]
    public void Constructor_TransparentWhiteBackground_PreservesZeroAlpha()
    {
        // Arrange — #00FFFFFF = fully transparent white (the exact repro from issue #646)
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "#00FFFFFF";

        // Act
        var control = new StashTabControl("id", "name", 0);

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(0);
        control.TabHeaderColor.Color.R.Should().Be(255);
        control.TabHeaderColor.Color.G.Should().Be(255);
        control.TabHeaderColor.Color.B.Should().Be(255);
    }

    [WpfFact]
    public void Constructor_FullyOpaqueColor_PreservesFullAlpha()
    {
        // Arrange
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "#FFFF0000";

        // Act
        var control = new StashTabControl("id", "name", 0);

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(255);
        control.TabHeaderColor.Color.R.Should().Be(255);
    }

    [WpfFact]
    public void Constructor_SemiTransparentColor_PreservesAlpha()
    {
        // Arrange — #80 = 128 alpha
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "#800000FF";

        // Act
        var control = new StashTabControl("id", "name", 0);

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(128);
        control.TabHeaderColor.Color.B.Should().Be(255);
    }

    [WpfFact]
    public void Constructor_NullOrEmptySetting_DefaultsToTransparent()
    {
        // Arrange
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "";

        // Act
        var control = new StashTabControl("id", "name", 0);

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(0);
    }

    #endregion

    #region SetTabHeaderColorForHighlightingFromUserSettings — preserves alpha from StashTabOverlayHighlightColor

    [WpfFact]
    public void SetTabHeaderColor_TransparentHighlightColor_PreservesZeroAlpha()
    {
        // Arrange
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "#FF000000";
        Settings.Default.StashTabOverlayHighlightColor = "#00FF0000";
        var control = new StashTabControl("id", "name", 0);

        // Act
        control.SetTabHeaderColorForHighlightingFromUserSettings();

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(0);
        control.TabHeaderColor.Color.R.Should().Be(255);
    }

    [WpfFact]
    public void SetTabHeaderColor_OpaqueHighlightColor_PreservesFullAlpha()
    {
        // Arrange
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "#FF000000";
        Settings.Default.StashTabOverlayHighlightColor = "#FF00FF00";
        var control = new StashTabControl("id", "name", 0);

        // Act
        control.SetTabHeaderColorForHighlightingFromUserSettings();

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(255);
        control.TabHeaderColor.Color.G.Should().Be(255);
    }

    [WpfFact]
    public void SetTabHeaderColor_SemiTransparentHighlightColor_PreservesAlpha()
    {
        // Arrange
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "#FF000000";
        Settings.Default.StashTabOverlayHighlightColor = "#96F90000";
        var control = new StashTabControl("id", "name", 0);

        // Act
        control.SetTabHeaderColorForHighlightingFromUserSettings();

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(0x96);
    }

    [WpfFact]
    public void SetTabHeaderColor_EmptyHighlightColor_DefaultsToTransparent()
    {
        // Arrange
        Settings.Default.StashTabOverlayTabDefaultBackgroundColor = "#FFFFFFFF";
        Settings.Default.StashTabOverlayHighlightColor = "";
        var control = new StashTabControl("id", "name", 0);

        // Act
        control.SetTabHeaderColorForHighlightingFromUserSettings();

        // Assert
        control.TabHeaderColor.Color.A.Should().Be(0);
    }

    #endregion
}
