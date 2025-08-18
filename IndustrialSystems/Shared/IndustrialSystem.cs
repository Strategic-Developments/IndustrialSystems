using IndustrialSystems.Definitions;
using IndustrialSystems.Shared.Blocks;
using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game.ModAPI;
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
        public HashSet<IIndustrialSystemMachine> AllMachines;

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
                if (blockdef is ConveyorDefinition)
                {
                    var nearby = IndustrialSystemsSessionComponent.Instance.ModularApi.GetConnectedBlocks(b, "IndustrialSystems", false);

                    foreach (var near in nearby)
                    {
                        var nearbyDef = Config.I.BlockDefinitions.GetValueOrDefault(near.BlockDefinition.SubtypeName, null);

                        //if 
                    }
                }
                else if (blockdef is DrillDefinition && b is IMyFunctionalBlock)
                {
                    var drill = new Drill((DrillDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    UpdateableBlocks.Add(drill);
                    AllMachines.Add(drill);
                }
                else if (blockdef is OutputCargoDefinition && b is IMyCargoContainer)
                {
                    var cargo = new OutputCargo((OutputCargoDefinition)blockdef, (IMyCargoContainer)b, this);
                    UpdateableBlocks.Add(cargo);
                    AllMachines.Add(cargo);
                }
                else if (blockdef is ResourceModifierDefinition && b is IMyFunctionalBlock)
                {
                    var output = new ResourceModifier((ResourceModifierDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    UpdateableBlocks.Add(output);
                    AllMachines.Add(output);
                }
                else if (blockdef is SmelterDefinition && b is IMyFunctionalBlock)
                {
                    var smelter = new Smelter((SmelterDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    UpdateableBlocks.Add(smelter);
                    AllMachines.Add(smelter);
                }
                else if (blockdef is GasRefinerDefinition && b is IMyFunctionalBlock)
                {
                    // todo
                    //var output = new GasRefiner((GasRefinerDefinition)blockdef, (IMyFunctionalBlock)b, this);
                    //UpdateableBlocks.Add(output);
                    //AllMachines.Add(output);
                }
            }
        }

        public void RemovePart(IMyCubeBlock b)
        {
            IIndustrialSystemMachine machineToKill = null;
            foreach (var machine in AllMachines)
            {
                if (machine.IsBlockAPartOf(b))
                {
                    machineToKill = machine;
                    machine.Close();
                    break;
                }
            }

            AllMachines.Remove(machineToKill);
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
