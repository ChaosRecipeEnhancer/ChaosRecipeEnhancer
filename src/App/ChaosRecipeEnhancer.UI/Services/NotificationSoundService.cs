using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using NAudio;
using NAudio.Wave;
using Serilog;
using System;
using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Services;

public interface INotificationSoundService
{
    void PlayNotificationSound(NotificationSoundType soundType);
}

public class NotificationSoundService : INotificationSoundService, IDisposable
{
    private readonly ILogger _log = Log.ForContext<NotificationSoundService>();
    private readonly IUserSettings _userSettings;
    private readonly Dictionary<NotificationSoundType, AudioResource> _soundPlayers;

    public NotificationSoundService(IUserSettings userSettings)
    {
        _userSettings = userSettings;
        _soundPlayers = InitializeSoundPlayers();
    }

    public void PlayNotificationSound(NotificationSoundType soundType)
    {
        if (_userSettings.SoundEnabled)
        {
            _log.Information("Playing notification sound: {SoundType} - Sound level: {SoundLevel}", soundType, _userSettings.SoundLevel);
            if (_soundPlayers.TryGetValue(soundType, out var audioResource))
            {
                audioResource.Play((float)_userSettings.SoundLevel);
            }
        }
    }

    private Dictionary<NotificationSoundType, AudioResource> InitializeSoundPlayers()
    {
        Dictionary<NotificationSoundType, AudioResource> players = [];

        try
        {
            if (_userSettings.SoundEnabled)
            {
                players = new Dictionary<NotificationSoundType, AudioResource>
                {
                    { NotificationSoundType.ItemSetStateChanged, new AudioResource(@"Assets\Sounds\ItemSetStateChanged.wav") },
                    { NotificationSoundType.SetPickingComplete, new AudioResource(@"Assets\Sounds\SetPickingComplete.wav") },
                    { NotificationSoundType.FilterReloaded, new AudioResource(@"Assets\Sounds\FilterReloaded.wav") },
                };
            }
        }
        catch (MmException ex)
        {
            // Log the error
            _log.Error($"Failed to initialize audio devices. Exception: {ex.Message}");

            // Notify the user of the error and offer options (this part depends on your UI implementation)
            GlobalErrorHandler.Spawn(
                ex.ToString(),
                "Error: Audio Device Initialization",
                "Failed to initialize audio devices. This is usually caused by not having a default sound device selected, or a sound driver issue. Disabling sound to prevent further errors."
            );

            // Disabling sound to prevent further errors
            _userSettings.SoundEnabled = false;
        }

        return players;
    }

    #region IDisposable Support

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                foreach (var player in _soundPlayers.Values)
                {
                    player.Dispose();
                }
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer.
            // Set large fields to null.
            _disposed = true;
        }
    }

    public void Dispose()
    {
        foreach (var soundPlayer in _soundPlayers.Values)
        {
            soundPlayer.Dispose();
        }
    }


    #endregion
}

public class AudioResource : IDisposable
{
    private readonly string _filePath;
    public AudioFileReader FileReader { get; private set; }
    public WaveOutEvent OutputDevice { get; private set; }

    public AudioResource(string filePath)
    {
        _filePath = filePath;
        FileReader = new AudioFileReader(filePath);
        OutputDevice = new WaveOutEvent();
        OutputDevice.Init(FileReader);
    }

    public void Play(float volume)
    {
        if (OutputDevice.PlaybackState == PlaybackState.Playing)
        {
            OutputDevice.Stop();
        }

        // Dispose and recreate FileReader and OutputDevice for every play to ensure clean state
        FileReader.Dispose();
        OutputDevice.Dispose();

        FileReader = new AudioFileReader(_filePath)
        {
            Volume = volume
        };

        OutputDevice = new WaveOutEvent();
        OutputDevice.Init(FileReader);
        OutputDevice.Play();
    }

    public void Dispose()
    {
        // Ensure to stop the playback before disposing
        if (OutputDevice.PlaybackState != PlaybackState.Stopped)
        {
            OutputDevice.Stop();
        }

        OutputDevice?.Dispose();
        FileReader?.Dispose();
    }
}
