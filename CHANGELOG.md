## Change Log

#### 2.0.0-alpha (WIP)

- documentation overhaul (including slimmer readme & new wiki pages)

#### 1.5.5

- fixing crash that could occur if user closes overlay while fetching with the stash tab overlay open (thanks to [@ttgx1000](https://github.com/ttgx1000))

#### 1.5.4

- new installer UI that allows for custom installer location (also gives you progress on installation and notifies you when it's complete)
- fixing filter manipulation not including higher item level items (i75+) when running chaos recipe, should now add higher item level items to filter if able
- introduced more limited filtering on items to ensure we don't overwrite any of your loot filter's rules (e.g. 6-socket items, 5-link items, 6-link items, etc.)

#### 1.5.3

- fixing crash that would occur when pressing certain keys on the hotkey window (thanks to [@Nyxion](https://github.com/Nyxion))
- adding message box to show when app is having issues connecting to the GGG API for fetching league info (thanks to [@coffeehouse](https://github.com/coffeehouse))
- added error logging to a file (will tie this to a 'Debug Mode' setting; for now I'll leave it on)
- added option to nuke settings after CRE client crash (thanks to Edwo#1770 on Discord for the idea)
- fixing incorrect name for "Do Not Preserve Low Item Level Gear" and further added info the tooltip (continues to confused folks on what it does)
- fixing clipboard access issue that will hopefully resolve filter reload issues (thanks to [@InsanityPrelude](https://github.com/InsanityPrelude) for pointing me to the solution)
- continued project refactors (tons of improvements under the hood that make my life easier, and your general experience much smoother)

#### 1.5.2

- Hotfix: Fixing Icons not showing up
- Hotfix: Fixing crash related to filter styles

#### 1.5.1

- fixing cre client crash that would happen when upgrading from any version to 1.5.0
- renaming "Fill Greedy" mode to "Preserve Low Item Level Gear" mode with a better description of what it does

#### 1.5.0

- adding new 'Suffix' stash tab selection mode (thanks to [@george-delchev](https://github.com/george-delchev))
- fixing app crashing when finding items without an ilvl in stash tabs (e.g. compasses)
- adding check for selected tab types; users can no longer query from tabs other than Normal or Quad tabs (i.e. no querying from Currency / Divination / Fragment tabs, etc.)
- modifying default settings that ship with the app to make things more intuitive (based on community feedback)
- modifying a lot of code under the hood in an effort to improve project structure and make future maintenance easier (thanks to [@george-delchev](https://github.com/george-delchev))

#### 1.4.1

- fixing random CLIPBRD_E_CANT_OPEN error being thrown
- adding 'Custom League' check box that makes it so you can input a custom league name in lieu of the main league selectors

#### 1.4.0

- removing online filter manipulation as a feature; **_you can still manipulate your offline filters_**
- fixing rate limit exceeded error (thanks to [@catinsock](https://github.com/catinsock))
- improved league selection (formerly a text input, now a selection list) (thanks to [@catinsock](https://github.com/catinsock))
- fixing random typos in the settings screen

#### 1.3.2

- fixing NullPointerException when attempting to update online filters (thanks to [@devinvisible](https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/commits?author=devinvisible))
- fixing typo 'Ceck for Updates' -> 'Check For Updates' (thanks to [@devinvisible](https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/commits?author=devinvisible))
- fixing some of the weapon 

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
