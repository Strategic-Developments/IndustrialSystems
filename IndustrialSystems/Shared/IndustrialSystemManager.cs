using IndustrialSystems.Utilities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class IndustrialSystemManager
    {
        public enum PartType
        {
            None,
            Conveyor,
            Splitter,
            Inserter,
            Inventory,
        }
        

        public static IndustrialSystemManager I;

        public Dictionary<int, IndustrialSystem> Networks;
        public Dictionary<string, Base6Directions.Direction> ISConveyorSubtypes;
        public Dictionary<string, Base6Directions.Direction> ISSplitterSubtypes;
        public static void Load()
        {
            I = new IndustrialSystemManager
            {
                Networks = new Dictionary<int, IndustrialSystem>(),
                ISConveyorSubtypes = new Dictionary<string, Base6Directions.Direction>()
                {
                    ["is_large_conveyor"] = Base6Directions.Direction.Up,
                    ["is_large_conveyor_corner"] = Base6Directions.Direction.Backward,
                    ["is_large_conveyor_t"] = Base6Directions.Direction.Down,
                    ["is_large_conveyor_t_side"] = Base6Directions.Direction.Right,
                    ["is_large_conveyor_x"] = Base6Directions.Direction.Up,
                },
                ISSplitterSubtypes = new Dictionary<string, Base6Directions.Direction>()
                {
                    ["is_large_splitter_x"] = Base6Directions.Direction.Up,
                }
            };
        }
        public static void Unload()
        {
            I = null;
        }
        public PartType GetBlockType(IMyCubeBlock b, out Base6Directions.Direction dir)
        {
            if (I.ISConveyorSubtypes.TryGetValue(b.BlockDefinition.SubtypeName, out dir))
            {
                return PartType.Conveyor;
            }
            else if (I.ISSplitterSubtypes.TryGetValue(b.BlockDefinition.SubtypeName, out dir))
            {
                return PartType.Splitter;
            }

            dir = Base6Directions.Direction.Forward;
            return PartType.None;
        }
        public void OnPartAdd(int assemblyId, IMyCubeBlock block, bool isBasePart)
        {
            if (!Networks.ContainsKey(assemblyId))
                Networks.Add(assemblyId, new IndustrialSystem(assemblyId));

            Networks[assemblyId].AddPart(block);
        }
        public void OnPartRemove(int assemblyId, IMyCubeBlock block, bool isBasePart)
        {
            if (!Networks.ContainsKey(assemblyId))
                return;

            Networks[assemblyId].RemovePart(block);
        }

        public void OnAssemblyDestroy(int assemblyId)
        {
            Networks.Remove(assemblyId);
        }

        public void Update()
        {
            
        }
    }
}
