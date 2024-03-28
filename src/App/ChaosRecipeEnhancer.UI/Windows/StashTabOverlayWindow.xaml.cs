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

            // save the position of the window after it's been moved
            Settings.Default.Save();
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
                textBlock.FontWeight = FontWeights.DemiBold;

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
            ActivateNextSetBySet(active, stashTabCell);
        }
    }

    public void ActivateNextSetBySet(bool active, InteractiveStashTabCell stashTabCell)
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
                                SetsToHighlight[0].Items.Where(x => x.Id == currentTab.Id).ToList().Count == 0)
                            {
                                currentTab.TabHeaderColor = Brushes.Transparent;
                            }
                        }
                    }

                    // activate next set
                    if (SetsToHighlight[0].Items != null)
                    {
                        foreach (var enhancedItem in SetsToHighlight[0].Items.ToList())
                        {
                            var currentTab = GetStashTabFromItem(enhancedItem);
                            currentTab?.ActivateItemCells(enhancedItem);
                            currentTab.SetTabHeaderColorForHighlightingFromUserSettings();
                        }
                    }

                    // Set has been completed
                    if (SetsToHighlight[0].Items != null && SetsToHighlight[0].Items.Count == 0)
                    {
                        SetsToHighlight.RemoveAt(0);

                        // play sound to notify user that a set has been completed
                        _model.PlaySetPickingCompletedNotificationSound();

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

                    // Check if the current set is empty after removing the item
                    if (currentSet.Items.Count == 0)
                    {
                        SetsToHighlight.RemoveAt(0);

                        // Play the notification sound when the set is completed
                        _model.PlaySetPickingCompletedNotificationSound();
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
                        nextTab.SetTabHeaderColorForHighlightingFromUserSettings();
                    }
                    else
                    {
                        nextTab.TabHeaderColor = Brushes.Transparent;
                    }
                }
            }
            else
            {
                // If the current set is empty, activate the next set
                ActivateNextItemByItem(active, null);
            }
        }
    }

    private void PrepareSelling()
    {
        SetsToHighlight.Clear();

        foreach (var s in StashTabControlManager.StashTabControls) s.PrepareOverlayList();

        if (Settings.Default.StashTabOverlayHighlightMode == (int)StashTabOverlayHighlightMode.ItemByItem)
        {
            var completeSets = new List<EnhancedItemSet>();

            foreach (var set in GlobalItemSetManagerState.SetsInProgress)
            {
                set.OrderItemsForPicking();

                if ((set.HasChaosRecipeQualifier && Settings.Default.ChaosRecipeTrackingEnabled) ||
                    (set.IsRegalRecipeEligible && !Settings.Default.ChaosRecipeTrackingEnabled))
                {
                    if (set.EmptyItemSlots == null || set.EmptyItemSlots.Count == 0)
                    {
                        completeSets.Add(new EnhancedItemSet
                        {
                            Items = new List<EnhancedItem>(set.Items),
                            EmptyItemSlots = new List<string>()
                        });
                    }
                }
            }

            foreach (var completeSet in completeSets)
            {
                SetsToHighlight.Add(new EnhancedItemSet
                {
                    Items = new List<EnhancedItem>(completeSet.Items),
                    EmptyItemSlots = new List<string>()
                });
            }
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
        var stashTabMetadataList = GlobalItemSetManagerState.StashTabMetadataListStashesResponse;

        if ((Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsByIndex && !string.IsNullOrWhiteSpace(Settings.Default.StashTabIndices)) ||
            (Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.TabNamePrefix && !string.IsNullOrWhiteSpace(Settings.Default.StashTabPrefixIndices)))
        {
            StashTabControlManager.GetStashTabIndicesFromSettingsForQueryByIndex();
        }
        else if (Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsById && !string.IsNullOrWhiteSpace(Settings.Default.StashTabIdentifiers))
        {
            StashTabControlManager.GetStashTabIndicesFromSettingsForQueryById(stashTabMetadataList);
        }

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
                        var tabToAdd = new StashTabControl(
                            tab.Id,
                            tab.Name,
                            tab.Index
                        );

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
            if (itemModel.StashTabId == s.Id) return s;
        }

        return null;
    }
}