# The PoE Chaos Recipe Enhancer

This App fetches data from PoE servers and shows which items you need to pick up for the Chaos Recipe. Optionally it manipulates your lootfilter, so you can go full braindead while farming! While selling your stuff it shows you what items to put in your inventory in which order. No more using your brain at all!

For Feedback or Bug Reports spam this [discord](https://discord.gg/KgVsUdSSrR) or open an Issue on github.

#### Current Version 1.1.4

For details on changes, check the Change Log at the bottom of this site.

## Installation

DO NOT download the whole github repository, since it may include some new experimental features.

[Download the setup here](https://github.com/kosace/EnhancePoEApp/releases) instead. 

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

The overlays will not work in Fullscreen Mode. They will only work in:
 - Windowed Mode
 - Fullscreen Windowed Mode
 
The tool only works in Windows, no support for other OS.

Overlay only works with Display Scaling 100% in Windows Settings (working on a fix). 

#### Main Overlay

The Main Overlay shows each itemtype with its color and while fetching data it shows a loading bar. When you have enough items of a specific itemtype the image will grey out. If you have Lootfilter Manipulation activated, this is the time to refresh your lootfilter manually ingame. Also if you have sound activated, it will play to remember you to refresh your lootfilter.

You can just drag the Main Overlay whereever you want, except if you set the opacity to 0 (better set it to 0.01).

 - Show Button: shows/ hides the StashTab Overlay
 - Fetch Button: starts fetching items from PoE servers periodically
 
There will be warning displayed in this overlay also:

 - Warning if you have full sets, but need lower ilvl items (ilvl 60-74)
 - Warning if you have full sets and need to sell them
 - Warning if you have full exalted shard recipe set
 - Warning if you are temporarily banned from fetching from PoE servers
 - Warning if you exceeded the rate limit

#### Stashtab Overlay

The Stashtab Overlay highlights items in a specific order if you have full sets in a stashtab. No more wondering which part is missing when selling. Also this way you can put 2 sets in one inventory. Leave it open when selling! Otherwise the highlighting will start from beginning with items you already sold.

If you want to change position or size of the Stashtabs Overlay, you have to press the "Edit" button on the Main Overlay. Then you can drag the Stashtabs Overlay around and resize it (bottom right corner), except if you set the opacity to 0 (better set it to 0.01). The TabHeader position and sizes can be modified in the Settings Page.

At the top of the overlay you can find the

 - Edit Button: makes the StashTab Overlay clickable and draggable

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

The name of your PoE account (not the character).

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

#### Stashtab Mode

Switches the way how you configure your stashtabs. 
 - ID: let's you input a sequence of stashtab IDs. 
 - Prefix: let's you input a stashtab name prefix
 
#### Stashtab Indices
 
 Visible in ID Stashtab Mode. Enter all stashtab with their IDs that you want to fetch seperated with a comma. Indexes start at 0.
 
 For example: "0,1,3,4" fetches the first, second, 4th and the 5th stashtab.
 
#### Stashtab Prefix

Visible in Prefix Stashtab Mode. The tool will look for any stashtab that starts with that prefix and get the IDs automatically. 

For example: "Test" will fetch stashtabs named "Test" but also Stashtabs named "Testsldfjs"

#### Opacity Overlay

The opacity of the Main Overlay from 0 to 1 which means 0% to 100%.
 
#### Close to Tray

When you close the Settings page it will minimize to the tray instead of terminating the tool.

#### Overlay Mode

Switches the Main Overlay modes.

##### Standard

Shows each itemtype and optionally their amounts. 

##### Minified

Smaller version of the standard overlay. Click on the number of sets (left) to open Stashtab Overlay or click on "S" (right) to start/ stop fetching. 

Red Border means it is not fetching.
Green Border means it is fetching.

##### Only Buttons

Only shows each Button and the number of full sets.

#### Show Item Amounts

Only works in Standard Overlay Mode. Shows the amount of each item you have to fill full sets. Means if you have 5 helmets it shows you 5. But if you have 7 rings it shows you 3 because you can make 3 sets with 7 rings. Itemlevels will be ignored.

#### Hotkeys

Here you can set hotkeys for showing/ hiding the Main Overlay, Stashtab Overlay and for starting/ stopping the fetching to the servers. 

Although the tool is fully usable with mouse only, too.

#### Opacity StashTab Overlay

The opacity of the Stashtab overlay in general. Although if you want the borders of the itemcells visible, set it to 1 and modify the Stashtab Background Color. There you can set opacity of the background.

#### Stashtab Background Color

Advantage of this is, that you can set opacity here too. Means you can have fully visible borders and still see your items in your stashtab.

The opacity is the 'A' of the RGBA.

#### Highlight Color

The color of the highlighting in Stashtab Overlay while selling.

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

#### Lootfitler Manipulation

Alters your lootfilter every time you need to pick up other items. You still need to refresh your lootfilter ingame manually. There is no legal way to do that for you.

#### Lootfilter Location

Opens a file dialog. Pick any lootfilter you want. Only shows .filter files. The standard location of PoE lootfilters is: 

"/Username/Documents/My Games/Path of Exile/"

#### Colors

The colors of each itemtype that are written in your lootfilter.

#### Notification Sound

Play a sound when you have to manually refresh your lootfilter ingame and when you picked up a whole set in the Stashtab Overlay.

#### Sound Volume

The sound volume...

##  F.A.Q.

#### The tool seems to not pick the right stashtabs?

Make sure you do NOT have "Remove-only Stashtabs" hidden. They will still be counted, even if you can't see them. That means the index will be off. 

This will also apply to Event Stashtabs. When you are playing in an event, check if you have Remove-only Tabs from events before. You can check your stashtab IDs by searching this address in your browser (replace YOURACCOUNTNAME and YOURLEAGUE accordingly):

https://www.pathofexile.com/character-window/get-stash-items?accountName=YOURACCOUNTNAME&league=YOURLEAGUE&tabs=1

It will output all your stashtabs currently available. 

#### The tool cannot find my account?

If you are a steam user, make sure you have linked your steam account to your PoE account on the website

#### The highlighting in StashTab Overlay seems off?

If you have changed your Display Scaling in Windows (Display Settings -> Scale and layout) it will not get the right mouse positions. Set it to 100%.

#### The highlighting in Stashtab Overlay shows items already sold

Make sure you do not close the overlay while selling. The data cannot be refreshed that fast, that means the tool calculates an order for picking the items out of your stash. This order will start from the beginning if you closed the overlay and did not fetch before. So just leave it open while selling.

#### The Stashtab Overlay is not aligned?

You can align it yourself by going in edit mode. Press "Edit" on top of the Stashtab Overlay and drag the Stashtab Overlay (at the top) around. In the bottom right corner there is a grip to resize the Stashtab Overlay.

#### Is this tool safe?
You can check and compile the sourcecode for yourself, so it should be safe as long as you are downloading it from here.

#### Can I get banned for using this tool?
Well, I contacted GGG but as usual they don't answer any questions regarding legality of 3rd party tools. As this tool does not interfer with any game files directly, the answer is: probably no!

Although there are 2 points which could be problematic:
1. The tool fetches data from PoE servers repeatedly, so they could think you are stressing the servers too much. That is why I limited the refreshrate a bit. One request per one second. 
2. The tool manipulates your lootfilter. I'm not sure if they are considered "game files" by GGG, personally I would'nt consider them like that. The tool certainly doesn't interact with the game client in any way. But if you are unsure you can deactivate this feature and only use the overlay. 

## Attributions

#### Algorithm

 - **Immo** helped me greatly with improving and simplifying the algorithm. Also he seems to know every API request to PoE servers? Much Thanks!

#### Item Icons

  - Icons made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com/)
  
  - Icons made by [Smashicons](https://www.flaticon.com/authors/smashicons) from [www.flaticon.com](https://www.flaticon.com/)
  
  - Icons made by [iconixar](https://www.flaticon.com/authors/iconixar) from [www.flaticon.com](https://www.flaticon.com/)
  
#### Client Icon 

 - Icons made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com/)
  
 
## Change Log

#### 1.1.4 

 - added user-agent to every request (thanks to @WhiteFang5)
 - changed minimum refreshrate to 45 seconds (this will change soon when I rework the fetching algorithm)
 - avoided InvalidCastException (thanks to @devinvisible)
 - removed "Hide" rules from lootfilter, now your own rules kick in if you have enough items for the recipe
 - actual numbers of each item are now shown
 
#### 1.1.3

 - fixed fetching bug (thanks Immo)

#### 1.1.2

 - fixed bug where highlight sound played with empty stashtabs
 - fixed bug where stash overlay showed itemsets with no low ilvl item
 - lootfilter now prevents hiding of 6 link and 6 socket items
 - added Save Button, the app will still save settings when you close it regularly
 - changed fetching, it is way faster now
 - added rate limit calculations, now it should be impossible to get request banned
 - added warnings for temporary bans and if you exceed the rate limit
 - crash reports should now appear correctly
 - now shows every itemtype in Overlay, including rings, belts and amulets
 - added Overlay modes
 - added Minified Overlay UI
 - added Buttons Only Overlay UI
 - added option to show item amounts (only in Standard mode)
 - updated Guide
 
#### 1.1.1

 - fixed bug where influenced items got hidden
 - (probably) fixed crash while fetching
 - fixed bug where exalted orb recipe rules did not write to itemfilter
 - fixed bug in item by item mode where sound was not playing on last item
 

#### 1.1.0

 - added warning when fetching failed
 - fixed a bug where Stashtab Overlay was not "through-cickable" after using the edit mode
 - changed Stashtab Overlay grid color to white
 - set minimum opacity of overlays to 0.01
 - fetching now automatically resumes when closing the Stashtab Overlay, if you were fetching before.
 - minor UI changes

#### 1.0.9

 - fixed bug not showing item order in set by set highlight mode in quad tabs
 - fixed bug when entering accountname or league with spaces
 - changed icon
 - fixed bug showing items ilvl < 60

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
  
  
  
  
  
  
