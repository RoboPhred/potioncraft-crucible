# Crucible Mod Framework

Crucible is a modding framework for PotionCraft, with the following features

- Abstracted game api, to allow for future-proof access to the game from BepInEx mods.
- YAML config based mod loader, to allow for configuration based mods without code.

### License

Copyright (c) RoboPhredDev, 2021

Licensed under the GNU Lesser General Public License

In essence, this means that

- All modifications or derivatives to any library in the Crucble framework must likewise be licensed under a LGPL or compatible license, and have their source be made available.
- Code from this repository cannot be used in other projects that are redistributed unless that project is also licensed under LGPL or a compatible license.

However, you can still make your own projects referencing Crucible under any license and without making the source available:

- Third party software that references any Crucible dll does NOT need to be LGPL. It may be licensed under any license, and may be closed source.
- Crucible Config Mods (package.yml and associated assets) may be licensed under any license.
- You can redistribute Crucible DLLs and other content freely, provided you include a copy of LICENSE.txt with your distribution.

For clarification and more information, see LICENSE.txt.
Where this section conflicts with LICENSE.txt, the content in LICENSE.txt takes priority.
