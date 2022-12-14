Update Regarding GGG's [`Do not share POESESSID values with other people`](https://www.pathofexile.com/forum/view-thread/3328601) forum post:

We have used the session ID as a form of making authenticated requests against some of the GGG's in-game data through the use of their APIs. **I will be working to move away from this model and moving to another form of authentication found on other PoE tool apps so that you do not have to input your PoE Session ID.** I'm hoping this can be done by 3.21, if not sooner.

I echo what GGG has claimed, and have always told folks to keep your session IDs secure, not share them with anyone. Treat them as you would your password. This is why we deliberately hide the value in our settings.

**Chaos Recipe Enhancer does not, has not, and will _never_ harvest your session ID or send it to anywhere.**

- Mario

---

<img src="https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/blob/master/DocumentationAssets/CRELogo.png" width="250" alt="Chaos Recipe Enhancer Logo">

# Chaos Recipe Enhancer

``` This app isn't affiliated with or officially endorsed by Grinding Gear Games. ```

This app fetches your stash data from the PoE servers and shows which items you need to pick up to complete the chaos, regal, or exalted shard recipe. When you're ready to vendor a set, we'll highlight the item pick order, which is great when you're pulling from messy tabs. 

Optionally, it can also manipulate your offline loot filter, so you can fully streamline your farm.

[![downloads][downloads-badge]][releases-link]
[![release][releases-badge]][releases-link]
[![issues][issues-badge]][issues-link]
[![Discord][discord-badge]][discord-link]
[![Support][support-badge]][support-link]

## User Guide (Video)

<a href="https://www.youtube.com/watch?v=7umgTuN8bMU">
    <img src="https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/blob/master/DocumentationAssets/User-Guide-Thumbnail.png" width="500" alt="CRE User Guide Video Thumbnail">
</a>

## Installation

1. Make sure you've installed the [.NET Framework Runtime (Version 4.8)][dotnet-framework-link]
2. Download and install the [latest the ChaosRecipeEnhancerSetup.msi][releases-link]

## Additional Features

 - Query from your personal stash or from your guild stash
 - Query multiple stash tabs by ID, prefix, and suffix
 - Customizable hotkeys
 - Customizable overlay position & sizes for different screen resolutions
 - Support for chaos, regal & exalted shard recipes
 - *Offline* loot filter manipulation (online filter syncing not supported)
 - Automatically fetch remaining items when you join a new instance

#### Stash Tab Overlay

<img src="https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/blob/master/DocumentationAssets/Stash-Tab-Overlay.gif" width="500" alt="Stash Tab Overlay">

#### Set Tracker Overlay & Loot Filter Manipulation

<img src="https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/blob/master/DocumentationAssets/Main-Overlay.png" width="500" alt="Set Tracker Overlay & Loot Filter Manipulation">

## Copyright

```
Copyright (C) 2022 Chaos Recipe Enhancer Team

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
```


[downloads-badge]: https://img.shields.io/github/downloads/ChaosRecipeEnhancer/EnhancePoEApp/total?style=for-the-badge&logo=github
[discord-badge]: https://img.shields.io/discord/786617230879883307?color=5865f2&label=Discord&style=for-the-badge&logo=discord&link
[discord-link]: https://discord.gg/ryss9jnRkZ
[releases-badge]: https://img.shields.io/github/v/release/ChaosRecipeEnhancer/EnhancePoEApp?style=for-the-badge&logo=github
[releases-link]: https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/releases
[issues-badge]: https://img.shields.io/github/issues-raw/ChaosRecipeEnhancer/EnhancePoEApp?style=for-the-badge
[issues-link]: https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/issues
[support-badge]: https://img.shields.io/badge/Paypal-Support-<COLOR>?style=for-the-badge&logo=paypal&color=ffae29
[support-link]: https://www.paypal.com/donate/?hosted_button_id=4NDCV5J5NTEWS
[dotnet-framework-link]: https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48
