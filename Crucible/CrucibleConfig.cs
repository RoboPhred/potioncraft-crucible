namespace RoboPhredDev.PotionCraft.Crucible
{
    using BepInEx.Configuration;

    public class CrucibleConfig
    {
        public ConfigEntry<string> ModulePath { get; set; }
        public ConfigEntry<string> ModPath { get; set; }

        public CrucibleConfig(ConfigFile config)
        {
            this.LoadFromBindings(config);
        }

        public void LoadFromBindings(ConfigFile config)
        {
            this.ModulePath = config.Bind("Crucible Config settings", "Module file path", "BepInEx/plugins/crucible/modules", "The path to your installed Crucible modules.");
            this.ModPath = config.Bind("Crucible Config settings", "Mod file path", "BepInEx/plugins", "The path to your installed Crucible mods.");
        }
    }
}
