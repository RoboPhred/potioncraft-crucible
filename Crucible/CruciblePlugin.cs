namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using BepInEx;
    using BepInEx.Configuration;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    [BepInPlugin("net.robophreddev.PotionCraft.Crucible", "Crucible Modding Framework", "1.0.0.0")]
    public class CruciblePlugin : BaseUnityPlugin
    {
        private ICollection<CrucibleConfigMod> mods;
        private CrucibleConfig config;

        public void Awake()
        {
            // FIXME: Come up with a better way to do this.
            // We might want to have config files define these.

            this.config = new CrucibleConfig(this.Config);

            CrucibleLog.Log("Loading Crucible modules.");
            this.LoadAllModules();

            CrucibleLog.Log("Loading Crucible mods.");
            this.mods = this.LoadAllConfigMods();
            CrucibleLog.Log($"> Loaded {this.mods.Count} mods.");

            CrucibleGameEvents.OnGameLoaded += (_, __) =>
            {
                CrucibleLog.Log("Activating Crucible mods.");
                foreach (var mod in this.mods)
                {
                    mod.ApplyConfiguration();
                }
            };
        }

        private void LoadAllModules()
        {
            string modulePath = this.config.ModulePath.Value;

            if (!Directory.Exists(modulePath))
            {
                return;
            }

            foreach (var dllFilePath in Directory.GetFiles(modulePath, "*.dll"))
            {
                try
                {
                    CrucibleLog.Log($"Loading module {dllFilePath}");
                    Assembly.LoadFile(dllFilePath);
                }
                catch (Exception e)
                {
                    CrucibleLog.Log($"Failed to load module {dllFilePath}: {e.Message}");
                }
            }
        }

        private ICollection<CrucibleConfigMod> LoadAllConfigMods()
        {

            string modPath = this.config.ModPath.Value;

            if (!Directory.Exists(modPath))
            {
                return new List<CrucibleConfigMod>();
            }

            var folders = Directory.GetDirectories(modPath);
            return folders.Select(folder => this.TryLoadMod(folder)).Where(x => x != null).ToList();
        }

        private CrucibleConfigMod TryLoadMod(string modFolder)
        {

            if(!File.Exists(modFolder + "/package.yml"))
            {
                return null;
            }

            try
            {
                var mod = CrucibleConfigMod.Load(modFolder);
                CrucibleLog.Log($"> Loaded mod \"{mod.Name}\".");
                return mod;
            }
            catch (CrucibleConfigModException ex)
            {
                Debug.Log(ex.ToExpandedString());
                return null;
            }
        }
    }
}
