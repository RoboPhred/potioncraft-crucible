// <copyright file="CrucibleSaveFile.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using SaveFileSystem;
    using UnityEngine;
    using Utils;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Provides access to custom data stored in PotionCraft save files.
    /// </summary>
    public sealed class CrucibleSaveFile : IDisposable
    {
        /// <summary>
        /// The zero-based line offset in the PotionCraft save file where our data is stored.
        /// </summary>
        /// <remarks>
        /// PotionCraft uses line 0 (for metadata) and 1 (for game data).
        /// We give a generous boundary for the game to use, to ensure we do not conflict.
        /// </remarks>
        private const int CrucibleFileLine = 511;

        private readonly File file;

        private bool isDisposed = false;
        private bool isDataLoaded = false;
        private Dictionary<string, string> dataByPluginGuid = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleSaveFile"/> class.
        /// </summary>
        /// <param name="file">The file to read / write data from.</param>
        internal CrucibleSaveFile(File file)
        {
            this.file = file;
        }

        /// <summary>
        /// Gets the save data associated with the caller's BepInEx plugin.
        /// The caller must be an assembly that implements a plugin with the <see cref="BepInPlugin"/> attribute.
        /// </summary>
        /// <remarks>
        /// If the caller does not provide a class with the <see cref="BepInPlugin"/> attribute, this function will throw a <see cref="BepInPluginRequiredException"/> exception.
        /// </remarks>
        /// <returns>The save data associated with the GUID, or <c>null</c> if none was saved.</returns>
        public string GetSaveData()
        {
            var currentPluginGuid = BepInExPluginUtilities.RequirePluginGuidFromAssembly(Assembly.GetCallingAssembly());
            return this.GetSaveData(currentPluginGuid);
        }

        /// <summary>
        /// Gets the save data associated with the caller's BepInEx plugin.
        /// The caller must be an assembly that implements a plugin with the <see cref="BepInPlugin"/> attribute.
        /// </summary>
        /// <remarks>
        /// If the caller does not provide a class with the <see cref="BepInPlugin"/> attribute, this function will throw a <see cref="BepInPluginRequiredException"/> exception.
        /// <p>
        /// This uses Unity's built in json handling, which is very limited.  For complex data, a third party serialization library shoud be used, and the data
        /// should be stored and retrieved as a string.
        /// </p>
        /// </remarks>
        /// <typeparam name="T">The data type to deserialize.</typeparam>
        /// <returns>The deserialized data, or <c>default(T)</c> if no data was saved for the given plugin GUID.</returns>
        public T GetSaveData<T>()
        {
            var currentPluginGuid = BepInExPluginUtilities.RequirePluginGuidFromAssembly(Assembly.GetCallingAssembly());
            return this.GetSaveData<T>(currentPluginGuid);
        }

        /// <summary>
        /// Gets the save data associated with the given plugin GUID.
        /// </summary>
        /// <param name="pluginGuid">The GUID of the plugin to fetch save data for.  This should be a BepInEx Plugin GUID.</param>
        /// <returns>The save data associated with the GUID, or <c>null</c> if none was saved.</returns>
        public string GetSaveData(string pluginGuid)
        {
            this.EnsureNotDisposed();
            this.TryLoadModData();

            return this.dataByPluginGuid[pluginGuid];
        }

        /// <summary>
        /// Deserializes the save data associated with the given plugin GUID.
        /// </summary>
        /// <remarks>
        /// This uses Unity's built in json handling, which is very limited.  For complex data, a third party serialization library shoud be used, and the data
        /// should be stored and retrieved as a string.
        /// </remarks>
        /// <param name="pluginGuid">The GUID of the plugin to fetch save data for.  This should be a BepInEx Plugin GUID.</param>
        /// <typeparam name="T">The data type to deserialize.</typeparam>
        /// <returns>The deserialized data, or <c>default(T)</c> if no data was saved for the given plugin GUID.</returns>
        public T GetSaveData<T>(string pluginGuid)
        {
            this.EnsureNotDisposed();
            this.TryLoadModData();

            if (this.dataByPluginGuid.TryGetValue(pluginGuid, out var jsonData))
            {
                return JsonUtility.FromJson<T>(jsonData);
            }

            return default;
        }

        /// <summary>
        /// Sets the save data for the caller's BepInEx plugin.
        /// The caller must be an assembly that implements a plugin with the <see cref="BepInPlugin"/> attribute.
        /// </summary>
        /// <remarks>
        /// If the caller does not provide a class with the <see cref="BepInPlugin"/> attribute, this function will throw a <see cref="BepInPluginRequiredException"/> exception.
        /// </remarks>
        /// <param name="value">The string to store in the save file for this plugin.</param>
        public void SetSaveData(string value)
        {
            var currentPluginGuid = BepInExPluginUtilities.RequirePluginGuidFromAssembly(Assembly.GetCallingAssembly());
            this.SetSaveData(currentPluginGuid, value);
        }

        /// <summary>
        /// Sets the save data for the caller's BepInEx plugin.
        /// The caller must be an assembly that implements a plugin with the <see cref="BepInPlugin"/> attribute.
        /// </summary>
        /// <remarks>
        /// If the caller does not provide a class with the <see cref="BepInPlugin"/> attribute, this function will throw a <see cref="BepInPluginRequiredException"/> exception.
        /// <p>
        /// This uses Unity's built in json handling, which is very limited.  For complex data, a third party serialization library shoud be used, and the data
        /// should be stored and retrieved as a string.
        /// </p>
        /// </remarks>
        /// <param name="value">The value to store in the save file for this plugin.</param>
        /// <typeparam name="T">The data type to store.</typeparam>
        public void SetSaveData<T>(T value)
        {
            var currentPluginGuid = BepInExPluginUtilities.RequirePluginGuidFromAssembly(Assembly.GetCallingAssembly());
            this.SetSaveData(currentPluginGuid, value);
        }

        /// <summary>
        /// Sets the save data for the given plugin GUID.
        /// </summary>
        /// <param name="pluginGuid">The GUID of the plugin to fetch save data for.  This should be a BepInEx Plugin GUID.</param>
        /// <param name="value">The string data to save for this plugin.</param>
        public void SetSaveData(string pluginGuid, string value)
        {
            this.EnsureNotDisposed();

            // Mod data must be loaded so dataByPluginGuid is available for writing.
            this.TryLoadModData();

            this.dataByPluginGuid[pluginGuid] = value;
        }

        /// <summary>
        /// Stores the serialized object to the save file.
        /// </summary>
        /// <remarks>
        /// This uses Unity's built in json handling, which is very limited.  For complex data, a third party serialization library shoud be used, and the data
        /// should be stored and retrieved as a string.
        /// </remarks>
        /// <param name="pluginGuid">The GUID of the plugin to fetch save data for.  This should be a BepInEx Plugin GUID.</param>
        /// <param name="value">The object to store in the save file.</param>
        /// <typeparam name="T">The data type to store.</typeparam>
        public void SetSaveData<T>(string pluginGuid, T value)
        {
            this.EnsureNotDisposed();

            // Mod data must be loaded so dataByPluginGuid is available for writing.
            this.TryLoadModData();

            this.dataByPluginGuid[pluginGuid] = JsonUtility.ToJson(value);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.TrySaveModData();
            this.isDisposed = true;
        }

        private void EnsureNotDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException($"{nameof(CrucibleSaveFile)} can only be accessed inside the event handler of {nameof(CrucibleGameEvents)}.${nameof(CrucibleGameEvents.OnSaveSaved)}");
            }
        }

        private void TryLoadModData()
        {
            if (this.isDataLoaded)
            {
                return;
            }

            this.isDataLoaded = true;

            var lines = System.IO.File.ReadAllLines(this.file.url);
            if (lines.Length <= CrucibleFileLine)
            {
                this.dataByPluginGuid = new Dictionary<string, string>();
                return;
            }

            var data = lines[CrucibleFileLine];
            if (!string.IsNullOrEmpty(data))
            {
                var decryptedData = Base64.Decode(data);

                // Tried hard to get unity's JsonUtility to serialize the data, but it abhores any sort of collection of elements.
                var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                var saveData = deserializer.Deserialize<CrucibleSaveData>(decryptedData);

                this.dataByPluginGuid = saveData.PluginData;
            }

            if (this.dataByPluginGuid == null)
            {
                this.dataByPluginGuid = new Dictionary<string, string>();
            }
        }

        private void TrySaveModData()
        {
            if (this.dataByPluginGuid == null)
            {
                return;
            }

            // Load the existing data
            var lines = System.IO.File.ReadAllLines(this.file.url);

            // Copy the data to our line array, making enough room to store our data at CrucibleFileLine.
            var saveLines = new string[Math.Max(lines.Length, CrucibleFileLine + 1)];
            Array.Copy(lines, saveLines, lines.Length);

            // Tried hard to get unity's JsonUtility to serialize the data, but it abhores any sort of collection of elements.
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).JsonCompatible().Build();
            var modJsonData = serializer.Serialize(new CrucibleSaveData
            {
                Version = 1,
                PluginData = this.dataByPluginGuid,
            });

            saveLines[CrucibleFileLine] = Base64.Encode(modJsonData);

            // Save the file
            System.IO.File.WriteAllLines(this.file.url, saveLines);
        }

        [Serializable]
        private struct CrucibleSaveData
        {
            public int Version;

            public Dictionary<string, string> PluginData;
        }
    }
}
