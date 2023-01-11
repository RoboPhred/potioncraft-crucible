# Crucible Mod Framework

Crucible is a modding framework for PotionCraft, with the following features

-   Abstracted game api, to allow for future-proof access to the game from BepInEx mods.
-   YAML config based mod loader, to allow for configuration based mods without code.

Out of the box, Crucible provides support for creating code-free PotionCraft mods for:

-   Custom ingredients
-   Custom potion bottles, including labels and icons
-   Custom potion bases and maps (Not yet updated for 1.0)
-   Custom potion effects (Not yet updated for 1.0)
-   Custom customers
-   Custom traders

Additionally, Crucible provides an API for other BepInEx mods that provides:

-   Shared access to sprite atlases, allowing mods to add new icons without conflicting.
-   Shared access to save data, allowing mods to add data to the save files in a way that will not conflict.
-   Adding new translatable text.
-   Identifying NPC templates by attribute.
-   Adding inventory items to NPC traders.
-   Access to create and customize ingredients, wrapped behind and API that will remain stable across game updates.
-   Support for mods that can add new features to Crucible Packages, both through new configuration sections and extending existing sections.

## Installation

### Installing Crucible

-   Download and install [BepInEx 5.x 64 bit](https://github.com/BepInEx/BepInEx/releases/latest)
    -   The contents of the BepInEx zip should be extracted to your PotionCraft steam directory.
    -   If properly installed, you should see a `winhttp.dll` file and `BepInEx` folder alongside your `Potion Craft.exe`
-   Download and install [Crucible](https://github.com/RoboPhred/potioncraft-crucible/releases)
    -   The contents of the Crucible zip should be extracted to your PotionCraft steam directory.
    -   If properly installed, you should have a `Crucible.dll` file (among others) at `Potion Craft/BepInEx/plugins/Crucible`

### Installing Crucible Package Mods

In general, each mod should include its own instructions for how to install it.

A successful mod installation should result in the mod placing a folder in `Potion Craft/crucible/mods`.

## Crucible Package Mods

Crucible Package mods are collections of assets and config files that allow mods to be created for PotionCraft without resorting to reverse engineering or compiling custom code.
Anyone who can put together art assets can use Crucible Packages to create mods for the game.

Some benefits of using Crucible Packages:

-   Low barrier of entry: No programming experience is required.
-   Much less chance of future game updates breaking compatibility. Only Crucible needs to be updated.

### Making Crucible Package Mods

See [Creating Crucible Packages](https://github.com/RoboPhred/potioncraft-crucible/wiki/Getting-Started:-Crucible-Package-Mods)

## Using the Crucible Modding API

Modders wishing to take advantage of Crucible's GameAPI should take a BepInEx dependency on Crucible and reference the Crucible.GameAPI dll.

**Note**: While it is possible to include the Crucible.GameAPI dll with your mod download, doing so puts you at great risk for causing conflicts with other mods, including Crucible itself. While Crucible.GameAPI takes great care
to remain maximally compatible with other mods, certain features may not function properly if duplicated across multiple mods. Storing custom data on save files and injecting custom sprite atlases are particularly fragile, and distributing
your own copy of Crucible.GameAPI may break this functionality in the presense of Crucible Core or other mods that also redistribute Crucible.GameAPI.

### License

Copyright (c) RoboPhredDev, 2021

Licensed under the GNU Lesser General Public License

In essence, this means that

-   If you wish to redistribute modifications or derivatives to any library in the Crucble framework, the modifications must be licensed under the LGPL or compatible license, include the LICENSE.txt copyright notice, and have their source be made publically available.
-   Code from this repository cannot be copied to other projects unless that project is also licensed under LGPL or a compatible license, made source-available, and includes the LICENSE.txt from this repository.
-   You can freely use anything from this repository for personal use so long as you do not redistribute it.

However, you can still make your own projects referencing Crucible under any license and without making the source available:

-   Third party software that references any Crucible dll does NOT need to be LGPL. It may be licensed under any license, and may be closed source.
-   Crucible Config Mods (package.yml and associated assets) may be licensed under any license.
-   You can redistribute Crucible DLLs and source code freely, provided you include a copy of LICENSE.txt with your distribution.
-   You can create and distribute mod packs containing Crucible DLLs anywhere, as long as the LICENSE.txt file from this repository is also included.

For clarification and more information, see LICENSE.txt.
Where this section conflicts with LICENSE.txt, the content in LICENSE.txt takes priority.
