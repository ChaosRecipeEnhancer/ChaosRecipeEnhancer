using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Windows;

internal partial class StashTabOverlayWindow
{
    private readonly ItemSetManager _itemSetManager;
    private readonly StashTabOverlayViewModel _model;

    public StashTabOverlayWindow(ItemSetManager itemSetManager)
    {
        _itemSetManager = itemSetManager;
        DataContext = _model = new StashTabOverlayViewModel(_itemSetManager);

        InitializeComponent();

        MouseHook.MouseAction += OnMouseHookClick;
    }

    public bool IsOpen { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Win32.MakeToolWindow(this);
    }

    public new virtual void Hide()
    {
        if (!IsOpen) return;

        MakeWindowClickThrough(true);
        _model.IsEditing = false;
        MouseHook.Stop();

        IsOpen = false;
        base.Hide();
    }

    public new virtual void Show()
    {
        IsOpen = true;

        MouseHook.Start();
        base.Show();
    }

    private void OnMouseHookClick(object sender, MouseHookEventArgs e)
    {
        if (!IsOpen || _model.SelectedStashTabHandler.StashManagerControl is null) return;

        if (ControlHelpers.HitTest(EditModeButton, e.ClickLocation))
            HandleEditButton();
        else
            foreach (var cell in _model.SelectedStashTabHandler.StashManagerControl.OverlayCellsList.Where(cell => cell.Active))
                if (ControlHelpers.HitTest(ControlHelpers.GetContainerForDataObject<Button>(StashTabControl, cell), e.ClickLocation))
                {
                    _itemSetManager.OnItemCellClicked(cell);
                    return;
                }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        MakeWindowClickThrough(true);
    }

    private void MakeWindowClickThrough(bool clickThrough)
    {
        if (clickThrough)
            Win32.MakeTransparent(this);
        else
            Win32.MakeNormal(this);
    }

    private void HandleEditButton()
    {
        if (_model.IsEditing)
        {
            MakeWindowClickThrough(true);
            MouseHook.Start();
            _model.IsEditing = false;
        }
        else
        {
            MouseHook.Stop();
            MakeWindowClickThrough(false);
            _model.IsEditing = true;
        }
    }

    private void OnEditModeButtonClick(object sender, RoutedEventArgs e)
    {
        HandleEditButton();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}