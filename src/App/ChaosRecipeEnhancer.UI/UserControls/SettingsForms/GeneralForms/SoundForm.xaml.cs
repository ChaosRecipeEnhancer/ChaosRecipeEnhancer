// using System;
// using System.IO;
// using System.Reflection;
// using System.Windows;
// using System.Windows.Forms;
// using System.Windows.Input;
// using ChaosRecipeEnhancer.UI.BusinessLogic.Constants;
// using ChaosRecipeEnhancer.UI.Properties;
//
// namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;
//
// public partial class SoundForm
// {
//     private readonly SoundFormViewModel _model;
//     
//     public SoundForm()
//     {
//         InitializeComponent();
//
//         DataContext = _model = new SoundFormViewModel();
//         
//         // If the user has changed the sound file location for the filter modification sound, open that file
//         if (!string.IsNullOrEmpty(Settings.Default.FilterModificationPendingSoundFileLocation) &&
//             !FilterSoundLocationDialog.Content.Equals("Default Sound"))
//             Data.Player.Open(new Uri(Settings.Default.FilterModificationPendingSoundFileLocation));
//         // Else, open our default sound file location for the included `FilterChanged.mp3` file
//         else
//             Data.Player.Open(new Uri(Path.Combine(
//                 Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
//                 SoundAssets.DefaultFilterChangedSoundFilePath)));
//
//         // If the user has changed the sound file location for the item set completed sound, open that file
//         if (!string.IsNullOrEmpty(Settings.Default.ItemSetCompletedSoundFileLocation) &&
//             !ItemPickupLocationDialog.Content.Equals("Default Sound"))
//             Data.PlayerSet.Open(new Uri(Settings.Default.ItemSetCompletedSoundFileLocation));
//         // Else, open our default sound file location for the included `ItemPickedUp.mp3` file
//         else
//             Data.PlayerSet.Open(new Uri(Path.Combine(
//                 Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
//                 SoundAssets.DefaultItemPickedUpSoundFilePath)));
//     }
//
//     private void VolumeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
//     {
//         Data.PlayNotificationSound();
//     }
//
//     private void FilterSoundLocationDialog_OnClick(object sender, RoutedEventArgs e)
//     {
//         var soundFilePath = GetSoundFilePath();
//
//         if (soundFilePath == null) return;
//
//         Settings.Default.FilterModificationPendingSoundFileLocation = soundFilePath;
//         FilterSoundLocationDialog.Content = soundFilePath;
//         
//         Data.Player.Open(new Uri(soundFilePath));
//         Data.PlayNotificationSound();
//     }
//
//     private void ItemPickupLocationDialog_OnClick(object sender, RoutedEventArgs e)
//     {
//         var soundFilePath = GetSoundFilePath();
//
//         if (soundFilePath == null) return;
//
//         Settings.Default.FilterModificationPendingSoundFileLocation = soundFilePath;
//
//         ItemPickupLocationDialog.Content = soundFilePath;
//
//         Data.PlayerSet.Open(new Uri(soundFilePath));
//         Data.PlayNotificationSoundSetPicked();
//     }
// }

