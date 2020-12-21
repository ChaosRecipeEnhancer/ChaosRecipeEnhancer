# The PoE Chaos Recipe Enhancer

This App fetches data from PoE servers and shows which items you need to pick up for the Chaos Recipe. Optionally it manipulates your lootfilter, so you can go full braindead while farming! While selling your stuff it shows you what items to put in your inventory in which order. No more using your brain at all!

For Feedback or Bug Reports send me an E-Mail to kosacewebdev@gmail.com or spam this [discord](https://discord.gg/KgVsUdSSrR).

#### Current Version 1.0.8 - Major Changes

For details on changes, check the bottom of this site.

## Installation

DO NOT download the whole github repository, since it may include some new experimental features.

[Download it here](https://github.com/kosace/EnhancePoEApp/raw/master/ChaosRecipeEnhancerSetup.msi) instead. 

Since this app is not trusted (too expensive), you will have to accept few times to install (Click on "More info" when Windows wants to protect your PC). 

## Features

 - Easy to use
 - Multiple Stashtabs
 - Hotkeys
 - Customizable Overlays
 - Stashtab Overlay for easy selling
 - Exalted Shard Recipe
 - Lootfilter Manipulation (optional)

#### Main Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/enhancepoescreen2.png" width="500">

#### Stashtab Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/EnhancePoEscreen1.png" width="500">

#### The Settings

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/Screenshot 2020-12-20 234313.png" width="500">

## Guide

#### Compatibility

The overlays will not work in Fullscreen Mode. Instead they will only work in:
 - DirectX11: Windowed or Fullscreen Windowed Mode
 - Vulkan: Windowed Mode or Fullscreen Windowed Mode (tell me, if there are problems with it)
 
The tool only works in Windows, no support for other OS.

#### Main Overlay

The Main Overlay shows each itemtype with its color and while fetching data it shows a loading bar. When you have enough items of a specific itemtype the image will grey out. If you have Lootfilter Manipulation activated, this is the time to refresh your lootfilter manually ingame. Also if you have sound activated, it will play to remember you to refresh your lootfilter.

You can just drag the Main Overlay whereever you want, except if you set the opacity to 0 (better set it to 0.01).

 - Show Button: shows/ hides the StashTab Overlay
 - Edit Button: makes the StashTab Overlay clickable and drawable

#### Stashtab Overlay

The Stashtab Overlay highlights items in a specific order if you have full sets in a stashtab. No more wondering which part is missing when selling. Also this way you can put 2 sets in one inventory. Leave it open when selling! Otherwise the highlighting will start from beginning with items you already sold.

If you want to change position or size of the Stashtabs Overlay, you have to press the "Edit" button on the Main Overlay. Then you can drag the Stashtabs Overlay around and resize it (bottom right corner), except if you set the opacity to 0 (better set it to 0.01). The TabHeader position and sizes can be modified in the Settings Page.

#### Lootfilter

You can use any lootfilter you want. If you don't have the recognition phrase in your lootfilter, the recipe rules will be added to the top of your existing lootfilter (which overwrites some rules, working on a fix). When the recognition phrase is added, the app will only change the styles in within the phrases. That means you can decide where the recipe rules should be modified.

Don't forget to refresh your lootfilter ingame everytime the itemtypes change!

###### Recoginition Phrases:
 - Chaos Recipe Start: "#Chaos Recipe Enhancer by kosace Chaos Recipe Start"
 - Chaos Recipe End: "#Chaos Recipe Enhancer by kosace Chaos Recipe End"
 
 - Exalted Recipe Start: "#Chaos Recipe Enhancer by kosace Exalted Recipe Start"
 - Exalted Recipe End: "#Chaos Recipe Enhancer by kosace Exalted Recipe End"
 
Don't "cross" them, meaning if you start one section end it before starting the other one.

You can change the look of the Chaos Recipe Section in "C:\Users\username\AppData\Roaming\ChaosRecipeEnhancer\Styles\NormalItemsStyle.txt". 

You can change the look of the Exalted Recipe Section in "C:\Users\username\AppData\Roaming\ChaosRecipeEnhancer\Styles\InfluencedItemsStyle.txt"

Unfortunately there is no possibility to automatically refresh your lootfilter ingame (that are legal), so you will have to do that manually (2 clicks).

### Settings

#### Account Name

The name of your PoE account (not the character)

#### PoE Session ID

Login at PoE website and copy it from your cookies (press F12). Everytime you relog at the website your SessionID gets refreshed. You can find an easy to use tutorial [here](http://www.vhpg.com/how-to-find-poe-session-id/). To prevent leaking your SessionID, it will only show dots.

Don't share your SessionID with anyone.

#### League

The league you are playing in.

- Normal: just the league name. e.g. "Standard" or "Heist"
- SSF: SSF + the league name e.g. "SSF Standard" or "SSF Heist"
- Hardcore: just "Hardcore"
- Event: the name under the character on poe website e.g. for Mayhem it was "Mayhem (DRE001)"

#### Full Set Threshold

This is the amount of sets you want to gather. The lootfilter will hide items depending on this setting. For example: if you want 5 sets and have 5 body armours already, the lootfilter will hide body armours. If you have this number of sets full, you should sell, otherwise every item will be hidden (except for jewellery). 

#### Refreshrate

The time in seconds the tool waits between fetching the data. The tool will ask the servers every x seconds, no matter if you are in a map or in your hideout. The minimum time is 15 seconds, but I recommend setting it higher, especially if you have many stashtabs configured.

#### Fill Greedy

The tool tries to fit at least 1 item with ilvl 60-74 in every set. Then it tries to fill the rest of the slots with higher ilvl items. When Fill Greedy is activated it will then fill the remaining slots with lower ilvl items again. When deactivated it will only allow 1 lower ilvl item in each set, that means it will save lower ilvl items for the next set. So you are more efficient when you are deactivating it while playing high tier maps.

#### Exalted Shard Recipe

When activated the tool will also look for influenced unidentified rare itemsets, but it will never hide influenced items. When you have a full set, it will tell you in the Main Overlay.

#### ...

#### Exalted Shard Recipe

If you have Exalted Shard Recipe activated, every influenced rare item will be highlighted no matter the ilvl. Also it doesn't hide influenced rares when you have full sets. Means, influenced items are always shown.

Exalted Shard Sets will still count as "Full Sets" and the Selling algorithm should still work (it shows you both recipes).

This feature is not tested too much, tell me if you find bugs!

##  F.A.Q.
#### How do I get my SessionID?


#### Is this tool safe?
You can check and compile the sourcecode for yourself, so it should be safe as long as you are downloading it from here.

#### Can I get banned for using this tool?
Well, I contacted GGG but as usual they don't answer any questions regarding legality of 3rd party tools. As this tool does not interfer with any game files directly, the answer is: probably no!

Although there are 2 points which could be problematic:
1. The tool fetches data from PoE servers repeatedly, so they could think you are stressing the servers too much. That is why I limited the refreshrate a bit. One request per one second. 
2. The tool manipulates your lootfilter. I'm not sure if they are considered "game files" by GGG, personally I would'nt consider them like that. The tool certainly doesn't interact with the game client in any way. But if you are unsure you can deactivate this feature and only use the overlay. 

## Attributions

#### ItemIcons

  - Icons made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com/)
  
  - Icons made by [Smashicons](https://www.flaticon.com/authors/smashicons) from [www.flaticon.com](https://www.flaticon.com/)
  
  - Icons made by [iconixar](https://www.flaticon.com/authors/iconixar) from [www.flaticon.com](https://www.flaticon.com/)
  
 
## Change Log

#### 1.0.8

 - grouped Settings by category
 - removed Save Button, everything should save automatically
 - removed individual stashtabs
 - added stashtab mode, for easier stashtab adding
 - now automatically detects quad tabs
 - now automatically detects stashtab names/ IDs
 - added distance algorithm, the tool prefers items close together
 - added highlight mode
 - added sound when full set is picked up for selling
 - added support for every itemlevel, now you can mix higher and lower ilvl items
 - added fill greedy mode, you can decide if there should only be one lower ilvl item in your sets or more
 - removed bases, works with classes now
 - removed the option for 2 hand weapons, now every 2 hand weapon with size 2x3 and every 1 hand weapon with size 1x3 will be allowed
 - added initial position and size of Stashtab Overlay optimized for full hd
 - added password font in SessionID field, no more leaking your ID
 - updated guide

#### 1.0.7

 - minor UI changes
 - added colors for jewellery
 - fixed crash occuring when sound notification is activated

#### 1.0.6

 - fixed lootfilter parsing bug (should recognize the phrases now correctly)
 - added default values
 - adjusted stashtab overlay highlighting sizes
 - added bows and 1h maces
 
#### 1.0.5

 - added Two-Hand Weapon support
 - added Custom Style support
 - added Exalted Shard Recipe (Read guide)
 - changed lootfilter parsing
 - added sound when lootfilter changes
 - added custom tabheader width for aligning tab headers to game
  
  
  
  
  
  
