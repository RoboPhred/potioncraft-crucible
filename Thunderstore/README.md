# Crucible Modding Framework
Crucible is a modding framework for PotionCraft, with the following features

-   Abstracted game api, to allow for future-proof access to the game from BepInEx mods.
-   YAML config based mod loader, to allow for configuration based mods without code.

Out of the box, Crucible provides support for creating code-free PotionCraft mods for:

-   Custom ingredients
-   Custom potion bottles, including labels and icons
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

## Installation (game, automated)
This is the recommended way to install Crucible Modding Framework.

- Download and install Thunderstore Mod Manager or r2modman.
  - Click Install with Mod Manager button on top of the page.
  - Run the game via the mod manager.

Note: If you are installing a Crucible dependent mod use the installation instructions provided by that mod. When installing from Thunderstore Crucible will be automatically downloaded if it is included as a dependency.

## Installation (manual)
If you are installing this manually, do the following:

If you are not sure where your Potion Craft steam directory is you can find out by opening steam, going to your library, and right clicking on Potion Craft > Properties > Local Files > Browse.

- Download and install [BepInEx 5.x 64 bit](https://github.com/BepInEx/BepInEx/releases)
  - The contents of the BepInEx zip should be extracted to your PotionCraft steam directory.
  - If properly installed, you should see a `winhttp.dll` file and `BepInEx` folder alongside your `Potion Craft.exe`
- Download and install [Crucible](https://github.com/RoboPhred/potioncraft-crucible/releases/latest)
  - The contents of the Crucible zip should be extracted to your PotionCraft steam directory.
  - If properly installed, you should have a `Crucible` driectory at `Potion Craft/BepInEx/plugins`
  
## Crucible Package Mods

Crucible Package mods are collections of assets and config files that allow mods to be created for PotionCraft without resorting to reverse engineering or compiling custom code.
Anyone who can put together art assets can use Crucible Packages to create mods for the game.

Some benefits of using Crucible Packages:

-   Low barrier of entry: No programming experience is required.
-   Much less chance of future game updates breaking compatibility. Only Crucible needs to be updated.

### Making Crucible Package Mods

See [Creating Crucible Packages](https://github.com/RoboPhred/potioncraft-crucible/wiki/Getting-Started:-Crucible-Package-Mods)

### Using Thunderstore with Crucible Package Mods

See [Using Thunderstore with Crucible Package Mods](https://github.com/RoboPhred/potioncraft-crucible/wiki/Getting-Started:-Uploading-your-Crucible-Package-Mod-to-Thunderstore)

## Using the Crucible Modding API

Modders wishing to take advantage of Crucible's GameAPI should take a BepInEx dependency on Crucible and reference the Crucible.GameAPI dll.

**Note**: While it is possible to include the Crucible.GameAPI dll with your mod download, doing so puts you at great risk for causing conflicts with other mods, including Crucible itself. While Crucible.GameAPI takes great care
to remain maximally compatible with other mods, certain features may not function properly if duplicated across multiple mods. Storing custom data on save files and injecting custom sprite atlases are particularly fragile, and distributing
your own copy of Crucible.GameAPI may break this functionality in the presense of Crucible Core or other mods that also redistribute Crucible.GameAPI.



### Steam Deck Installation
See [this guide](https://docs.google.com/document/d/1Y3PDeMaffkh7x4U3j46YZ9K6AhM2EvRF9v3mAGBFzW4) for installing Potion Craft mods on the Steam Deck