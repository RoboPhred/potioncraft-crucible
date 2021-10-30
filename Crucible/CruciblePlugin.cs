namespace RoboPhredDev.PotionCraft.Crucible
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BepInEx;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Ingredients;
    using UnityEngine;

    [BepInPlugin("net.robophreddev.PotionCraft.Crucible", "Crucible Modding Framework", "1.0.0.0")]
    public class CruciblePlugin : BaseUnityPlugin
    {
        private ICollection<CrucibleConfigMod> mods;

        public void Awake()
        {
            // FIXME: We have to reference assemblies to load them!  Auto load all assemblies in a folder to work around this.
            CrucibleLog.Log("Dummy reference " + typeof(CrucibleIngredientConfig));

            CrucibleLog.Log("Loading Crucible mods.");
            this.mods = LoadAllConfigMods();
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
