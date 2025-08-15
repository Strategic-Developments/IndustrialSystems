using System;
using System.Collections.Generic;
using IndustrialSystems.Server;
using Sandbox.ModAPI;
using VRageMath;
using static ModularAssemblies.Communication.DefinitionDefs;

namespace ModularAssemblies
{
    /* Hey there modders!
     *
     * This file is a *template*. Make sure to keep up-to-date with the latest version, which can be found at https://github.com/StarCoreSE/Modular-Assemblies-Client-Mod-Template.
     *
     * If you're just here for the API, head on over to https://github.com/StarCoreSE/Modular-Assemblies/wiki/The-Modular-API for a (semi) comprehensive guide.
     *
     */
    internal partial class ModularDefinition
    {
        // You can declare functions in here, and they are shared between all other ModularDefinition files.
        // However, for all but the simplest of assemblies it would be wise to have a separate utilities class.

        // This is the important bit.
        internal ModularPhysicalDefinition ConveyorNetwork => new ModularPhysicalDefinition
        {
            // Unique name of the definition.
            Name = "ConveyorNetwork",

            //OnInit = () =>
            //{
            //    
            //},

            // Triggers whenever a new part is added to an assembly.
            OnPartAdd = IndustrialSystemManager.I.OnPartAdd,

            // Triggers whenever a part is removed from an assembly.
            OnPartRemove = IndustrialSystemManager.I.OnPartRemove,

            // Triggers whenever a part is destroyed, just after OnPartRemove.
            //OnPartDestroy = (assemblyId, block, isBasePart) =>
            //{
            // You can remove this function, and any others if need be.
            //MyAPIGateway.Utilities.ShowMessage("Modular Assemblies", $"ExampleDefinition.OnPartDestroy called.\nI hope the explosion was pretty.");
            //MyAPIGateway.Utilities.ShowNotification("Assembly has " + ModularApi.GetMemberParts(assemblyId).Length + " blocks.");
            //},

            OnAssemblyClose = IndustrialSystemManager.I.OnAssemblyDestroy,

            // Optional - if this is set, an assembly will not be created until a baseblock exists.
            // 
            BaseBlockSubtype = null,

            // All SubtypeIds that can be part of this assembly.
            AllowedBlockSubtypes = new[]
            {
                "is_large_conveyor",
                "is_large_conveyor_corner",
                "is_large_conveyor_t",
                "is_large_conveyor_x",
            },

            // Allowed connection directions & whitelists, measured in blocks.
            // If an allowed SubtypeId is not included here, connections are allowed on all sides.
            // If the connection type whitelist is empty, all allowed subtypes may connect on that side.
            AllowedConnections = new Dictionary<string, Dictionary<Vector3I, string[]>>
            {
                ["is_large_conveyor"] = new Dictionary<Vector3I, string[]>
                {
                    // In this definition, a small reactor can only connect on faces with conveyors.
                    [Vector3I.Up] = Array.Empty<string>(), // Build Info is really handy for checking directions.
                    [Vector3I.Down] = Array.Empty<string>(),
                },
                ["is_large_conveyor_corner"] = new Dictionary<Vector3I, string[]>
                {
                    // In this definition, a small reactor can only connect on faces with conveyors.
                    [Vector3I.Backward] = Array.Empty<string>(), // Build Info is really handy for checking directions.
                    [Vector3I.Down] = Array.Empty<string>(),
                },
                ["is_large_conveyor_t"] = new Dictionary<Vector3I, string[]>
                {
                    // In this definition, a small reactor can only connect on faces with conveyors.
                    [Vector3I.Left] = Array.Empty<string>(), // Build Info is really handy for checking directions.
                    [Vector3I.Right] = Array.Empty<string>(),
                    [Vector3I.Down] = Array.Empty<string>(),
                },
                ["is_large_conveyor_x"] = new Dictionary<Vector3I, string[]>
                {
                    // In this definition, a small reactor can only connect on faces with conveyors.
                    [Vector3I.Left] = Array.Empty<string>(), // Build Info is really handy for checking directions.
                    [Vector3I.Right] = Array.Empty<string>(),
                    [Vector3I.Up] = Array.Empty<string>(),
                    [Vector3I.Down] = Array.Empty<string>(),
                },
            },
        };
    }
}
