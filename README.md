# Enhance PoE

A Chaos Recipe Tool

For Feedback or Bug Reports send me an E-Mail to kosacewebdev@gmail.com.

## Installation
Coming Soon...

## Features

 - Easy to use
 - Multiple Stashtabs
 - Customizable Overlay
 - Stashtab Overlay
 - Lootfilter Manipulation (optional)

##  F.A.Q.
#### How do I get my SessionID?
Login at PoE website and copy it from your cookies. Everytime you relog at the website your SessionID gets refreshed. You can find an easy to use tutorial [here](http://www.vhpg.com/how-to-find-poe-session-id/).

Don't share your SessionID with anyone.

#### Is this tool safe?
You can compile the sourcecode for yourself or control outgoing traffic with [Wireshark](https://www.wireshark.org/). You will see that this tool only communicates with PoE servers.

#### Can I get banned for using this tool?
Well, I contacted GGG but as usual they don't answer any questions regarding legality of 3rd party tools. As this tool does not interfer with any game files directly, the answer is: probably no!

Although there are 2 points which could be problematic:
1. The tool fetches data from PoE servers repeatedly, so they could think you are stressing the servers too much. That is why I limited the refreshrate a bit. One request per one second (if you have under 15 stashtabs configured). 
2. The tool manipulates your lootfilter. I'm not sure if they are considered "game files" by GGG, personally I would'nt consider them so. But if you are unsure you can deactivate this feature and only use the overlay. 
