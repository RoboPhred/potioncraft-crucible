# Crucible GameAPI

Provides an api wrapping many game functions, allowing easy and future proof access to game functions.

This API also provides shared access to sections of the game that might otherwise cause inter-mod compatibility issues, such as adding custom ingredient icons.

## Designed for future proof updates

This library tries its best to wrap PotionCraft's types behind its own, allowing for breaking changes from PotionCraft to be absorbed by this library.
This allows the responsibility of updating for game compatibility to be handled by the GameAPI library, instead of having to update each and every mod.

Developers of Crucible GameAPI need to be sure they do not cause backwards-incompatible changes when developing this library. For more information on what changes
are allowed, see [.NET Compatibility](https://docs.microsoft.com/en-us/dotnet/core/compatibility/).
