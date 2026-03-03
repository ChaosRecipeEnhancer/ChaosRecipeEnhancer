using ChaosRecipeEnhancer.UI.Models.FilterSounds;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class FilterSoundPicker : UserControl
{
    // PoE built-in alert sounds with descriptive labels.
    private static readonly List<string> _builtInSounds = new()
    {
        "1 - Generic Alert",
        "2 - Generic Alert (Higher)",
        "3 - Generic Alert (Blip)",
        "4 - Generic Alert (Buzz)",
        "5 - Generic Alert (Pling)",
        "6 - Generic Alert (Big Bling)",
        "7 - Generic Alert (Clash)",
        "8 - Generic Alert (Reverse)",
        "9 - Generic Alert (Short)",
        "10 - Top-Value Currency",
        "11 - Medium-Value Currency",
        "12 - Low-Value Currency",
        "13 - Unique Item",
        "14 - Divination Card",
        "15 - Map Drop",
        "16 - Shaper / Elder / Conqueror"
    };

    private List<SoundPack> _allPacks;
    private bool _suppressCommunitySync;

    public FilterSoundPicker()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadCommunityPacks();
        RestoreCommunitySelectionFromPath();
    }

    private void LoadCommunityPacks()
    {
        try
        {
            var service = Ioc.Default.GetService<IFilterSoundPackService>();
            _allPacks = service?.GetAllSoundPacks() ?? new List<SoundPack>();
        }
        catch
        {
            _allPacks = new List<SoundPack>();
        }
        CommunityPacks = new ObservableCollection<SoundPack>(_allPacks);
    }

    /// <summary>
    /// When loading, if mode is Community and CustomSoundPath has a value like "Asuzara/6veryvaluable.mp3",
    /// restore the pack and sound combo selections to match.
    /// </summary>
    private void RestoreCommunitySelectionFromPath()
    {
        if (SoundMode != 3 || string.IsNullOrEmpty(CustomSoundPath)) return;

        _suppressCommunitySync = true;

        var parts = CustomSoundPath.Replace('\\', '/').Split('/', 2);
        if (parts.Length == 2)
        {
            var authorName = parts[0];
            var fileName = parts[1];

            var pack = _allPacks?.FirstOrDefault(p =>
                p.AuthorName.Equals(authorName, StringComparison.OrdinalIgnoreCase));
            if (pack != null)
            {
                CommunityPackCombo.SelectedItem = pack;
                CurrentPackSounds = new ObservableCollection<SoundEntry>(pack.Sounds);

                var sound = pack.Sounds.FirstOrDefault(s =>
                    s.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (sound != null)
                {
                    CommunitySoundCombo.SelectedItem = sound;
                }
            }
        }

        _suppressCommunitySync = false;
    }

    #region Public Properties

    public List<string> BuiltInSounds => _builtInSounds;

    public static readonly DependencyProperty CommunityPacksProperty =
        DependencyProperty.Register(nameof(CommunityPacks), typeof(ObservableCollection<SoundPack>), typeof(FilterSoundPicker),
            new PropertyMetadata(new ObservableCollection<SoundPack>()));

    public ObservableCollection<SoundPack> CommunityPacks
    {
        get => (ObservableCollection<SoundPack>)GetValue(CommunityPacksProperty);
        set => SetValue(CommunityPacksProperty, value);
    }

    public static readonly DependencyProperty CurrentPackSoundsProperty =
        DependencyProperty.Register(nameof(CurrentPackSounds), typeof(ObservableCollection<SoundEntry>), typeof(FilterSoundPicker),
            new PropertyMetadata(new ObservableCollection<SoundEntry>()));

    public ObservableCollection<SoundEntry> CurrentPackSounds
    {
        get => (ObservableCollection<SoundEntry>)GetValue(CurrentPackSoundsProperty);
        set => SetValue(CurrentPackSoundsProperty, value);
    }

    #endregion

    #region SoundMode DP

    public static readonly DependencyProperty SoundModeProperty =
        DependencyProperty.Register(nameof(SoundMode), typeof(int), typeof(FilterSoundPicker),
            new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSoundModeChanged));

    public int SoundMode
    {
        get => (int)GetValue(SoundModeProperty);
        set => SetValue(SoundModeProperty, value);
    }

    #endregion

    #region SoundId DP

    public static readonly DependencyProperty SoundIdProperty =
        DependencyProperty.Register(nameof(SoundId), typeof(int), typeof(FilterSoundPicker),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSoundIdChanged));

    public int SoundId
    {
        get => (int)GetValue(SoundIdProperty);
        set => SetValue(SoundIdProperty, value);
    }

    public static readonly DependencyProperty SoundIdIndexProperty =
        DependencyProperty.Register(nameof(SoundIdIndex), typeof(int), typeof(FilterSoundPicker),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSoundIdIndexChanged));

    public int SoundIdIndex
    {
        get => (int)GetValue(SoundIdIndexProperty);
        set => SetValue(SoundIdIndexProperty, value);
    }

    private static bool _suppressSoundIdSync;

    private static void OnSoundIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (_suppressSoundIdSync) return;
        if (d is FilterSoundPicker picker)
        {
            _suppressSoundIdSync = true;
            picker.SoundIdIndex = Math.Clamp((int)e.NewValue - 1, 0, _builtInSounds.Count - 1);
            _suppressSoundIdSync = false;
        }
    }

    private static void OnSoundIdIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (_suppressSoundIdSync) return;
        if (d is FilterSoundPicker picker)
        {
            _suppressSoundIdSync = true;
            picker.SoundId = (int)e.NewValue + 1;
            _suppressSoundIdSync = false;
        }
    }

    #endregion

    #region SoundVolume DP

    public static readonly DependencyProperty SoundVolumeProperty =
        DependencyProperty.Register(nameof(SoundVolume), typeof(int), typeof(FilterSoundPicker),
            new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public int SoundVolume
    {
        get => (int)GetValue(SoundVolumeProperty);
        set => SetValue(SoundVolumeProperty, value);
    }

    #endregion

    #region CustomSoundPath DP

    public static readonly DependencyProperty CustomSoundPathProperty =
        DependencyProperty.Register(nameof(CustomSoundPath), typeof(string), typeof(FilterSoundPicker),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string CustomSoundPath
    {
        get => (string)GetValue(CustomSoundPathProperty);
        set => SetValue(CustomSoundPathProperty, value);
    }

    #endregion

    #region Read-only computed mode flags

    private static readonly DependencyPropertyKey IsNormalSoundModePropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsNormalSoundMode), typeof(bool), typeof(FilterSoundPicker),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IsNormalSoundModeProperty = IsNormalSoundModePropertyKey.DependencyProperty;

    public bool IsNormalSoundMode
    {
        get => (bool)GetValue(IsNormalSoundModeProperty);
        private set => SetValue(IsNormalSoundModePropertyKey, value);
    }

    private static readonly DependencyPropertyKey IsCustomSoundModePropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsCustomSoundMode), typeof(bool), typeof(FilterSoundPicker),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IsCustomSoundModeProperty = IsCustomSoundModePropertyKey.DependencyProperty;

    public bool IsCustomSoundMode
    {
        get => (bool)GetValue(IsCustomSoundModeProperty);
        private set => SetValue(IsCustomSoundModePropertyKey, value);
    }

    private static readonly DependencyPropertyKey IsCommunitySoundModePropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsCommunitySoundMode), typeof(bool), typeof(FilterSoundPicker),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IsCommunitySoundModeProperty = IsCommunitySoundModePropertyKey.DependencyProperty;

    public bool IsCommunitySoundMode
    {
        get => (bool)GetValue(IsCommunitySoundModeProperty);
        private set => SetValue(IsCommunitySoundModePropertyKey, value);
    }

    private static void OnSoundModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FilterSoundPicker picker)
        {
            var mode = (int)e.NewValue;
            picker.IsNormalSoundMode = mode == 1;
            picker.IsCustomSoundMode = mode == 2;
            picker.IsCommunitySoundMode = mode == 3;

            // When switching to Community mode, restore selection from CustomSoundPath
            if (mode == 3)
            {
                picker.RestoreCommunitySelectionFromPath();
            }
        }
    }

    #endregion

    #region Community Sound Events

    private void OnCommunityPackChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CommunityPackCombo.SelectedItem is SoundPack pack)
        {
            CurrentPackSounds = new ObservableCollection<SoundEntry>(pack.Sounds);

            // Auto-select first sound in the pack
            if (pack.Sounds.Count > 0 && !_suppressCommunitySync)
            {
                CommunitySoundCombo.SelectedIndex = 0;
            }
        }
    }

    private void OnCommunitySoundChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressCommunitySync) return;

        if (CommunityPackCombo.SelectedItem is SoundPack pack &&
            CommunitySoundCombo.SelectedItem is SoundEntry sound)
        {
            // Write the relative path that the filter will use: "AuthorName/filename.mp3"
            CustomSoundPath = $"{pack.AuthorName}/{sound.FileName}";
        }
    }

    #endregion

    #region Test Sound Buttons

    private void OnTestNormalSoundClick(object sender, RoutedEventArgs e)
    {
        var id = SoundId;
        if (id < 1 || id > 16) return;

        try
        {
            var builtInPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"Assets\Sounds\FilterSounds\BuiltIn",
                $"AlertSound{id}.mp3");

            if (File.Exists(builtInPath))
            {
                var service = Ioc.Default.GetService<IFilterSoundPackService>();
                service?.PreviewSound(builtInPath);
            }
        }
        catch
        {
            // Preview is best-effort
        }
    }

    private void OnTestCustomSoundClick(object sender, RoutedEventArgs e)
    {
        PreviewFromCustomSoundPath();
    }

    private void OnTestCommunitySoundClick(object sender, RoutedEventArgs e)
    {
        PreviewFromCustomSoundPath();
    }

    private void PreviewFromCustomSoundPath()
    {
        var path = CustomSoundPath;
        if (string.IsNullOrWhiteSpace(path)) return;

        try
        {
            // Try bundled community sounds first
            var bundledPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"Assets\Sounds\FilterSounds\Community",
                path);

            if (File.Exists(bundledPath))
            {
                var service = Ioc.Default.GetService<IFilterSoundPackService>();
                service?.PreviewSound(bundledPath);
                return;
            }

            // Fall back to raw path
            var resolvedPath = Path.IsPathRooted(path)
                ? path
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            if (File.Exists(resolvedPath))
            {
                var service = Ioc.Default.GetService<IFilterSoundPackService>();
                service?.PreviewSound(resolvedPath);
            }
        }
        catch
        {
            // Preview is best-effort
        }
    }

    #endregion
}
