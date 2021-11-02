// <copyright file="CruciblePlugin.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

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

    /// <summary>
    /// BepInEx plugin for Crucible Config mods.
    /// </summary>
    [BepInPlugin("net.RoboPhredDev.PotionCraft.Crucible", "Crucible Modding Framework", "1.0.0.0")]
    public class CruciblePlugin : BaseUnityPlugin
    {
        private ICollection<CrucibleConfigMod> mods;

        /// <summary>
        /// Called by unity when the plugin loads.
        /// </summary>
        public void Awake()
        {
            // FIXME: Come up with a better way to do this.
            // We might want to have config files define these.
            CrucibleLog.Log("Loading Crucible modules.");
            LoadAllModules();

            CrucibleLog.Log("Loading Crucible mods.");
            this.mods = LoadAllConfigMods();

            CrucibleGameEvents.OnGameInitialized += (_, __) =>
            {
                CrucibleLog.Log("Activating Crucible mods.");
                foreach (var mod in this.mods)
                {
                    try
                    {
                        mod.ApplyConfiguration();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.ToExpandedString());
                    }
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
