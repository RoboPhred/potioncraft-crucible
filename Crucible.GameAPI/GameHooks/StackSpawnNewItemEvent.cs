// <copyright file="StackSpawnNewItemEvent.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using ObjectBased.Stack;
    using UnityEngine;

    /// <summary>
    /// An event raised when a new stack item is spawning.
    /// </summary>
    public static class StackSpawnNewItemEvent
    {
        private static bool isInitialized;
        private static EventHandler<StackSpawnNewItemEventArgs> onSpawnNewItemPreInitialize;

        /// <summary>
        /// Raised when a new stack item is spawning.
        /// </summary>
        public static event EventHandler<StackSpawnNewItemEventArgs> OnSpawnNewItemPreInititialize
        {
            add
            {
                EnsurePatch();
                onSpawnNewItemPreInitialize += value;
            }

            remove
            {
                onSpawnNewItemPreInitialize -= value;
            }
        }

        private static void EnsurePatch()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;

            var spawnNewItemStackMethod = AccessTools.Method(typeof(Stack), "SpawnNewItemStack");
            if (spawnNewItemStackMethod != null)
            {
                var transpileMethod = AccessTools.Method(typeof(StackSpawnNewItemEvent), nameof(Transpile));
                HarmonyInstance.Instance.Patch(spawnNewItemStackMethod, transpiler: new HarmonyMethod(transpileMethod));
            }
            else
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate Stack SpawnNewItemStack method!");
            }
        }

        private static void OnSpawnNewItemPreInitialize(Stack stack)
        {
            var e = new StackSpawnNewItemEventArgs(stack);
            onSpawnNewItemPreInitialize?.Invoke(null, e);
        }

        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
        {
            var setTransformPositionPropSetter = AccessTools.PropertySetter(typeof(Transform), nameof(Transform.position));
            var onSpawnNewItemPreInitialize = AccessTools.Method(typeof(StackSpawnNewItemEvent), nameof(OnSpawnNewItemPreInitialize));
            var found = false;
            foreach (var instruction in instructions)
            {
                // We want to call an event after the object is set up but before initialize is called.
                if (!found && instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == setTransformPositionPropSetter)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_0); // stack
                    yield return new CodeInstruction(OpCodes.Call, onSpawnNewItemPreInitialize);
                }

                yield return instruction;
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject Stack SpawnNewItemStack event caller!");
            }
        }
    }
}
