# The PoE Chaos Recipe Enhancer

This App fetches data from PoE servers and shows which items you need to pick up for the Chaos Recipe. Optionally it manipulates your lootfilter, so you can go full braindead while farming! While selling your stuff it shows you what items to put in your inventory in which order. No more using your brain at all!

For Feedback or Bug Reports send me an E-Mail to kosacewebdev@gmail.com or spam this [discord](https://discord.gg/KgVsUdSSrR).

#### Updates

 - added Two-Hand Weapon support
 - added Custom Style support
 - added Exalted Shard Recipe (Read guide)
 - changed lootfilter parsing

#### Main Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/enhancepoescreen2.png" width="500">

#### Stashtab Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/EnhancePoEscreen1.png" width="500">

#### The Settings

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/enhancepoescreen3.png" width="500">

## Features

 - Easy to use
 - Multiple Stashtabs
 - Hotkeys
 - Customizable Overlay
 - Stashtab Overlay for easy selling
 - Exalted Shard Recipe
 - Lootfilter Manipulation (optional)


## Installation

[Download](https://github.com/kosace/EnhancePoEApp/raw/master/ChaosRecipeEnhancerSetup.msi) and install. 

Since this app is not trusted (too expensive), you will have to accept few times to install (Click on "More info" when Windows wants to protect your PC). 

## Guide

#### Compatibility

The overlays will not work in Fullscreen Mode. Instead they will only work in:
 - DirectX11: Windowed or Fullscreen Windowed Mode
 - Vulkan: Windowed Mode
 
The tool only works in Windows, no support for other OS.

#### Main Overlay

The Main Overlay shows each itemtype with its color and while fetching data it shows a loading bar. When you have enough items of a specific itemtype the image will grey out. If you have Lootfilter Manipulation activated, this is the time to refresh your lootfilter ingame. 

You can just drag the Main Overlay whereever you want, except if you set the opacity to 0 (better set it to 0.01).

#### Stashtab Overlay

The Stashtab Overlay highlights items in a specific order if you have full sets in a stashtab. No more wondering which part is missing when selling. Also this way you can put 2 sets in one inventory. Leave it open when selling! Otherwise the highlighting will start from beginning with items you already sold.

If you want to change position or size of the Stashtabs Overlay, you have to press the "Edit" button on the Main Overlay. Then you can drag the Stashtabs Overlay around and resize it (bottom right corner), except if you set the opacity to 0 (better set it to 0.01). At the moment the tabheader sizes are optimized for full hd. I will add support for manually editing these later.

#### Stashtabs Order

You have to specify the number of your stashtab from left to right starting from 0. That means your 1st stashtab is 0, your 2nd is 1 and so on. It seems that folders count as 1 stashtab. At the moment it is not possible to fetch items in folders (tell me if you know how).

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

#### Bases

If you want to change the bases you pick up, navigate to "C:\Users\username\AppData\Roaming\ChaosRecipeEnhancer\Bases", and edit the .txt files accordingly. This way you can still use this tool when new bases are released or if you want to pick up bigger weapons too. At the moment identified items will get ignored.

Bows and Quivers are not supported. Tell me, if you want support for them.

You have to restart the app after changing the files.

The list of bases may include some old bases and too big weapons at the moment. If you have a up-to-date list of all bases and small weapons (1x3), send me an E-Mail please. 
Also Sai in weapon bases produces a bug where some body armours get highlighted, so i deleted Sai.

#### Exalted Shard Recipe

If you have Exalted Shard Recipe activated, every influenced rare item will be highlighted no matter the ilvl. Also it doesn't hide influenced rares when you have full sets. Means, influenced items are always shown.

Exalted Shard Sets will still count as "Full Sets" and the Selling algorithm should still work (it shows you both recipes).

This feature is not tested too much, tell me if you find bugs!

##  F.A.Q.
#### How do I get my SessionID?
Login at PoE website and copy it from your cookies. Everytime you relog at the website your SessionID gets refreshed. You can find an easy to use tutorial [here](http://www.vhpg.com/how-to-find-poe-session-id/).

Don't share your SessionID with anyone.

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
  
  
  
  
  
  
