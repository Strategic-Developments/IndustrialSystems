using IndustrialSystems.Definitions;
using IndustrialSystems.Shared.Blocks;
using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using Sandbox.ModAPI;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRageMath;
using VRageRender;
using VRageRender.Messages;

namespace IndustrialSystems.Shared
{
    public class IndustrialSystem
    {
        public int ModularId;

        public HashSet<ConveyorLine> AllConveyorLines;
        public HashSet<ConveyorLine> BackConveyorLines;

        public HashSet<Pipeline> AllPipes;

        public HashSet<IUpdateable> UpdateableBlocks;

        public IndustrialSystem(int modularId)
        {
            this.ModularId = modularId;

            AllConveyorLines = new HashSet<ConveyorLine>();
            BackConveyorLines = new HashSet<ConveyorLine>();
            AllPipes = new HashSet<Pipeline>();
            UpdateableBlocks = new HashSet<IUpdateable>();
        }



        public void AddPart(IMyCubeBlock b)
        {
            Definition blockdef = Config.I.BlockDefinitions.GetValueOrDefault(b.BlockDefinition.SubtypeName, null);
            if (blockdef != null)
            {
                bool BackConveyorsNeedRecompute = false;

                IIndustrialSystemMachine newMachine = null;
                IUpdateable updatable = null;
                if (blockdef is ConveyorDefinition)
                {
                    var conveyordef = (ConveyorDefinition)blockdef;

                    if (conveyordef.Conveyors.IsConveyor)
                    {
                        BackConveyorsNeedRecompute = true;
                        // TODO: Actually optimize this
                        var Conveyor = new ConveyorLine(conveyordef.Conveyors.ItemTravelFrequency, new List<IMyCubeBlock> { b });
                        AllConveyorLines.Add(Conveyor);

                        newMachine = Conveyor;

                        if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                            ChatCommands.ShowMessage("Added conveyor");
                    }
                    else if (conveyordef.Conveyors.IsSplitter)
                    {
                        newMachine = new ConveyorSplitter(b, this, conveyordef.Conveyors);
                        if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                            ChatCommands.ShowMessage("Added splitter");
                    }
                    else if (conveyordef.Conveyors.IsMerger)
                    {
                        newMachine = new ConveyorMerger(b, this, conveyordef.Conveyors);
                        if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                            ChatCommands.ShowMessage("Added merger");
                    }
                    else if(IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                    {
                        ChatCommands.ShowMessage($"Conveyor add fail! {conveyordef.Conveyors.Connections.Length}");
                    }
                }
                else if (blockdef is DrillDefinition && b is IMyFunctionalBlock)
                {
                    updatable = new Drill((DrillDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    newMachine = updatable;

                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage("Added drill");
                }
                else if (blockdef is OutputCargoDefinition && b is IMyCargoContainer)
                {
                    updatable = new OutputCargo((OutputCargoDefinition)blockdef, (IMyCargoContainer)b, this);
                    newMachine = updatable;

                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage("Added output cargo");
                }
                else if (blockdef is ResourceModifierDefinition && b is IMyFunctionalBlock)
                {
                    updatable = new ResourceModifier((ResourceModifierDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    newMachine = updatable;

                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage("Added resource modifier");
                }
                else if (blockdef is SmelterDefinition && b is IMyFunctionalBlock)
                {
                    updatable = new Smelter((SmelterDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    newMachine = updatable;

                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage("Added smelter");
                }
                else if (blockdef is GasRefinerDefinition && b is IMyFunctionalBlock)
                {
                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage("Added gas refiner");
                    // todo
                    //var output = new GasRefiner((GasRefinerDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    //UpdateableBlocks.Add(output);
                    //AllMachines.Add(output);
                }
                else
                {
                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage($"Add fail {blockdef.GetType().Name}");
                }

                if (updatable != null)
                    UpdateableBlocks.Add(updatable);
                IndustrialSystemManager.I.AllMachines.Add(b.EntityId, newMachine);

                var nearby = IndustrialSystemsSessionComp.I.ModularApi.GetConnectedBlocks(b, "IndustrialSystems", false);
                foreach (var near in nearby)
                {
                    IIndustrialSystemMachine machine = null;
                    if (IndustrialSystemManager.I.AllMachines.TryGetValue(near.EntityId, out machine) && machine is ConveyorLine)
                    {
                        BackConveyorsNeedRecompute = true;
                        ((ConveyorLine)machine).ComputeNeighbors();
                    }
                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage("neighbor");
                }
                if (BackConveyorsNeedRecompute)
                {
                    BackConveyorLines.Clear();
                    foreach (var c in AllConveyorLines.Where((co) => !(co.Destination is ConveyorLine)))
                    {
                        BackConveyorLines.Add(c);
                    }
                }
            }
            else if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
            {
                ChatCommands.ShowMessage($"Failed to add {b.BlockDefinition.SubtypeName}");
            }
        }

        

        public void RemovePart(IMyCubeBlock b)
        {
            IIndustrialSystemMachine machineToKill = null;
            if (IndustrialSystemManager.I.AllMachines.TryGetValue(b.EntityId, out machineToKill))
            {
                machineToKill.Close();
                IndustrialSystemManager.I.AllMachines.Remove(b.EntityId);
                if (machineToKill is IUpdateable)
                    UpdateableBlocks.Remove((IUpdateable)machineToKill);
                else if (machineToKill is ConveyorLine)
                {
                    AllConveyorLines.Remove((ConveyorLine)machineToKill);
                    BackConveyorLines.Remove((ConveyorLine)machineToKill);
                }
                else if (machineToKill is Pipeline)
                {
                    AllPipes.Remove((Pipeline)machineToKill);
                }
            }
        }
    }
}
