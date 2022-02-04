# The PoE Chaos Recipe Enhancer

This App fetches data from PoE servers and shows which items you need to pick up for the Chaos Recipe. Optionally it manipulates your loot filter, so you can go full brain-dead while farming! While selling your stuff it shows you what items to put in your inventory in which order. No more using your brain at all!

For Feedback or Bug Reports spam this [discord](https://discord.gg/KgVsUdSSrR) or open an Issue on github.

#### Current Version 1.3.1

This product isn't affiliated with or endorsed by Grinding Gear Games in any way.

For details on changes, check the Change Log at the bottom of this site.

## Support

With Kosace's permission, I will be changing the PayPal link and all donations will be going towards giveaways happening every league. I am blessed enough to have a solid job as a developer and do this only to give back to my community, so this could be your chance to give back, too!

[![Donate with PayPal](https://raw.githubusercontent.com/kosace/EnhancePoEApp/master/Webp.net-resizeimage(5).png)](https://www.paypal.com/donate/?hosted_button_id=4NDCV5J5NTEWS)

## Installation

_**_NOTE: We will be updating the installation / setup guides in the near future to ensure you're up and running quicker._**_

DO NOT download the whole github repository, since it may include some new experimental features.

[Download the setup.msi here](https://github.com/kosace/EnhancePoEApp/releases) instead. 

Since this app is not trusted (too expensive), you will have to accept few times to install (Click on "More info" when Windows wants to protect your PC). 

If you have updated your application to 1.2.0 please use a new item filter as some changes may break your existing one! Make sure to back up any custom filters you want our program to interact with in case of any [potentially unwanted behavior!](https://github.com/kosace/EnhancePoEApp/issues/106)

## Features

 - Easy to use
 - Multiple Stash Tabs
 - Hotkeys
 - Customizable Overlays
 - Stash Tab Overlay for easy selling
 - Chaos Recipe
 - Regal Recipe
 - Exalted Shard Recipe
 - Loot filter Manipulation (optional)
 - Automatically fetches when you join a new map

#### Main Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/enhancepoescreen2.png" width="500" alt="Main Overlay">

#### Stash Tab Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/EnhancePoEscreen1.png" width="500" alt="Stash Tab Overlay">

#### The Settings

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/Screenshot 2020-12-20 234313.png" width="500" alt="Settings Page">

## Guide

#### Compatibility

The overlays will not work in Fullscreen Mode. They will only work in:
 - Windowed Mode
 - Fullscreen Windowed Mode
 
The tool has been tested with Windows 10/11, we have no plans to officially support other operating systems.

Overlay only works with Display Scaling 100% in Windows Settings (working on a fix). 

#### Main Overlay

The Main Overlay shows each item type with its color and while fetching data it shows a loading bar. When you have enough items of a specific item type the image will grey out. If you have loot filter manipulation activated, this is the time to refresh your loot filter manually in-game. Also if you have sound activated, it will play to remember you to refresh your loot filter.

You can just drag the Main Overlay wherever you want, except if you set the opacity to 0 (better set it to 0.01).

 - Show Button: shows/ hides the Stash Tab Overlay
 - Fetch Button: starts fetching items from PoE servers periodically
 
There will be warning displayed in this overlay also:

 - Warning if you have full sets, but need lower item level items (item levels 60-74)
 - Warning if you have full sets and need to sell them
 - Warning if you have full exalted shard recipe set
 - Warning if you are temporarily banned from fetching from PoE servers
 - Warning if you exceeded the rate limit

#### Stash Tab Overlay

The stash tab Overlay highlights items in a specific order if you have full sets in a stash tab. No more wondering which part is missing when selling. Also this way you can put 2 sets in one inventory. Leave it open when selling! Otherwise the highlighting will start from beginning with items you already sold.

If you want to change position or size of the stash tabs Overlay, you have to press the "Edit" button on the Main Overlay. Then you can drag the Stash tabs Overlay around and resize it (bottom right corner), except if you set the opacity to 0 (better set it to 0.01). The TabHeader position and sizes can be modified in the Settings Page.

At the top of the overlay you can find the

 - Edit Button: makes the Stash Tab Overlay clickable and draggable

#### Loot Filter

You can use any loot filter you want. If you don't have the recognition phrase in your loot filter, the recipe rules will be added to the top of your existing loot filter (which overwrites some rules, working on a fix). When the recognition phrase is added, the app will only change the styles in within the phrases. That means you can decide where the recipe rules should be modified.

Don't forget to refresh your loot filter in-game everytime the item types change!

Please make a copy of your loot filter before using it.

#### **_Notes on manipulation of Online Loot Filters_**

So some updates on the filter manipulation: Local works just fine! Woo! 🥳

But... we're running into some limitations with the ONLINE Filter Manipulation.

Here's where we're at:
- Our program has no trouble manipulating the contents of the online filter
- Our program is successfully sending/saving those changes up to your profile (you can check for yourself after running a fetch)

Once your fetch is complete and your filter is updated on your profile (ONLINE, NOT in-game yet), you hit the reload filter button expecting the new filters to show up. They don't.

After hitting up the dev chat on TFT, I believe the cause is caching that is happening on the GGG side of things for your Loot Filters.

According to FilterBlade, they had to get special permission to implement their version of 'near real-time updates' for filters made on their site.

As a developer, I can completely understand their reasons for implementing a cache before hitting the database - it's faster and generally speaker easier a load on some of their services under the hood.

So, all that said, for the time being we won't be able to guarantee that ***ONLINE *** filter manipulation will work 100% with the new update. It may not even affect the lot of y'all unless you're hitting the set threshold you have set every couple of minutes.

Really sorry about this, y'all!

###### Recognition Phrases:
 - Chaos Recipe Start: "#Chaos Recipe Enhancer by kosace Chaos Recipe Start"
 - Chaos Recipe End: "#Chaos Recipe Enhancer by kosace Chaos Recipe End"
 
 - Exalted Recipe Start: "#Chaos Recipe Enhancer by kosace Exalted Recipe Start"
 - Exalted Recipe End: "#Chaos Recipe Enhancer by kosace Exalted Recipe End"
 
Don't "cross" them, meaning if you start one section end it before starting the other one.

You can change the look of the Chaos Recipe Section in "C:\Users\username\AppData\Roaming\ChaosRecipeEnhancer\Styles\NormalItemsStyle.txt". 

You can change the look of the Exalted Recipe Section in "C:\Users\username\AppData\Roaming\ChaosRecipeEnhancer\Styles\InfluencedItemsStyle.txt"

Unfortunately there is no possibility to automatically refresh your loot filter in-game (that are legal), so you will have to do that manually (2 clicks).

### Settings

#### Account Name

The name of your PoE account (not the character).

#### PoE Session ID

Login at PoE website and copy it from your cookies (press F12). Everytime you re-log at the website your SessionID gets refreshed. You can find an easy to use tutorial [here](http://www.vhpg.com/how-to-find-poe-session-id/). To prevent leaking your SessionID, it will only show dots.

Don't share your SessionID with anyone.

#### League

The league you are playing in.

- Normal: just the league name. e.g. "Standard" or "Heist"
- SSF: SSF + the league name e.g. "SSF Standard" or "SSF Heist"
- Hardcore: just "Hardcore"
- Event: the name under the character on poe website e.g. for Mayhem it was "Mayhem (DRE001)"

#### Full Set Threshold

This is the amount of sets you want to gather. The loot filter will hide items depending on this setting. For example: if you want 5 sets and have 5 body armours already, the loot filter will hide body armours. If you have this number of sets full, you should sell, otherwise every item will be hidden (except for jewellery). 

#### Refresh Rate

The time in seconds the tool waits between fetching the data. The tool will ask the servers every x seconds, no matter if you are in a map or in your hideout. The minimum time is 15 seconds, but I recommend setting it higher, especially if you have many stash tabs configured.

#### Fill Greedy

The tool tries to fit at least 1 item with item levels 60-74 in every set. Then it tries to fill the rest of the slots with higher item level items. When Fill Greedy is activated it will then fill the remaining slots with lower item level items again. When deactivated it will only allow 1 lower item level item in each set, that means it will save lower item level items for the next set. So you are more efficient when you are deactivating it while playing high tier maps.

#### Exalted Shard Recipe

When activated the tool will also look for influenced unidentified rare item sets, but it will never hide influenced items. When you have a full set, it will tell you in the Main Overlay.

#### Stash tab Mode

Switches the way how you configure your stash tabs. 
 - ID: let's you input a sequence of stash tab IDs. 
 - Prefix: let's you input a stash tab name prefix
 
#### Stash tab Indices
 
 Visible in ID Stash tab Mode. Enter all stash tab with their IDs that you want to fetch seperated with a comma. Indexes start at 0.
 
 For example: "0,1,3,4" fetches the first, second, 4th and the 5th stash tab.
 
#### Stash tab Prefix

Visible in Prefix Stash tab Mode. The tool will look for any stash tab that starts with that prefix and get the IDs automatically. 

For example: "Test" will fetch stash tabs named "Test" but also Stash tabs named "Testsldfjs"

#### Opacity Overlay

The opacity of the Main Overlay from 0 to 1 which means 0% to 100%.
 
#### Close to Tray

When you close the Settings page it will minimize to the tray instead of terminating the tool.

#### Overlay Mode

Switches the Main Overlay modes.

##### Standard

Shows each item type and optionally their amounts. 

##### Minified

Smaller version of the standard overlay. Click on the number of sets (left) to open Stash tab Overlay or click on "S" (right) to start/ stop fetching. 

Red Border means it is not fetching.
Green Border means it is fetching.

##### Only Buttons

Only shows each Button and the number of full sets.

#### Show Item Amounts

Only works in Standard Overlay Mode. Shows the amount of each item you have to fill full sets. Means if you have 5 helmets it shows you 5. But if you have 7 rings it shows you 3 because you can make 3 sets with 7 rings. Item levels will be ignored.

#### Hotkeys

Here you can set hotkeys for showing/ hiding the Main Overlay, Stash tab Overlay and for starting/ stopping the fetching to the servers. 

Although the tool is fully usable with mouse only, too.

#### Opacity Stash Tab Overlay

The opacity of the Stash tab overlay in general. Although if you want the borders of the item cells visible, set it to 1 and modify the Stash tab Background Color. There you can set opacity of the background.

#### Stash tab Background Color

Advantage of this is, that you can set opacity here too. Means you can have fully visible borders and still see your items in your stash tab.

The opacity is the 'A' of the RGBA.

#### Highlight Color

The color of the highlighting in Stash tab Overlay while selling.

#### Highlight Mode

 - Item by Item: shows you each item after another in the right order to fit 2 full sets in your inventory
 - Set by Set: shows you whole sets after each other. The right order to fit 2 full sets in your inventory will still be marked.
 - All Items: shows you every item in every full set. No order.
 
#### Tab Header Margin

The space from the left side of the window to your first tab header.

#### Tab Header Width

The width of each tab header.

#### Tab Header Gap

The space between each tab header.

#### Loot Filter Manipulation

Alters your loot filter every time you need to pick up other items. You still need to refresh your loot filter in-game manually. There is no legal way to do that for you.

#### Loot filter Location

Opens a file dialog. Pick any loot filter you want. Only shows .filter files. The standard location of PoE loot filters is: 

"/Username/Documents/My Games/Path of Exile/"

#### Colors

The colors of each item type that are written in your loot filter.

#### Notification Sound

Play a sound when you have to manually refresh your loot filter in-game and when you picked up a whole set in the Stash tab Overlay.

#### Sound Volume

The sound volume...

##  F.A.Q.

#### The tool seems to not pick the right stash tabs

Make sure you do NOT have "Remove-only Stash tabs" hidden. They will still be counted, even if you can't see them. That means the index will be off. 

This will also apply to Event Stash tabs. When you are playing in an event, check if you have Remove-only Tabs from events before. You can check your stash tab IDs by searching this address in your browser (replace YOURACCOUNTNAME and YOURLEAGUE accordingly):

https://www.pathofexile.com/character-window/get-stash-items?accountName=YOURACCOUNTNAME&league=YOURLEAGUE&tabs=1

It will output all your stash tabs currently available. 

#### The tool cannot find my account

If you are a steam user, make sure you have linked your steam account to your PoE account on the website

#### The highlighting in Stash Tab Overlay seems off

If you have changed your Display Scaling in Windows (Display Settings -> Scale and layout) it will not get the right mouse positions. Set it to 100%.

#### The highlighting in Stash tab Overlay shows items already sold

Make sure you do not close the overlay while selling. The data cannot be refreshed that fast, that means the tool calculates an order for picking the items out of your stash. This order will start from the beginning if you closed the overlay and did not fetch before. So just leave it open while selling.

#### The Stash tab Overlay is not aligned

You can align it yourself by going in edit mode. Press "Edit" on top of the Stash tab Overlay and drag the Stash tab Overlay (at the top) around. In the bottom right corner there is a grip to resize the Stash tab Overlay.

#### Is this tool safe?
You can check and compile the sourcecode for yourself, so it should be safe as long as you are downloading it from here. DO NOT download from any other locations and ensure you're downloading from our original repository.

#### Can I get banned for using this tool?
Well, I contacted GGG but as usual they don't answer any questions regarding legality of 3rd party tools. As this tool does not interfere with any game files directly, the answer is: probably no!

Although there are 2 points which could be problematic:
1. The tool fetches data from PoE servers repeatedly, so they could think you are stressing the servers too much. That is why I limited the refresh rate a bit. One request per one second. 
2. The tool manipulates your loot filter. I'm not sure if they are considered "game files" by GGG, personally I wouldn't consider them like that. The tool certainly doesn't interact with the game client in any way. But if you are unsure you can deactivate this feature and only use the overlay. 

## Attributions

#### Algorithm

 - **Immo** helped me greatly with improving and simplifying the algorithm. Also he seems to know every API request to PoE servers? Much Thanks!

#### Item Icons

  - Icons made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com/)
  
  - Icons made by [Smashicons](https://www.flaticon.com/authors/smashicons) from [www.flaticon.com](https://www.flaticon.com/)
  
  - Icons made by [iconixar](https://www.flaticon.com/authors/iconixar) from [www.flaticon.com](https://www.flaticon.com/)
  
#### Client Icon 

 - Icons made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com/)
  
  
## Contributions

If you want to contribute to the project you can push changes to the "patches"-branch. Do not push to master please.

Best thing would be to join the discord though especially if you want to make bigger changes. There we can discuss how to do what.
Help is always appreciated since this is my starter project for WPF anyways.

## Change Log

#### 1.3.1

- fixing "Unable to parse parameter for Class Rule" issues with One-Handed and Two-Handed weapons

#### 1.3.0

Note: The project is now being actively maintained by [@HiMarioLopez](https://github.com/HiMarioLopez) (Discord: Meatbox#1607, PoE Account Name: [ClumsyParasite](https://www.pathofexile.com/account/view-profile/ClumsyParasite)).
Feel free to hit me up on Discord or in-game! I'll be juicing it up for the first few days of 3.17.

- fixing filter manipulation and reloading issues
- adding rogue harbour as a valid 'home' location (thanks to [@Irfy](https://github.com/Irfy))
- fixing clipboard set/reset error (thanks to [@Essyer](https://github.com/Essyer))

#### 1.2.6

 - changed installer
 - added auto updater
 - added item filter reload button (thanks to [@kxc0re](https://github.com/kxc0re))

#### 1.2.5

 - fixed bug where non-rare items would count into sets
 - added custom sound option (thanks to [@C64Gamer](https://github.com/C64Gamer))

#### 1.2.4

 - added error message when online loot filter was not found
 - removed itemlog

#### 1.2.3

 - fixed breaking bug where items could not be found anymore because GGG changed their Api (thanks to [@b0ykoe](https://github.com/b0ykoe))
 - added online filter functionality, this feature may need some testing (thanks to [@Zyertdox](https://github.com/Zyertdox) )

#### 1.2.2

 - fixed bug from "fixed bug where amount of items owned/ missing got calculated too early" since it introduced another bug :D

#### 1.2.1

 - added warning when no stash tabs were found
 - fixed bug where missing chaos items never showed (thanks to @ikogan)
 - added warning when no recipes selected
 - fixed bug where exalted recipe set could never be filled (it will show a warning now when you have an exalted set full)
 - added option for item icons on minimap (only little white stars atm)
 - added option to lock main overlay position (@hakfo ;))
 - fixed bug where you couldn't fetch anymore after connection got refused
 - clarified warning for refused connections (forbidden)
 - added option to show amount of items missing instead of items owned
 - fixed bug where amount of items owned/ missing got calculated too early
 - added auto fetch support for each language (except thai, need help there :D), this needs to be the same language your PoE client is in

#### 1.2.0

 - removed check for refresh rate as it has been removed

#### 1.1.9

 - added Regal Recipe (thanks to Immo)
 - removed refresh rate
 - added manual Fetch button (30 sec cooldown)
 - added auto-fetch on new map (120 sec cooldown)
 - added Log File Location dialog (specify your PoE log file location to enable auto-fetch)

#### 1.1.8

 - fixed a "not enough new lines" bug (thanks to @MarcLandis)

#### 1.1.7

 - fixed new lines bug in exalted shard recipe (which will also occur if you dont use this recipe)

#### 1.1.6

 - fixed items not being activated and deactivated correctly

#### 1.1.5

 - added Reset Settings button
 - fixed infinite spaces in loot filter bug
 - added Always Active Section
 - rearranged UI
 - added support for identified items
 - increased Tab Header Margin to 500 maximum
 - added Show Item Amounts support for Minified UI mode
 - added Contribution Section
 - separated Exalted recipe from Chaos Recipe. You can now turn both on and off individually.

#### 1.1.4 

 - added user-agent to every request (thanks to @WhiteFang5)
 - changed minimum refresh rate to 45 seconds (this will change soon when I rework the fetching algorithm)
 - avoided InvalidCastException (thanks to @devinvisible)
 - removed "Hide" rules from loot filter, now your own rules kick in if you have enough items for the recipe
 - actual numbers of each item are now shown
 
#### 1.1.3

 - fixed fetching bug (thanks Immo)

#### 1.1.2

 - fixed bug where highlight sound played with empty stash tabs
 - fixed bug where stash overlay showed item sets with no low item level item
 - loot filter now prevents hiding of 6 link and 6 socket items
 - added Save Button, the app will still save settings when you close it regularly
 - changed fetching, it is way faster now
 - added rate limit calculations, now it should be impossible to get request banned
 - added warnings for temporary bans and if you exceed the rate limit
 - crash reports should now appear correctly
 - now shows every item type in Overlay, including rings, belts and amulets
 - added Overlay modes
 - added Minified Overlay UI
 - added Buttons Only Overlay UI
 - added option to show item amounts (only in Standard mode)
 - updated Guide
 
#### 1.1.1

 - fixed bug where influenced items got hidden
 - (probably) fixed crash while fetching
 - fixed bug where exalted orb recipe rules did not write to item filter
 - fixed bug in item by item mode where sound was not playing on last item
 

#### 1.1.0

 - added warning when fetching failed
 - fixed a bug where Stash tab Overlay was not "through-clickable" after using the edit mode
 - changed Stash tab Overlay grid color to white
 - set minimum opacity of overlays to 0.01
 - fetching now automatically resumes when closing the Stash tab Overlay, if you were fetching before.
 - minor UI changes

#### 1.0.9

 - fixed bug not showing item order in set by set highlight mode in quad tabs
 - fixed bug when entering account name or league with spaces
 - changed icon
 - fixed bug showing items item level < 60

#### 1.0.8

 - grouped Settings by category
 - removed Save Button, everything should save automatically
 - removed individual stash tabs
 - added stash tab mode, for easier stash tab adding
 - now automatically detects quad tabs
 - now automatically detects stash tab names/ IDs
 - added distance algorithm, the tool prefers items close together
 - added highlight mode
 - added sound when full set is picked up for selling
 - added support for every item level, now you can mix higher and lower item level items
 - added fill greedy mode, you can decide if there should only be one lower item level item in your sets or more
 - removed bases, works with classes now
 - removed the option for 2 hand weapons, now every 2 hand weapon with size 2x3 and every 1 hand weapon with size 1x3 will be allowed
 - added initial position and size of Stash tab Overlay optimized for full hd
 - added password font in SessionID field, no more leaking your ID
 - updated guide

#### 1.0.7

 - minor UI changes
 - added colors for jewellery
 - fixed crash occuring when sound notification is activated

#### 1.0.6

 - fixed loot filter parsing bug (should recognize the phrases now correctly)
 - added default values
 - adjusted stash tab overlay highlighting sizes
 - added bows and 1h maces
 
#### 1.0.5

 - added Two-Hand Weapon support
 - added Custom Style support
 - added Exalted Shard Recipe (Read guide)
 - changed loot filter parsing
 - added sound when loot filter changes
 - added custom tab header width for aligning tab headers to game
  
  
  
  
  
  
