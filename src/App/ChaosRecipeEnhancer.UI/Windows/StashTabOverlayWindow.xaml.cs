using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.UserControls.StashTab;
using ChaosRecipeEnhancer.UI.Utilities.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class StashTabOverlayWindow : Window
{
    private readonly StashTabOverlayViewModel _model;
    private static List<EnhancedItemSet> SetsToHighlight { get; } = new();

    public StashTabOverlayWindow()
    {
        InitializeComponent();
        DataContext = _model = new StashTabOverlayViewModel();
        MouseHookForStashTabOverlay.MouseAction += HandleMouseAction;
    }

    private void HandleMouseAction(object sender, MouseHookEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            if (e.Message == MouseHookForStashTabOverlay.MouseMessages.WM_LBUTTONDOWN)
            {
                if (!_model.IsEditing)
                {
                    Coordinates.OverlayClickEvent(this, e);
                }
            }
        });
    }

    public bool IsOpen { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e) => WindowsUtilitiesForOverlays.MakeToolWindow(this);

    protected override void OnClosing(CancelEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void OnEditModeButtonClick(object sender, RoutedEventArgs e) => HandleEditButton();

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    protected override void OnSourceInitialized(EventArgs e)
    {
        MakeWindowClickThrough(true);
        base.OnSourceInitialized(e);
    }

    private void MakeWindowClickThrough(bool clickThrough)
    {
        if (clickThrough)
            WindowsUtilitiesForOverlays.MakeTransparent(this);
        else
            WindowsUtilitiesForOverlays.MakeNormal(this);
    }

    public void HandleEditButton()
    {
        if (_model.IsEditing)
        {
            MakeWindowClickThrough(true);
        }
        else
        {
            MakeWindowClickThrough(false);
        }

        _model.IsEditing = !_model.IsEditing;
    }

    public new virtual void Show()
    {
        StashTabOverlayTabControl.Items.Clear();

        // open stash overlay window
        IsOpen = true;
        _model.IsEditing = false;

        // fetch stash data from api
        GenerateReconstructedStashTabsFromApiResponse();

        // Ensure the user has fetched stash data before populating our Stash Tab Overlay
        if (StashTabControlManager.StashTabControls.Count != 0)
        {
            IsOpen = true;

            // For each individual stash tab in our query results
            foreach (var stashTabData in StashTabControlManager.StashTabControls)
            {
                // Creating an object that represents a Stash Tab (the physical tab that you interact with)
                TabItem newStashTab;

                // Creating a text block that will contain the name of said Stash Tab
                var textBlock = new TextBlock
                {
                    Text = stashTabData.Name,
                    DataContext = stashTabData
                };

                textBlock.SetBinding(TextBlock.BackgroundProperty, new Binding("TabHeaderColor"));
                textBlock.FontSize = 16;

                stashTabData.NameContainer = textBlock;

                if (stashTabData.Quad)
                {
                    newStashTab = new TabItem
                    {
                        Header = textBlock,
                        Content = new QuadStashGrid
                        {
                            ItemsSource = stashTabData.OverlayCellsList
                        }
                    };
                }
                else
                {
                    newStashTab = new TabItem
                    {
                        Header = textBlock,
                        Content = new NormalStashGrid
                        {
                            ItemsSource = stashTabData.OverlayCellsList
                        }
                    };
                }

                StashTabOverlayTabControl.Items.Add(newStashTab);
            }

            StashTabOverlayTabControl.SelectedIndex = 0;

            PrepareSelling();
            ActivateNextCell(true, null);

            MouseHookForStashTabOverlay.Start();
            base.Show();
        }
        else
        {
            MessageBox.Show(
                "No StashTabs Available! Fetch before opening overlay.",
                "Error: Stash Tab Overlay",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    public new virtual void Hide()
    {
        if (!IsOpen) return;

        MakeWindowClickThrough(true);
        _model.IsEditing = false;
        MouseHookForStashTabOverlay.Stop();


        foreach (var i in StashTabControlManager.StashTabControls)
        {
            i.OverlayCellsList.Clear();
            i.NameContainer = null;
        }

        IsOpen = false;
        base.Hide();
    }

    public void ActivateNextCell(bool active, InteractiveStashTabCell stashTabCell)
    {
        if (!active) return;

        if (Settings.Default.StashTabOverlayHighlightMode == (int)StashTabOverlayHighlightMode.ItemByItem)
        {
            ActivateNextItemByItem(active, stashTabCell);
        }
        else
        {
            if (SetsToHighlight != null && SetsToHighlight.Count > 0)
            {
                // check for full sets
                if (SetsToHighlight[0].EmptyItemSlots != null && SetsToHighlight[0].EmptyItemSlots.Count == 0)
                {
                    if (stashTabCell != null && stashTabCell.ItemModel != null)
                    {
                        var highlightItem = stashTabCell.ItemModel;
                        var currentTab = GetStashTabFromItem(highlightItem);

                        if (currentTab != null)
                        {
                            currentTab.DeactivateSingleItemCells(stashTabCell.ItemModel);
                            if (SetsToHighlight[0].Items != null)
                            {
                                SetsToHighlight[0].Items.Remove(highlightItem);
                            }

                            // disable tab header color if no more items in set for the current tab
                            if (SetsToHighlight[0].Items != null &&
                                SetsToHighlight[0].Items.Where(x => x.StashTabIndex == currentTab.Index).ToList().Count == 0)
                            {
                                currentTab.TabHeaderColor = Brushes.Transparent;
                            }
                        }
                    }

                    // activate next set
                    if (SetsToHighlight[0].Items != null)
                    {
                        foreach (var i in SetsToHighlight[0].Items.ToList())
                        {
                            var currentTab = GetStashTabFromItem(i);
                            currentTab?.ActivateItemCells(i);
                            currentTab.TabHeaderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor));
                        }
                    }

                    // Set has been completed
                    if (SetsToHighlight[0].Items != null && SetsToHighlight[0].Items.Count == 0)
                    {
                        SetsToHighlight.RemoveAt(0);

                        // activate next set
                        ActivateNextCell(true, null);
                    }
                }
            }
        }
    }

    private void ActivateNextItemByItem(bool active, InteractiveStashTabCell stashTabCell)
    {
        if (!active) return;

        // Remove if SetsToHighlight is empty
        if (SetsToHighlight.Count == 0)
        {
            return;
        }

        // Get the current item set
        var currentSet = SetsToHighlight[0];

        // Check if the current set has any items left
        if (currentSet.Items.Count > 0)
        {
            if (stashTabCell != null && stashTabCell.ItemModel != null)
            {
                var highlightItem = stashTabCell.ItemModel;
                var currentTab = GetStashTabFromItem(highlightItem);

                if (currentTab != null)
                {
                    currentTab.DeactivateSingleItemCells(stashTabCell.ItemModel);
                    currentSet.Items.Remove(highlightItem);

                    // Set the tab header color to transparent
                    currentTab.TabHeaderColor = Brushes.Transparent;

                    // Check if the current set is empty
                    if (currentSet.Items.Count == 0)
                    {
                        SetsToHighlight.RemoveAt(0);
                    }
                }
            }

            // Activate the next item in the current set
            if (currentSet.Items.Count > 0)
            {
                var nextHighlightItem = currentSet.Items[0];
                var nextTab = GetStashTabFromItem(nextHighlightItem);

                if (nextTab != null)
                {
                    nextTab.ActivateItemCells(nextHighlightItem);

                    // Set the tab header color based on the settings
                    if (!string.IsNullOrEmpty(Settings.Default.StashTabOverlayHighlightColor))
                    {
                        nextTab.TabHeaderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor));
                    }
                    else
                    {
                        nextTab.TabHeaderColor = Brushes.Transparent;
                    }
                }
            }
        }
    }

    private void PrepareSelling()
    {
        SetsToHighlight.Clear();

        foreach (var s in StashTabControlManager.StashTabControls) s.PrepareOverlayList();

        if (Settings.Default.StashTabOverlayHighlightMode == (int)StashTabOverlayHighlightMode.ItemByItem)
        {
            var itemsToHighlight = new List<EnhancedItem>();

            foreach (var set in GlobalItemSetManagerState.SetsInProgress)
            {
                set.OrderItemsForPicking();

                if ((set.HasChaosRecipeQualifier && Settings.Default.ChaosRecipeTrackingEnabled) ||
                    (set.IsRegalRecipeEligible && !Settings.Default.ChaosRecipeTrackingEnabled))
                {
                    itemsToHighlight.AddRange(set.Items);
                }
            }

            SetsToHighlight.Add(new EnhancedItemSet
            {
                Items = itemsToHighlight,
                EmptyItemSlots = new List<string>()
            });
        }
        else
        {
            foreach (var set in GlobalItemSetManagerState.SetsInProgress)
            {
                set.OrderItemsForPicking();

                if ((set.HasChaosRecipeQualifier && Settings.Default.ChaosRecipeTrackingEnabled) ||
                    (set.IsRegalRecipeEligible && !Settings.Default.ChaosRecipeTrackingEnabled))
                {
                    SetsToHighlight.Add(new EnhancedItemSet
                    {
                        Items = new List<EnhancedItem>(set.Items),
                        EmptyItemSlots = new List<string>(set.EmptyItemSlots)
                    });
                }
            }
        }

        Trace.WriteLine("Sets to highlight: " + SetsToHighlight.Count);
    }

    private void GenerateReconstructedStashTabsFromApiResponse()
    {
        var reconstructedStashTabs = new List<StashTabControl>();

        if ((Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsFromList && !string.IsNullOrWhiteSpace(Settings.Default.StashTabIndices)) ||
            (Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.TabNamePrefix && !string.IsNullOrWhiteSpace(Settings.Default.StashTabPrefixIndices)))
        {
            StashTabControlManager.GetStashTabIndicesFromSettings();
        }

        var stashTabMetadataList = GlobalItemSetManagerState.StashTabMetadataListStashesResponse;

        if (stashTabMetadataList != null && StashTabControlManager.StashTabIndices != null)
        {
            foreach (var tab in stashTabMetadataList)
            {
                for (var index = StashTabControlManager.StashTabIndices.Count - 1; index > -1; index--)
                {
                    if (StashTabControlManager.StashTabIndices[index] != tab.Index) continue;

                    StashTabControlManager.StashTabIndices.RemoveAt(index);

                    if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                    {
                        var tabToAdd = new StashTabControl($"[{tab.Index}] {tab.Name}", tab.Index);
                        if (tab.Type == "QuadStash") tabToAdd.Quad = true;
                        reconstructedStashTabs.Add(tabToAdd);
                    }
                }
            }

            StashTabControlManager.StashTabControls = reconstructedStashTabs;
        }
    }

    private StashTabControl GetStashTabFromItem(EnhancedItem itemModel)
    {
        foreach (var s in StashTabControlManager.StashTabControls)
        {
            if (itemModel.StashTabIndex == s.Index) return s;
        }

        return null;
    }
}