using IndustrialSystems.Definitions;
using IndustrialSystems.Shared.Blocks;
using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
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

        public IIndustrialSystemMachine Destination;
        public IIndustrialSystemMachine Start;

        public List<IMyCubeBlock> Path;
        public List<ConveyorItem> Items;

        public ushort DistanceToInsertAtStart;

        public ConveyorLine(ushort frequency, List<IMyCubeBlock> blocks)
        {
            Frequency = frequency;
            Path = blocks;
            Items = new List<ConveyorItem>();

            DistanceToInsertAtStart = (ushort)(frequency * blocks.Count);
            ComputeNeighbors();
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
        }

        public void ComputeNeighbors()
        {
            if (Path.Count == 0)
                return;

            IMyCubeBlock startConv = Path[0];
            IMyCubeBlock endConv = Path[Path.Count - 1];

            ConveyorDefinition startDef = (ConveyorDefinition)Config.I.BlockDefinitions[startConv.BlockDefinition.SubtypeId];
            ConveyorDefinition endDef = (ConveyorDefinition)Config.I.BlockDefinitions[endConv.BlockDefinition.SubtypeId];

            Vector3I transformedStartVector = startConv.TransformVector(startDef.Conveyors.Connections[0]);
            Vector3I transformedEndVector = endConv.TransformVector(endDef.Conveyors.Connections[1]);

            IMySlimBlock startBlock = startConv.CubeGrid.GetCubeBlock(transformedStartVector);
            IMySlimBlock endBlock = startConv.CubeGrid.GetCubeBlock(transformedEndVector);

            IIndustrialSystemMachine otherMachine;
            if (startBlock?.FatBlock != null && IndustrialSystemManager.I.AllMachines.TryGetValue(startBlock.FatBlock.EntityId, out otherMachine))
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
                    Start = otherMachine;

                    if (otherMachine is ConveyorLine)
                    {
                        ((ConveyorLine)otherMachine).Destination = this;
                    }


                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage($"linked machine start - {otherMachine.GetType().Name}");
                }
            }

            if (endBlock?.FatBlock != null && IndustrialSystemManager.I.AllMachines.TryGetValue(endBlock.FatBlock.EntityId, out otherMachine))
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
                    Destination = otherMachine;

                    if (otherMachine is ConveyorLine)
                    {
                        ((ConveyorLine)otherMachine).Start = this;
                    }
                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage($"linked machine dest - {otherMachine.GetType().Name}");
                }
            }
        }

        
    }
}
