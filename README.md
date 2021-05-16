# Tron for FiveM

A *basic* implementation of the GTA:O Deadline gamemode, except it's not the gamemode part, it's *just* the trail effect for the `shotaro` and `deathbike2` bikes.

## Config options

There are no config options.
If you want to change something go look in the code.
I've marked 3 *somewhat* configurable constants in the main class.
I've also added comments to pretty much everything, so good luck.

## Features

- The trail color is based on the vehicle color combination by default, giving the vehicle a new secondary color will make it use that color instead.
- Trail disappears when you get off your bike.
- Music events are triggered whenever you get on the bike and they stop when you get off again.
- Deadline screen effects.
- You can only see your own trail. Effects are not synced with other players.
- Trails 'lean' properly to follow the lean angle of your bike.

## Demo

https://youtu.be/zfYkC17rXeI

## Changelog

### 1.0.1

- Added the `deathbike2` model.
- Added support for more trail color options by detecting the secondary color of the vehicle if color combinations are not used.

### 1.0.0

- Initial release.

## Download

https://github.com/TomGrobbe/Tron/releases

## Forum topic

https://forum.cfx.re/t/tron-deadline-bike-trails/2870376?u=vespura
