using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game.ModAPI.Interfaces;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;
using Sandbox.Game.EntityComponents;
using Sandbox.Common.ObjectBuilders;
using VRage.ObjectBuilders;
using VRage.Game.Models;
using VRage.Render.Particles;
using System.Linq.Expressions;
using System.IO;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.Weapons;
using VRage;
using VRage.Collections;
using VRage.Voxels;
using ProtoBuf;
using System.Collections.Concurrent;
using VRage.Serialization;
using Sandbox.Engine.Physics;
using Sandbox.Game.GameSystems;
using System.Data;
using IndustrialSystems.Utilities;
using IndustrialSystems.Networking;
using Sandbox.Game.Entities.Cube;
using IndustrialSystems.Shared;
using IndustrialSystems.Definitions;
using static IndustrialSystems.Definitions.DefinitionConstants;
using VRage.Game.ObjectBuilders.Definitions;
using ModularAssemblies;
using ModularAssemblies.Communication;

namespace IndustrialSystems
{
    using HiAristeas = DefinitionDefs.ModularDefinitionContainer;
    using IHopeYouLikeWhatIveDone = DefinitionDefs.ModularPhysicalDefinition;
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, Priority = 0)]
    public class IndustrialSystemsSessionComponent : MySessionComponentBase
    {
        public static IndustrialSystemsSessionComponent Instance;
        public ModularDefinitionApi ModularApi;
        public Guid StorageGuid;
        public override void LoadData()
        {
            // todo: get a new one
            StorageGuid = new Guid("75afa715-498d-4b33-a486-ccc05aa364b9");
            
            Instance = this;

            IndustrialSystemManager.Load();
            Network.Load();
            Config.Load();

            Network.OnMessageReceived += OnNetworkMessageRecieved;
            MyAPIGateway.Utilities.RegisterMessageHandler(MessageHandlerId, OnModMessageRecieved);

            ModularApi.Init(ModContext, null);
            
        }
        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            Config.I.InitializeResourceVector();

            if (!ModularApi.IsReady)
                throw new Exception();

            ModularApi.RegisterDefinitions(new HiAristeas
            {
                PhysicalDefs = new[]
                {
                    new IHopeYouLikeWhatIveDone
                    {
                        // Unique name of the definition.
                        Name = "IndustrialSystem",

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

                        AllowedBlockSubtypes = Config.I.BlockDefinitions.Keys.ToArray(),

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
                    }
                }
            });
        }
        // Priority should ensure these are all called between LoadData and Init
        private void OnModMessageRecieved(object obj)
        {
            if (obj is object[])
            {
                object[] objects = (object[])obj;
                ISTypes type = (ISTypes)objects[0];

                // the trolling is immense with this one
                switch (type)
                {
                    case ISTypes.MaterialArray:
                        foreach (var mdefa in MaterialDefinitions.ConvertFromObjectArray(objects))
                            ProcessMaterialDef(mdefa);
                        break;
                    case ISTypes.Material:
                        MaterialDefinition mdef = MaterialDefinition.ConvertFromObjectArray(objects);
                        ProcessMaterialDef(mdef);
                        break;
                    case ISTypes.Drill:
                        var ddef = DrillDefinition.ConvertFromObjectArray(objects);
                        Config.I.BlockDefinitions.Add(ddef.Base.SubtypeId, ddef);
                        break;
                    case ISTypes.GasRefiner:
                        var gdef = GasRefinerDefinition.ConvertFromObjectArray(objects);
                        Config.I.BlockDefinitions.Add(gdef.Base.SubtypeId, gdef);
                        break;
                    case ISTypes.ResourceModifier:
                        var rdef = ResourceModifierDefinition.ConvertFromObjectArray(objects);
                        Config.I.BlockDefinitions.Add(rdef.Base.SubtypeId, rdef);
                        break;
                    case ISTypes.Output:
                        var odef = OutputCargoDefinition.ConvertFromObjectArray(objects);
                        Config.I.BlockDefinitions.Add(odef.Base.SubtypeId, odef);
                        break;
                    case ISTypes.Smelter:
                        var sdef = SmelterDefinition.ConvertFromObjectArray(objects);
                        Config.I.BlockDefinitions.Add(sdef.Base.SubtypeId, sdef);
                        break;
                    case ISTypes.ConveyorArray:
                        foreach (var cdefa in ConveyorDefinitions.ConvertFromObjectArray(objects))
                            Config.I.BlockDefinitions.Add(cdefa.Base.SubtypeId, cdefa);
                        break;
                    case ISTypes.Conveyor:
                        var cdef = ConveyorDefinition.ConvertFromObjectArray(objects);
                        Config.I.BlockDefinitions.Add(cdef.Base.SubtypeId, cdef);
                        break;
                }
            }
        }

        private void ProcessMaterialDef(MaterialDefinition mdef)
        {
            if (mdef.Material.IsMinedOre)
            {
                Config.I.MaterialOreDefinitions.Add(mdef.Base.SubtypeId, mdef);
            }
            else
            {
                Config.I.MaterialVoxelDefinitions.Add(
                    MyDefinitionManager.Static.GetVoxelMaterialDefinition(mdef.Base.SubtypeId).Index,
                    mdef);
            }
        }

        private void OnNetworkMessageRecieved(Packet packet, ulong SenderId, bool fromServer)
        {
            
        }

        public override void UpdateBeforeSimulation()
        {
            IndustrialSystemManager.I.Update();
        }
        protected override void UnloadData()
        {
            MyAPIGateway.Utilities.UnregisterMessageHandler(MessageHandlerId, OnModMessageRecieved);
            Network.OnMessageReceived -= OnNetworkMessageRecieved;
            Network.Unload();
            Instance = null;
        }
        public override void SaveData()
        {
            
        }
    }
}
