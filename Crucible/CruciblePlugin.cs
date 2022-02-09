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
    using BepInEx;
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    /// <summary>
    /// BepInEx plugin for Crucible Config mods.
    /// </summary>
    [BepInPlugin("net.RoboPhredDev.PotionCraft.Crucible", "Crucible Modding Framework", "1.1.0.0")]
    public class CruciblePlugin : BaseUnityPlugin
    {
        private ICollection<CruciblePackageMod> mods;

        /// <summary>
        /// Called by unity when the plugin loads.
        /// </summary>
        public void Awake()
        {
            CrucibleGameEvents.OnGameInitialized += (_, __) =>
            {
                // Load mods after the game is initialized, so bepinex plugins have all loaded.
                // This is because various config mods might rely on yaml deserializers provided by those plugins.
                CrucibleLog.Log("Loading Crucible mods.");
                this.mods = LoadAllConfigMods();

                CrucibleLog.Log("Activating Crucible mods.");
                foreach (var mod in this.mods)
                {
                    ActivateMod(mod);
                }
            };

            CrucibleGameEvents.OnSaveLoaded += (_, e) =>
            {
                var saveData = e.SaveFile.GetSaveData<CruciblePluginSaveData>();

                if (saveData.InstalledCruciblePackageIDs != null)
                {
                    var missingMods = new List<string>();
                    foreach (var package in saveData.InstalledCruciblePackageIDs)
                    {
                        if (!this.mods.Any(m => m.ID == package))
                        {
                            missingMods.Add(package);
                        }
                    }

                    var newModCount = this.mods.Count(x => !saveData.InstalledCruciblePackageIDs.Contains(x.ID));

                    if (missingMods.Count > 0)
                    {
                        Notification.ShowText("Missing Crucible Packages", $"Some mods that this save relies on were not found: {string.Join(", ", missingMods)}.", Notification.TextType.EventText);
                    }
                    else if (newModCount > 0)
                    {
                        Notification.ShowText("New Crucible Packages", $"{newModCount} new {(newModCount != 1 ? "mods were" : "mod was")} installed.", Notification.TextType.EventText);
                    }

                }
            };

            CrucibleGameEvents.OnSaveSaved += (_, e) =>
            {
                e.SaveFile.SetSaveData(new CruciblePluginSaveData()
                {
                    CrucibleCoreVersion = "1.0.0.0",
                    InstalledCruciblePackageIDs = this.mods.Select(m => m.ID).ToList(),
                });
            };
        }

        private static void ActivateMod(CruciblePackageMod mod)
        {
            try
            {
                mod.EnsureDependenciesMet();
                mod.ApplyConfiguration();
            }
            catch (CrucibleMissingDependencyException ex)
            {
                Debug.Log(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToExpandedString());
            }
        }

        private static ICollection<CruciblePackageMod> LoadAllConfigMods()
        {
            var mods = new List<CruciblePackageMod>();

            var path = Path.Combine("crucible", "mods");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return mods;
            }

            var directories = Directory.GetDirectories(path);
            var directoryMods = directories.Select(folder => TryLoadDirectoryMod(folder)).Where(x => x != null);
            mods.AddRange(directoryMods);

            var zipFiles = Directory.GetFiles(path, "*.zip");
            var zipMods = zipFiles.Select(file => TryLoadFileMod(file)).Where(x => x != null);
            mods.AddRange(zipMods);

            return mods;
        }

        private static CruciblePackageMod TryLoadDirectoryMod(string modFolder)
        {
            try
            {
                var mod = CruciblePackageMod.LoadFromFolder(modFolder);
                CrucibleLog.Log($"> Loaded mod \"{mod.Name}\".");
                return mod;
            }
            catch (CruciblePackageModLoadException ex)
            {
                CrucibleLog.Log($"Failed to load mod at \"{modFolder}\": {ex.ToExpandedString()}");
                return null;
            }
        }

        private static CruciblePackageMod TryLoadFileMod(string zipFilePath)
        {
            try
            {
                var mod = CruciblePackageMod.LoadFromZip(zipFilePath);
                CrucibleLog.Log($"> Loaded mod \"{mod.Name}\".");
                return mod;
            }
            catch (CruciblePackageModLoadException ex)
            {
                CrucibleLog.Log($"Failed to load mod at \"{zipFilePath}\": {ex.ToExpandedString()}");
                return null;
            }
        }
    }
}
