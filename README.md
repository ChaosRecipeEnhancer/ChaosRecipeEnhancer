# The PoE Chaos Recipe Enhancer

This app fetches your stash data from the PoE servers and shows which items you need to pick up for the chaos recipe. Optionally, it can also manipulate your loot filter, so you can go full brain-dead while farming! While selling your stuff it shows you what items to put in your inventory in which order. No more using your brain at all! Great for some quick and easy chaos, specifically during league starts!

For Feedback or Bug Reports spam this [discord](https://discord.gg/KgVsUdSSrR) or [open an Issue on GitHub](https://github.com/kosace/EnhancePoEApp/issues/new) (and please include as much detail as possible, with screenshots of your settings if you can!).

#### Current Version 1.5.2 (May 13th, 2022; Path of Exile v3.18)

This product isn't affiliated with or endorsed by Grinding Gear Games in any way.

For details on changes, check out the CHANGELOG.md file, alternatively, every new version of the app has a copy of the changes included in that release.

## Support

With Kosace's permission, I will be changing the PayPal link and all donations will be going towards giveaways happening every league. I am blessed enough to have a solid job as a developer and do this only to give back to my community, so this could be your chance to give back, too!

[![Donate with PayPal](https://raw.githubusercontent.com/kosace/EnhancePoEApp/master/DocumentationAssets/Donate.png)](https://www.paypal.com/donate/?hosted_button_id=4NDCV5J5NTEWS)

## Installation

### Pre-requisites

The only pre-requisite for this app is to install the [.NET Framework Runtime](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)

### Downloads

DO NOT download the whole GitHub repository, since it may include some new experimental features.

[Download the setup.msi here](https://github.com/kosace/EnhancePoEApp/releases) instead. 

Since this app is not 'trusted' (i.e. we're not a corporation who has the money or clout to sign our installer), you will have to accept a few security prompts to install the program ('Keep' the file on browser, then click on "More info" when Windows wants to protect your PC). Apologies for the inconvenience there. If anyone knows a cheap and easy way to get our installer signed please let us know! 

If you have updated your application to 1.2.0 please use a new item filter as some changes may break your existing one! Make sure to back up any custom filters you want our program to interact with in case of any [potentially unwanted behavior!](https://github.com/kosace/EnhancePoEApp/issues/106)

## Features

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

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/DocumentationAssets/Overlay-Screenshot-2.png" width="500" alt="Main Overlay">

#### Stash Tab Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/DocumentationAssets/Overlay-Screenshot-1.png" width="500" alt="Stash Tab Overlay">

#### The Settings

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/DocumentationAssets/Settings-Screenshot.png" width="500" alt="Settings Page">

## Guide

#### Compatibility

The overlays will **not** work in full-screen Mode. They will only work in:
 - Windowed Mode
 - Full-screen Windowed Mode
 
The tool has been tested with Windows 10/11, we have no plans to officially support other operating systems at this time. If you'd like to contribute towards a MacOS or Linux-compatible version of the app, please let us know!

We can only guarantee the overlay will work with display scaling set to 100% in your Windows Settings. I'd say we're working on a fix, but it's relatively low priority. Some folks have reported it working at 125% scaling @ 1440p. Again, not guaranteed.

#### Main In-Game Overlay

The main in-game overlay shows each item type with its color and while fetching data it shows a loading bar. When you have enough items of a specific item type the image will grey out. If you have loot filter manipulation activated, this is the time to refresh your loot filter manually in-game. Also, if you have sound activated, it will play to remind to refresh your loot filter.

You can drag the main in-game overlay wherever you want, except in cases where you set the opacity to 0 (it's probably better to set it to 0.01 or some higher value).

 - Show Button: shows/ hides the Stash Tab Overlay
 - Fetch Button: starts fetching items from PoE servers periodically
 
There will be warning displayed in this overlay also:

 - Warning if you have full sets, but need lower item level items (item levels 60-74)
 - Warning if you have full sets and need to sell them
 - Warning if you have full exalted shard recipe set
 - Warning if you are temporarily banned from fetching from PoE servers
 - Warning if you exceeded the rate limit (keep in mind rate-limits are enforced ACROSS ALL APPS AND WEBSITES for a given account; So if you query from PoE Trade Macro too many times, you will block your entire account for a fixed period, including this application)
    - Note: We have a few restrictions built in to our app to keep it from fetching more often than is needed. If you run into any rate limit issues, please try and check for other sources of rate-limit including other PoE apps, your trade searches, etc.

#### Stash Tab Overlay

The stash tab Overlay highlights items in a specific order if you have full sets in a stash tab. No more wondering which piece of gear you are missing when selling. Another added bit is with this feature, you should be able to fit 2 full sets in one inventory. Leave it open when selling! Otherwise the highlighting will start from beginning with items you already sold.

If you want to change position or size of the stash tab overlay, you have to press the "Edit" button on the stash tab overlay. Then you can drag the stash tabs overlay around and resize it (bottom right corner), except if you set the opacity to 0 (better set it to 0.01). The TabHeader position and sizes can be modified in the settings page.

At the top of the stash tab overlay you can find the

 - Edit Button: makes the Stash Tab Overlay clickable and draggable

#### Loot Filter

You can use any loot filter you want, [but preferably an offline filter](https://github.com/kosace/EnhancePoEApp/pull/274#issuecomment-1030049058). If you don't have the recognition phrase in your loot filter, the recipe rules will be added to the top of your existing loot filter (which overwrites some rules, working on a fix). When the recognition phrase is added, the app will only change the styles in within the phrases. That means you can decide where the recipe rules should be modified.

Don't forget to refresh your loot filter in-game every time the item types change!

Make sure to back up any custom filters you want our program to interact with in case of any [potentially unwanted behavior!](https://github.com/kosace/EnhancePoEApp/issues/106). I've said this before but just want to make it very clear. Don't want anyone to lose hours of their hard work due to a nasty bug!

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

Login at PoE website and copy it from your cookies (press F12). Every time you re-log at the website your SessionID gets refreshed. You can find an easy to use tutorial [here](http://www.vhpg.com/how-to-find-poe-session-id/). To prevent leaking your SessionID, it will only show dots.

Don't share your SessionID with anyone.

#### League

The league you are playing in; we pre-load all the GGG-hosted leagues into the drop down menu. If you are playing in a custom league, you can check the "Custom League" combo box and it will allow you to manually input your league name. Please be mindful of the spelling on those custom leagues.

#### Full Set Threshold

This is the amount of sets you want to gather. The loot filter will hide items depending on this setting. For example: if you want 5 sets and have 5 body armours already, the loot filter will hide body armours. If you have this number of sets full, you should sell, otherwise every item will be hidden (except for jewelery). 

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
 
 Note: We cannot query specialty tabs - this includes Currency tabs, Fossil Tabs, Divination Card tabs, Unique tabs, etc.

 We can ONLY query normal and quad tabs.

#### Stash tab Indices
 
 Visible in ID Stash tab Mode. Enter all stash tab with their IDs that you want to fetch separated with a comma. Indexes start at 0.
 
 For example: "0,1,3,4" fetches the first, second, 4th and the 5th stash tab.
 
#### Stash tab Prefix

Visible in Prefix Stash tab Mode. The tool will look for any stash tab that starts with that prefix and get the IDs automatically. 

e.g. "Test" will fetch stash tabs named "Test" but also Stash tabs named "Testsldfjs"

### Stash tab Suffix

Matches your given input and will look for any stash tab that ends with the provided suffix.

e.g. "CR" will match tabs named "Dump CR", "Test CR", "CR"

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
You can check and compile the source code for yourself, so it should be safe as long as you are downloading it from here. DO NOT download from any other locations and ensure you're downloading from our original repository.

#### Can I get banned for using this tool?
Well, I contacted GGG but as usual they don't answer any questions regarding legality of 3rd party tools. As this tool does not interfere with any game files directly, the answer is: probably no!

Although there are 2 points which could be problematic:
1. The tool fetches data from PoE servers repeatedly, so they could think you are stressing the servers too much. That is why I limited the refresh rate a bit. One request per one second. 
2. The tool manipulates your loot filter. I'm not sure if they are considered "game files" by GGG, personally I wouldn't consider them like that. The tool certainly doesn't interact with the game client in any way. But if you are unsure you can deactivate this feature and only use the overlay. 

### Have another question that's not on this list?

Find us on Discord and we're more than happy to answer any questions you may have. Chances are, someone else has already encountered your issue and found a way to resolve it. Here's the link to our server: https://discord.gg/HDSQVpnd

Message me directly if the invite link expires: Meatbox#1607

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

Some steps to follow if you want to start contributing to the project:

0. Download and install [.NET Framework 4.8 SDK (Not just the runtime)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
    - Note: I _highly_ recommend Visual Studio as your IDE of choice for this project, you can install with the VS installer
1. Download the latest version of the [Wix Toolset](https://wixtoolset.org/releases/)
2. Download and install the [Wix Toolset Extension for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=WixToolset.WixToolsetVisualStudio2022Extension)
3. Try your best to work on your own branch. All branches should go in either the 'feature' or 'bugfix' folder, respectively. All branches should spawn from the latest version of develop.
    - So, for instance, 'feature/cool-new-button' or 'bugfix/fixing-cool-button'
4. Once your work is done, please PR it into 'develop', and NOT 'master' (else, I'll just auto-reject the PR)
5. If you could ping me on Discord that would be awesome, especially if you want to make bigger changes. It'll probably be the quickest way to talk to me about code reviews, new features, design, etc.

Help is always appreciated, and we can never get enough of it. Thanks again for everyone's support and contributions to the project.

Cheers, and stay sane, exiles!