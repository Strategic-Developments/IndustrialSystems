using IndustrialSystems.Definitions;
using IndustrialSystems.Shared.Blocks;
using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;
using VRageRender;
using VRageRender.Messages;

namespace IndustrialSystems.Shared
{
    public class IndustrialSystem
    {
        public IMyCubeGrid Grid;

        public HashSet<ConveyorLine> AllConveyorLines;
        public HashSet<ConveyorLine> BackConveyorLines;

        public HashSet<Pipeline> AllPipes;

        public HashSet<IUpdateable> UpdateableBlocks;
        public Dictionary<long, IIndustrialSystemMachine> AllMachines;
        public IndustrialSystem(IMyCubeGrid grid)
        {
            Grid = grid;

            AllConveyorLines = new HashSet<ConveyorLine>();
            BackConveyorLines = new HashSet<ConveyorLine>();
            AllPipes = new HashSet<Pipeline>();
            UpdateableBlocks = new HashSet<IUpdateable>();
            AllMachines = new Dictionary<long, IIndustrialSystemMachine>();

            Grid.OnBlockRemoved += OnBlockRemoved;
            Grid.OnClose += OnClose;
            Grid.OnBlockAdded += OnBlockAdd;

            Grid.GetBlocks(null, OnBlockAdded);
        }

        private void OnClose(IMyEntity ent)
        {
            Grid.OnBlockRemoved -= OnBlockRemoved;
            Grid.OnClose -= OnClose;
            Grid.OnBlockAdded -= OnBlockAdd;

            AllConveyorLines = null;
            BackConveyorLines = null;
            AllPipes = null;
            UpdateableBlocks = null;
            AllMachines = null;

            IndustrialSystemManager.I.Systems.Remove(ent.EntityId);
        }
        private bool OnBlockAdded(IMySlimBlock obj)
        {
            OnBlockAdd(obj);
            return false;
        }
        private void OnBlockAdd(IMySlimBlock obj)
        {
            IMyCubeBlock b = obj.FatBlock;

            if (b == null)
                return;


            Definition blockdef;
            if (Config.I.BlockDefinitions.TryGetValue(b.BlockDefinition.SubtypeId, out blockdef))
            {
                IIndustrialSystemMachine other;
                if (AllMachines.TryGetValue(b.EntityId, out other))
                {
                    RemoveBlock(b, other);
                }

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
                        var Conveyor = new ConveyorLine(conveyordef.Conveyors.ItemTravelFrequency, new List<IMyCubeBlock> { b }, this);
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
                    else if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
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
                AllMachines.Add(b.EntityId, newMachine);

                var neighbors = IndustrialSystemManager.I.SlimBlockObjectPool.Pop();
                b.SlimBlock.GetNeighbours(neighbors);
                foreach (var nearBlock in neighbors)
                {
                    var near = nearBlock.FatBlock;

                    if (near == null)
                        continue;

                    IIndustrialSystemMachine machine = null;
                    if (AllMachines.TryGetValue(near.EntityId, out machine))
                    {
                        if (machine is ConveyorLine)
                        {
                            BackConveyorsNeedRecompute = true;
                            ((ConveyorLine)machine).RecomputeNeighbors();
                        }
                    }
                    if (IndustrialSystemsSessionComp.I.DebugLevel >= 1)
                        ChatCommands.ShowMessage("neighbor");
                }
                neighbors.Clear();
                IndustrialSystemManager.I.SlimBlockObjectPool.Push(neighbors);


                if (BackConveyorsNeedRecompute)
                {
                    BackConveyorLines.Clear();
                    foreach (var c in AllConveyorLines.Where((co) => !(co.Destination is ConveyorLine)))
                    {
                        BackConveyorLines.Add(c);
                    }
                }
            }
        }


        public void OnBlockRemoved(IMySlimBlock bl)
        {
            IMyCubeBlock b = bl.FatBlock;
            if (b == null)
                return;

            IIndustrialSystemMachine machineToKill = null;
            if (AllMachines.TryGetValue(b.EntityId, out machineToKill))
            {
                RemoveBlock(b, machineToKill);
            }
        }

        private void RemoveBlock(IMyCubeBlock b, IIndustrialSystemMachine machineToKill)
        {
            machineToKill.Close();
            AllMachines.Remove(b.EntityId);
            if (machineToKill is IUpdateable)
                UpdateableBlocks.Remove((IUpdateable)machineToKill);
            if (machineToKill is ConveyorLine)
            {
                AllConveyorLines.Remove((ConveyorLine)machineToKill);
                BackConveyorLines.Clear();
                foreach (var c in AllConveyorLines.Where((co) => !(co.Destination is ConveyorLine)))
                {
                    BackConveyorLines.Add(c);
                }
            }
            if (machineToKill is Pipeline)
            {
                AllPipes.Remove((Pipeline)machineToKill);
            }
        }
    }
}
