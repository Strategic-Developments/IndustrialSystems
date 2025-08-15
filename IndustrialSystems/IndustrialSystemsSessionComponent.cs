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
using IndustrialSystems.Definitions;
using Sandbox.Game.Entities.Cube;
using IndustrialSystems.Shared;

namespace IndustrialSystems
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class IndustrialSystemsSessionComponent : MySessionComponentBase
    {
        public static IndustrialSystemsSessionComponent Instance;
        public Guid StorageGuid;
        public override void LoadData()
        {
            // todo: get a new one
            //StorageGuid = new Guid("");
            
            Instance = this;

            IndustrialSystemManager.Load();

            MyAPIGateway.Utilities.RegisterMessageHandler(DefinitionConstants.MessageHandlerId, OnModMessageRecieved);
            
        }
        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            
            Network.Load();
            
            Network.OnMessageReceived += OnNetworkMessageRecieved;
        }
        private void OnModMessageRecieved(object obj)
        {
            
        }
        private void OnNetworkMessageRecieved(ushort ChannelId, Packet packet, ulong SenderId, bool fromServer)
        {
            
        }

        public override void UpdateBeforeSimulation()
        {
            IndustrialSystemManager.I.Update();
        }
        protected override void UnloadData()
        {
            MyAPIGateway.Utilities.UnregisterMessageHandler(DefinitionConstants.MessageHandlerId, OnModMessageRecieved);
            Network.OnMessageReceived -= OnNetworkMessageRecieved;
            Network.Unload();
            Instance = null;
        }
        public override void SaveData()
        {
            
        }
    }
}
