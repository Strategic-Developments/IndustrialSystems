using IndustrialSystems.Definitions;
using static IndustrialSystems.Definitions.Definition;
using static IndustrialSystems.Definitions.NameDef;
using static IndustrialSystems.Definitions.GasReqDef;
using static IndustrialSystems.Definitions.BatchJobDef;
using static IndustrialSystems.Definitions.MachineInventoryDef;
using static IndustrialSystems.Definitions.DefinitionConstants;
using static IndustrialSystems.Definitions.DefinitionConstants.ISTypes;
using static IndustrialSystems.Definitions.DefinitionConstants.ItemType;
using static IndustrialSystems.Definitions.DrillDefinition;
using static IndustrialSystems.Definitions.GasRefinerDefinition;
using static IndustrialSystems.Definitions.MaterialDefinition;
using static IndustrialSystems.Definitions.OutputCargoDefinition;
using static IndustrialSystems.Definitions.ResourceModifierDefinition;
using static IndustrialSystems.Definitions.SmelterDefinition;
using static IndustrialSystems.Definitions.ConveyorDefinition;
using static IndustrialSystems.Definitions.Constructors;
using BatchSettings = System.Collections.Generic.Dictionary<string, int>;
using OreToGas = System.Collections.Generic.Dictionary<string, VRage.MyTuple<string, int>[]>;
using GasExports = System.Collections.Generic.Dictionary<string, VRageMath.Vector3I[]>;
using MaterialProperties = System.Collections.Generic.Dictionary<string, float>;
using Fluid = VRage.MyTuple<string, int>;
using VRageMath;

namespace ISDefinitions
{
    public partial class Definitions
    {
        GasRefinerDefinition Template_GasRefiner => new GasRefinerDefinition
        {
            Base = new NameDef
            {
                SubtypeId = "is_oxygenrefiner",
                DefinitionPriority = 0,
            },
            MachineInventory = new MachineInventoryDef
            {
                MaxItemsInInventory = 10,
                PowerRequirementOverride = 1,
            },
            BatchJob = new BatchJobDef
            {
                BatchAmount = 1,
                BatchTimeTicks = 60,
            },
            RefineOresToGas = new OreToGas
            {
                ["Ice"] = new Fluid[]
                {
                    new Fluid { Item1 = "Hydrogen", Item2 = 2 },
                    new Fluid { Item1 = "Oxygen", Item2 = 1 },
                },
            },
            GasExports = new GasExports
            {
                ["Hydrogen"] = new[]
                {
                    Vector3I.Forward,
                },
                ["Oxygen"] = new[]
                {
                    Vector3I.Backward,
                }
            }
        };
    }
}
