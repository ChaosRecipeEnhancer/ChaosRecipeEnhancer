using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.Extensions.Native;
using ChaosRecipeEnhancer.UI.Properties;
using System.Windows.Media;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.UserControls.StashTab;
using ChaosRecipeEnhancer.UI.Utilities;
using System.Linq;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class StashTabOverlayWindow
{
    private readonly IItemSetManagerService _itemSetManagerService = Ioc.Default.GetService<IItemSetManagerService>();

    private readonly StashTabOverlayViewModel _model;
    public static List<EnhancedItemSet> SetsToHighlight { get; } = new();

    public StashTabOverlayWindow()
    {
        InitializeComponent();
        DataContext = _model = new StashTabOverlayViewModel();
        StashTabOverlayTabControl.ItemsSource = _model.OverlayStashTabList;

        NativeMouseExtensions.MouseAction += (s, e) => Coordinates.OverlayClickEvent(this);
    }

    public bool IsOpen { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Win32.MakeToolWindow(this);
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

    public void HandleEditButton()
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

    public new virtual void Show()
    {
        IsOpen = true;

        GenerateReconstructedStashTabsFromApiResponse();

        // Ensure the user has fetched stash data before populating our Stash Tab Overlay
        if (StashTabControlManager.StashTabControls.Count != 0)
        {
            IsOpen = true;

            _model.OverlayStashTabList.Clear();

            // For each individual stash tab in our query results
            foreach (var stashTabData in StashTabControlManager.StashTabControls)
            {
                // Creating an object that represents a Stash Tab (the physical tab that you interact with)
                TabItem newStashTab;

                // Creating a text block that will contain the name of said Stash Tab
                var textBlock = new TextBlock
                {
                    Text = stashTabData.TabName,
                    DataContext = stashTabData
                };

                textBlock.SetBinding(TextBlock.BackgroundProperty, new Binding("TabHeaderColor"));
                textBlock.SetBinding(TextBlock.PaddingProperty, new Binding("TabHeaderWidth"));
                textBlock.FontSize = 16;

                stashTabData.TabNameContainer = textBlock;

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

                _model.OverlayStashTabList.Add(newStashTab);
            }

            StashTabOverlayTabControl.SelectedIndex = 0;

            PrepareSelling();
            ActivateNextCell(true, null, StashTabOverlayTabControl);

            // If "Set by Set" highlight mode enabled, paint all Stash Tab Headers to their respective colors
            if (Settings.Default.StashTabOverlayHighlightMode == (int)StashTabOverlayHighlightMode.SetBySet)
            {
                foreach (var set in SetsToHighlight)
                {
                    foreach (var i in set.Items)
                    {
                        var currTab = GetStashTabFromItem(i);
                        currTab.ActivateItemCells(i);

                        currTab.TabHeaderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor));
                    }
                }
            }

            NativeMouseExtensions.Start();
            base.Show();
        }
        else
        {
            MessageBox.Show("No StashTabs Available! Fetch before opening overlay.", "Error: Stash Tab Overlay",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        MouseHook.Start();
        base.Show();
    }

    public new virtual void Hide()
    {
        if (!IsOpen) return;

        MakeWindowClickThrough(true);
        _model.IsEditing = false;
        MouseHook.Stop();

        foreach (var i in StashTabControlManager.StashTabControls)
        {
            i.OverlayCellsList.Clear();
            i.TabNameContainer = null;
        }

        IsOpen = false;
        base.Hide();
    }

    public void ActivateNextCell(bool active, InteractiveStashTabCell stashTabCell, TabControl tabControl)
    {
        if (!active) return;

        // this is hilariously bad
        var currentlySelectedStashOverlayTabName = tabControl != null
            ? ((TextBlock)((HeaderedContentControl)tabControl.SelectedItem).Header).Text
            : "";

        // activate set by set
        if (Settings.Default.StashTabOverlayHighlightMode == (int)StashTabOverlayHighlightMode.SetBySet)
        {
            if (SetsToHighlight.Count > 0)
            {
                // check for full sets
                if (SetsToHighlight[0].EmptyItemSlots.Count == 0)
                {
                    if (stashTabCell != null)
                    {
                        var highlightItem = stashTabCell.ItemModel;
                        var currentTab = GetStashTabFromItem(highlightItem);

                        if (currentTab != null)
                        {
                            currentTab.DeactivateSingleItemCells(stashTabCell.ItemModel);
                            SetsToHighlight[0].Items.Remove(highlightItem);

                            // disable tab header color if no more items in set for the current tab
                            if (SetsToHighlight[0]
                                    .Items
                                    .Where(x => x.StashTabIndex == currentTab.TabIndex)
                                    .ToList().Count == 0)
                            {
                                currentTab.TabHeaderColor = Brushes.Transparent;
                            }
                        }
                    }

                    foreach (var i in SetsToHighlight[0].Items.ToList())
                    {
                        var currentTab = GetStashTabFromItem(i);
                        currentTab.ActivateItemCells(i);
                    }

                    // Set has been completed
                    if (SetsToHighlight[0].Items.Count == 0)
                    {
                        SetsToHighlight.RemoveAt(0);

                        // activate next set
                        ActivateNextCell(true, null, null);
                    }
                }
            }
        }
        // activate cell by cell / item by item
        else if (Settings.Default.StashTabOverlayHighlightMode == (int)StashTabOverlayHighlightMode.ItemByItem)
        {
            foreach (var s in StashTabControlManager.StashTabControls.ToList())
            {
                s.DeactivateItemCells();
                s.TabHeaderColor = Brushes.Transparent;
            }

            // remove and sound if itemList empty
            if (SetsToHighlight.Count > 0)
            {
                if (SetsToHighlight[0].Items.Count == 0)
                {
                    SetsToHighlight.RemoveAt(0);
                }
            }

            // next item if itemList not empty
            if (SetsToHighlight.Count > 0)
            {
                if (SetsToHighlight[0].Items.Count > 0 && SetsToHighlight[0].EmptyItemSlots.Count == 0)
                {
                    var highlightItem = SetsToHighlight[0].Items[0];
                    var currentTab = GetStashTabFromItem(highlightItem);

                    if (currentTab == null) return;

                    currentTab.ActivateItemCells(highlightItem);

                    if (currentTab.TabName != currentlySelectedStashOverlayTabName && !string.IsNullOrEmpty(Settings.Default.StashTabOverlayHighlightColor))
                    {
                        currentTab.TabHeaderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor));
                    }
                    else
                    {
                        currentTab.TabHeaderColor = Brushes.Transparent;
                    }

                    SetsToHighlight[0].Items.RemoveAt(0);
                }
            }
        }
    }

    private void PrepareSelling()
    {
        SetsToHighlight.Clear();

        if (_itemSetManagerService == null) return;

        foreach (var s in StashTabControlManager.StashTabControls) s.PrepareOverlayList();
        foreach (var set in _itemSetManagerService.RetrieveSetsInProgress())
        {
            set.OrderItemsForPicking();
            if (set.HasRecipeQualifier)
            {
                SetsToHighlight.Add(new EnhancedItemSet
                {
                    Items = new List<EnhancedItem>(set.Items),
                    EmptyItemSlots = new List<string>(set.EmptyItemSlots)
                });
            }
        }
    }

    private void GenerateReconstructedStashTabsFromApiResponse()
    {
        var reconstructedStashTabs = new List<StashTabControl>();

        if (Settings.Default.StashTabIndices != null) StashTabControlManager.GetStashTabIndices();

        var stashTabMetadataList = _itemSetManagerService.RetrieveStashTabMetadataList();

        // mode = Select Tab From List (default)
        if (Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsFromList)
        {
            if (stashTabMetadataList != null)
            {
                foreach (var tab in stashTabMetadataList.StashTabs)
                {
                    for (var index = StashTabControlManager.StashTabIndices.Count - 1; index > -1; index--)
                    {
                        if (StashTabControlManager.StashTabIndices[index] != tab.Index) continue;

                        StashTabControlManager.StashTabIndices.RemoveAt(index);

                        if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                        {
                            var tabToAdd = new StashTabControl(tab.Name, tab.Index);
                            if (tab.Type == "QuadStash") tabToAdd.Quad = true;
                            reconstructedStashTabs.Add(tabToAdd);
                        }
                    }
                }

                StashTabControlManager.StashTabControls = reconstructedStashTabs;
                ParseAllStashTabNamesFromApiResponse();
            }
        }
        // mode = Individual Stash Tab Prefix
        else if (Settings.Default.StashTabQueryMode == (int)StashTabQueryMode.TabNamePrefix)
        {
            if (stashTabMetadataList != null)
            {
                var individualStashTabPrefix = Settings.Default.StashTabPrefix;

                ParseAllStashTabNamesFromApiResponse();

                foreach (var tab in stashTabMetadataList.StashTabs)
                {
                    if (tab.Name.StartsWith(individualStashTabPrefix))
                    {
                        if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                        {
                            var tabToAdd = new StashTabControl(tab.Name, tab.Index);
                            if (tab.Type == "QuadStash") tabToAdd.Quad = true;
                            reconstructedStashTabs.Add(tabToAdd);

                        }
                    }
                }

                StashTabControlManager.StashTabControls = reconstructedStashTabs;
            }
        }
    }

    private void ParseAllStashTabNamesFromApiResponse()
    {
        var stashTabMetadataList = _itemSetManagerService.RetrieveStashTabMetadataList();
        foreach (var s in StashTabControlManager.StashTabControls)
        {
            foreach (var props in stashTabMetadataList.StashTabs)
            {
                if (s.TabIndex == props.Index)
                {
                    s.TabName = props.Name;
                }
            }
        }
    }

    private StashTabControl GetStashTabFromItem(EnhancedItem itemModel)
    {
        foreach (var s in StashTabControlManager.StashTabControls)
        {
            if (itemModel.StashTabIndex == s.TabIndex) return s;
        }

        return null;
    }
}