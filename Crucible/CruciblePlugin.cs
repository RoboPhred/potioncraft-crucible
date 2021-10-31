namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using BepInEx;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    [BepInPlugin("net.robophreddev.PotionCraft.Crucible", "Crucible Modding Framework", "1.0.0.0")]
    public class CruciblePlugin : BaseUnityPlugin
    {
        private ICollection<CrucibleConfigMod> mods;

        public void Awake()
        {
            // FIXME: Come up with a better way to do this.
            // We might want to have config files define these.
            CrucibleLog.Log("Loading Crucible modules.");
            LoadAllModules();

            CrucibleLog.Log("Loading Crucible mods.");
            this.mods = LoadAllConfigMods();

            CrucibleGameEvents.OnGameLoaded += (_, __) =>
            {
                CrucibleLog.Log("Activating Crucible mods.");
                foreach (var mod in this.mods)
                {
                    mod.ApplyConfiguration();
                }
            };
        }

        private static void LoadAllModules()
        {
            if (!Directory.Exists("crucible/modules"))
            {
                return;
            }

            foreach (var dllFilePath in Directory.GetFiles("crucible/modules", "*.dll"))
            {
                try
                {
                    CrucibleLog.Log($"> Loading module {dllFilePath}");
                    Assembly.LoadFile(dllFilePath);
                }
                catch (Exception e)
                {
                    CrucibleLog.Log($"Failed to load module {dllFilePath}: {e.Message}");
                }
            }
        }

        private static ICollection<CrucibleConfigMod> LoadAllConfigMods()
        {
            if (!Directory.Exists("crucible/mods"))
            {
                return new List<CrucibleConfigMod>();
            }

            var folders = Directory.GetDirectories("crucible/mods");
            return folders.Select(folder => TryLoadMod(folder)).Where(x => x != null).ToList();
        }

        private static CrucibleConfigMod TryLoadMod(string modFolder)
        {
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
