using ChaosRecipeEnhancer.UI.Models.FilterSounds;
using NAudio;
using NAudio.Wave;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChaosRecipeEnhancer.UI.Services;

public interface IFilterSoundPackService
{
    List<SoundPack> GetAllSoundPacks();
    void PreviewSound(string bundledFilePath);
    void CopySoundsToFilterFolder(string filterFolderPath, string authorFolder);
}

public class FilterSoundPackService : IFilterSoundPackService, IDisposable
{
    private readonly ILogger _log = Log.ForContext<FilterSoundPackService>();

    private static readonly string CommunitySoundsPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        @"Assets\Sounds\FilterSounds\Community"
    );

    private static readonly Dictionary<string, string> StandardDisplayNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["1maybevaluable.mp3"] = "1 - May be valuable",
        ["2currency.mp3"] = "2 - Currency",
        ["3uniques.mp3"] = "3 - Uniques",
        ["4maps.mp3"] = "4 - Maps",
        ["5highmaps.mp3"] = "5 - High maps",
        ["6veryvaluable.mp3"] = "6 - Very valuable",
        ["7chancing.mp3"] = "7 - Chancing",
        ["12leveling.mp3"] = "12 - Leveling",
        ["placeholder.mp3"] = "Placeholder"
    };

    private AudioResource _previewAudioResource;

    public List<SoundPack> GetAllSoundPacks()
    {
        var packs = new List<SoundPack>();

        if (!Directory.Exists(CommunitySoundsPath))
        {
            _log.Warning("Filter sound pack directory not found at {Path}", CommunitySoundsPath);
            return packs;
        }

        foreach (var authorPath in Directory.GetDirectories(CommunitySoundsPath).OrderBy(path => path))
        {
            var authorName = Path.GetFileName(authorPath);

            if (string.IsNullOrWhiteSpace(authorName))
            {
                continue;
            }

            var category = authorName.Equals("Maven", StringComparison.OrdinalIgnoreCase)
                ? "Game & NPCs"
                : "Community & Streamers";

            var soundPack = new SoundPack
            {
                AuthorName = authorName,
                DisplayName = authorName,
                Category = category,
                Sounds = BuildSoundEntries(authorPath, authorName)
            };

            packs.Add(soundPack);
        }

        return packs;
    }

    public void PreviewSound(string bundledFilePath)
    {
        try
        {
            _previewAudioResource?.Dispose();

            _previewAudioResource = new AudioResource(bundledFilePath);
            _previewAudioResource.Play(1.0f);
        }
        catch (Exception ex) when (ex is IOException or DirectoryNotFoundException or FileNotFoundException or MmException)
        {
            _log.Error(ex, "Failed to preview filter sound: {BundledFilePath}", bundledFilePath);
        }
    }

    public void CopySoundsToFilterFolder(string filterFolderPath, string authorFolder)
    {
        if (string.IsNullOrWhiteSpace(filterFolderPath) || string.IsNullOrWhiteSpace(authorFolder))
        {
            return;
        }

        var sourceFolderPath = Path.Combine(CommunitySoundsPath, authorFolder);
        if (!Directory.Exists(sourceFolderPath))
        {
            _log.Warning("Could not copy filter sounds. Source folder does not exist: {SourceFolderPath}", sourceFolderPath);
            return;
        }

        var targetFolderPath = Path.Combine(filterFolderPath, authorFolder);
        Directory.CreateDirectory(targetFolderPath);

        foreach (var sourceFilePath in Directory.GetFiles(sourceFolderPath))
        {
            var fileName = Path.GetFileName(sourceFilePath);
            var destinationFilePath = Path.Combine(targetFolderPath, fileName);
            File.Copy(sourceFilePath, destinationFilePath, true);
        }
    }

    public void Dispose()
    {
        _previewAudioResource?.Dispose();
    }

    private static List<SoundEntry> BuildSoundEntries(string authorPath, string authorName)
    {
        return Directory
            .GetFiles(authorPath)
            .Where(path => path.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path)
            .Select(path => CreateEntryFromPath(path, StandardDisplayNames.GetValueOrDefault(Path.GetFileName(path), Path.GetFileNameWithoutExtension(path))))
            .ToList();
    }

    private static SoundEntry CreateEntryFromPath(string fullPath)
    {
        return CreateEntryFromPath(fullPath, Path.GetFileNameWithoutExtension(fullPath));
    }

    private static SoundEntry CreateEntryFromPath(string fullPath, string displayName)
    {
        return new SoundEntry
        {
            FileName = Path.GetFileName(fullPath),
            DisplayName = displayName,
            RelativePath = Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, fullPath)
        };
    }
}
