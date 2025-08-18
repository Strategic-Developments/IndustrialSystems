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

namespace IndustrialSystems
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, Priority = int.MinValue)]
    public class IndustrialSystemsSessionComponent : MySessionComponentBase
    {
        public static IndustrialSystemsSessionComponent Instance;
        public Guid StorageGuid;
        public override void LoadData()
        {
            // todo: get a new one
            StorageGuid = new Guid("75afa715-498d-4b33-a486-ccc05aa364b9");
            
            Instance = this;

            IndustrialSystemManager.Load();

            MyAPIGateway.Utilities.RegisterMessageHandler(MessageHandlerId, OnModMessageRecieved);
            
        }
        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            
            Network.Load();
            
            Network.OnMessageReceived += OnNetworkMessageRecieved;
        }
        private void OnModMessageRecieved(object obj)
        {
            if (obj is object[])
            {
                object[] objects = (object[])obj;
                ISTypes type = (ISTypes)objects[0];

                Definition def = null;
                switch (type)
                {
                    case ISTypes.Material:
                        MaterialDefinition mdef = MaterialDefinition.ConvertFromObjectArray(objects);
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
                        break;
                    case ISTypes.Drill:
                        def = DrillDefinition.ConvertFromObjectArray(objects);
                        break;
                    case ISTypes.GasRefiner:
                        def = GasRefinerDefinition.ConvertFromObjectArray(objects);
                        break;
                    case ISTypes.ResourceModifier:
                        def = ResourceModifierDefinition.ConvertFromObjectArray(objects);
                        break;
                    case ISTypes.Output:
                        def = OutputDefinition.ConvertFromObjectArray(objects);
                        break;
                    case ISTypes.Smelter:
                        def = SmelterDefinition.ConvertFromObjectArray(objects);
                        break;
                }

                if (def != null)
                {
                    Config.I.BlockDefinitions.Add(def.Base.SubtypeId, def);
                }
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
