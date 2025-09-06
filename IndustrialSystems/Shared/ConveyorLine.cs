using IndustrialSystems.Definitions;
using IndustrialSystems.Shared.Blocks;
using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class ConveyorLine : IIndustrialSystemMachine
    {
        public ushort Frequency;
        public IndustrialSystem parentSystem;

        public IIndustrialSystemMachine Destination;
        public Vector3I DestinationPos;
        public IIndustrialSystemMachine Start;
        public Vector3I StartPos;

        public List<IMyCubeBlock> Path;
        public List<ConveyorItem> Items;

        public ushort DistanceToInsertAtStart;

        public ConveyorLine(ushort frequency, List<IMyCubeBlock> blocks, IndustrialSystem parentSystem)
        {
            Frequency = frequency;
            Path = blocks;
            Items = new List<ConveyorItem>();

            DistanceToInsertAtStart = (ushort)(frequency * blocks.Count);
            this.parentSystem = parentSystem;
            RecomputeNeighbors();
        }

        public void Close()
        {
            if (Start != null && Start is ConveyorLine)
            {
                ((ConveyorLine)Start).Destination = null;
            }
            if (Destination != null && Destination is ConveyorLine)
            {
                ((ConveyorLine)Destination).Start = null;
            }
            Path = null;
            Items = null;
        }

        public void RecomputeNeighbors()
        {
            if (Path.Count == 0)
                return;

            IMyCubeBlock startConv = Path[0];
            IMyCubeBlock endConv = Path[Path.Count - 1];

            ConveyorDefinition startDef = (ConveyorDefinition)Config.I.BlockDefinitions[startConv.BlockDefinition.SubtypeId];
            ConveyorDefinition endDef = (ConveyorDefinition)Config.I.BlockDefinitions[endConv.BlockDefinition.SubtypeId];

            StartPos = startConv.TransformVector(endDef.Conveyors.Connections[1]);
            DestinationPos = endConv.TransformVector(startDef.Conveyors.Connections[0]);

            IMySlimBlock startBlock = startConv.CubeGrid.GetCubeBlock(StartPos);
            IMySlimBlock endBlock = startConv.CubeGrid.GetCubeBlock(DestinationPos);

            IIndustrialSystemMachine otherMachine;
            if (startBlock?.FatBlock != null && parentSystem.AllMachines.TryGetValue(startBlock.FatBlock.EntityId, out otherMachine))
            {
                if (otherMachine is IConveyorJunction)
                {
                    var junction = (IConveyorJunction)otherMachine;
                    if (junction.Link(this, startConv, false))
                    {
                        Start = otherMachine;
                        if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                            ChatCommands.ShowMessage("linked junction start");
                    }
                }
                else
                {
                    if (!(otherMachine is ConveyorLine))
                        Start = otherMachine;

                    if (otherMachine is ConveyorLine &&
                        ((ConveyorLine)otherMachine).DestinationPos == (startConv.Min + startConv.Max) / 2)
                    {
                        Start = otherMachine;
                        ((ConveyorLine)otherMachine).Destination = this;
                    }


                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage($"linked machine start - {otherMachine.GetType().Name}");
                }
            }

            if (endBlock?.FatBlock != null && parentSystem.AllMachines
                .TryGetValue(endBlock.FatBlock.EntityId, out otherMachine))
            {
                if (otherMachine is IConveyorJunction)
                {
                    var junction = (IConveyorJunction)otherMachine;
                    if (junction.Link(this, startConv, true))
                    {
                        Destination = otherMachine;
                        if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                            ChatCommands.ShowMessage("linked junction dest");
                    }
                }
                else
                {
                    if (!(otherMachine is ConveyorLine))
                        Destination = otherMachine;

                    if (otherMachine is ConveyorLine &&
                        ((ConveyorLine)otherMachine).StartPos == (endConv.Min + endConv.Max) / 2)
                    {
                        Destination = otherMachine;
                        ((ConveyorLine)otherMachine).Start = this;
                    }
                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage($"linked machine dest - {otherMachine.GetType().Name}");
                }
            }
        }
        public Vector3D GetPosition()
        {
            if (Path.Count == 0)
                return Vector3D.Zero;

            return Path[0].GetPosition();
        }

    }
}
