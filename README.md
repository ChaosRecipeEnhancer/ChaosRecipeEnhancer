# Enhance PoE - A Chaos Recipe Tool

For Feedback or Bug Reports send me an E-Mail to kosacewebdev@gmail.com.


#### Main Overlay

<img src="https://github.com/kosace/EnhancePoEApp/blob/master/enhancepoescreen2.png" width="1000">

#### Stashtab Overlay

![Stashtab Overlay](https://github.com/kosace/EnhancePoEApp/blob/master/EnhancePoEscreen1.png)

#### The Settings

![Settings](https://github.com/kosace/EnhancePoEApp/blob/master/enhancepoescreen3.png)

## Installation
Coming Soon...

## Features

 - Easy to use
 - Multiple Stashtabs
 - Customizable Overlay
 - Stashtab Overlay for easy selling
 - Lootfilter Manipulation (optional)

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

The Stashtab Overlay highlights items in a specific order if you have full sets in a stashtab. No more wondering which part is missing when selling. Also this way you can put 2 sets in one inventory.

If you want to change position or size of the Stashtabs Overlay, you have to press the "Edit" button on the Main Overlay. Then you can drag the Stashtabs Overlay around and resize it (bottom right corner), except if you set the opacity to 0 (better set it to 0.01). At the moment the tabheader sizes are optimized for full hd. I will add support for manually editing these later.

#### Stashtabs Order

You have to specify the number of your stashtab from left to right starting from 0. That means your 1st stashtab is 0, your 2nd is 1 and so on. It seems that folders count as 1 stashtab. At the moment it is not possible to fetch items in folders (send E-Mail if you know how).

#### Lootfilter

You can use any lootfilter you want. When modified new rules will be added without deleting your old rules. Only if you have specific rules for the chaos recipe these rules will get overwritten. At the moment there is no possibility of changing the look of highlighting other than color.

Unfortunately there is no possibility to automatically refresh your lootfilter ingame (that are legal), so you will have to do that manually (2 clicks).

#### Bases

If you want to change the bases you pick up, navigate to installationsfolder/bases and edit the .txt files accordingly. This way you can still use this tool when new bases are released or if you want to pick up bigger weapons too. 

##  F.A.Q.
#### How do I get my SessionID?
Login at PoE website and copy it from your cookies. Everytime you relog at the website your SessionID gets refreshed. You can find an easy to use tutorial [here](http://www.vhpg.com/how-to-find-poe-session-id/).

Don't share your SessionID with anyone.

#### Is this tool safe?
You can compile the sourcecode for yourself or control outgoing traffic with [Wireshark](https://www.wireshark.org/). You will see that this tool only communicates with PoE servers. The hotkeys are safe too as they are Windows hotkeys. No keylogging if you thought so.

#### Can I get banned for using this tool?
Well, I contacted GGG but as usual they don't answer any questions regarding legality of 3rd party tools. As this tool does not interfer with any game files directly, the answer is: probably no!

Although there are 2 points which could be problematic:
1. The tool fetches data from PoE servers repeatedly, so they could think you are stressing the servers too much. That is why I limited the refreshrate a bit. One request per one second (if you have under 15 stashtabs configured). 
2. The tool manipulates your lootfilter. I'm not sure if they are considered "game files" by GGG, personally I would'nt consider them so. But if you are unsure you can deactivate this feature and only use the overlay. 
