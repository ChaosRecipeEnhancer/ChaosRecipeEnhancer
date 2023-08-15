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
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class StashTabOverlayWindow
{
    private readonly IItemSetManagerService _itemSetManagerService = Ioc.Default.GetService<IItemSetManagerService>();
    private List<EnhancedItemSet> ItemSetListHighlight { get; } = new List<EnhancedItemSet>();

    private readonly StashTabOverlayViewModel _model;

    public StashTabOverlayWindow()
    {
        InitializeComponent();
        DataContext = _model = new StashTabOverlayViewModel();

        StashTabOverlayTabControl.ItemsSource = _model.OverlayStashTabList;

        MouseHook.MouseAction += OnMouseHookClick;
    }

    #region Pre-Fuckup

    public bool IsOpen { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Win32.MakeToolWindow(this);
    }

    // public new virtual void Hide()
    // {
    //     if (!IsOpen) return;
    //
    //     MakeWindowClickThrough(true);
    //     _model.IsEditing = false;
    //     MouseHook.Stop();
    //
    //     IsOpen = false;
    //     base.Hide();
    // }

    // public new virtual void Show()
    // {
    //     IsOpen = true;
    //
    //     MouseHook.Start();
    //     base.Show();
    // }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        MakeWindowClickThrough(true);
    }

    private void OnMouseHookClick(object sender, MouseHookEventArgs e)
    {
        if (!IsOpen) return;
        Coordinates.OverlayClickEvent(this);
        if (ControlHelpers.HitTest(EditModeButton, e.ClickLocation)) HandleEditButton();
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

    #endregion

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

            // If "All Items" highlight mode enabled, paint all Stash Tab Headers to their respective colors
            if (Settings.Default.StashTabOverlayHighlightMode == 2)
            {
                foreach (var set in ItemSetListHighlight)
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

    private StashTabControl GetStashTabFromItem(EnhancedItem itemModel)
    {
        foreach (var s in StashTabControlManager.StashTabControls)
        {
            if (itemModel.StashTabIndex == s.TabIndex) return s;
        }

        return null;
    }

    public void ActivateNextCell(bool active, InteractiveStashTabCell stashTabCell, TabControl tabControl)
    {
        if (!active) return;

        var currentlySelectedStashOverlayTabName = tabControl != null
            ? ((TextBlock)((HeaderedContentControl)tabControl.SelectedItem).Header).Text
            : "";

        // activate set by set
        if (Settings.Default.StashTabOverlayHighlightMode == 1)
        {
            if (ItemSetListHighlight.Count > 0)
            {
                // check for full sets
                if (ItemSetListHighlight[0].EmptyItemSlots.Count == 0)
                {
                    if (stashTabCell != null)
                    {
                        var highlightItem = stashTabCell.ItemModel;
                        var currentTab = GetStashTabFromItem(highlightItem);

                        if (currentTab != null)
                        {
                            currentTab.TabHeaderColor = Brushes.Transparent;
                            currentTab.DeactivateSingleItemCells(stashTabCell.ItemModel);
                            ItemSetListHighlight[0].Items.Remove(highlightItem);
                        }
                    }

                    foreach (var i in ItemSetListHighlight[0].Items.ToList())
                    {
                        var currTab = GetStashTabFromItem(i);
                        currTab.ActivateItemCells(i);
                    }

                    // mark item order
                    if (ItemSetListHighlight[0] != null)
                    {
                        if (ItemSetListHighlight[0].Items.Count > 0)
                        {
                            var currentStashTab = GetStashTabFromItem(ItemSetListHighlight[0].Items[0]);
                            currentStashTab.MarkNextItem(ItemSetListHighlight[0].Items[0]);
                            currentStashTab.TabHeaderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor));
                        }
                    }

                    // Set has been completed
                    if (ItemSetListHighlight[0].Items.Count == 0)
                    {
                        ItemSetListHighlight.RemoveAt(0);

                        // activate next set
                        ActivateNextCell(true, null, null);
                    }
                }
            }
        }
    }

    public void PrepareSelling()
    {
        ItemSetListHighlight.Clear();

        // if (_apiService.IsFetching) return;

        if (_itemSetManagerService == null) return;

        foreach (var s in StashTabControlManager.StashTabControls) s.PrepareOverlayList();
        foreach (var itemSet in _itemSetManagerService.RetrieveSetsInProgress()) itemSet.OrderItemsForPicking();

        foreach (var set in _itemSetManagerService.RetrieveSetsInProgress())
        {
            if (set.HasRecipeQualifier)
            {
                ItemSetListHighlight.Add(new EnhancedItemSet
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

        // mode = Individual Stash Tab Indices
        if (Settings.Default.StashTabQueryMode == 0)
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
        else if (Settings.Default.StashTabQueryMode == 1)
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
                            reconstructedStashTabs.Add(new StashTabControl(tab.Name, tab.Index));
                    }
                }

                StashTabControlManager.StashTabControls = reconstructedStashTabs;
            }
        }
        // mode = Individual Stash Tab Suffix
        else if (Settings.Default.StashTabQueryMode == 2)
        {
            if (stashTabMetadataList != null)
            {
                var individualStashTabSuffix = Settings.Default.StashTabSuffix;

                ParseAllStashTabNamesFromApiResponse();

                foreach (var tab in stashTabMetadataList.StashTabs)
                {
                    if (tab.Name.EndsWith(individualStashTabSuffix))
                    {
                        if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                        {
                            reconstructedStashTabs.Add(new StashTabControl(tab.Name, tab.Index));
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
}