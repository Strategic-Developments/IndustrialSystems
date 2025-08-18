using IndustrialSystems.Definitions;
using static IndustrialSystems.Definitions.Definition;
using static IndustrialSystems.Definitions.NameDef;
using static IndustrialSystems.Definitions.GasReqDef;
using static IndustrialSystems.Definitions.BatchJobDef;
using static IndustrialSystems.Definitions.DefinitionConstants;
using static IndustrialSystems.Definitions.DefinitionConstants.ISTypes;
using static IndustrialSystems.Definitions.DefinitionConstants.ItemType;
using static IndustrialSystems.Definitions.BlockMachineDefinition;
using static IndustrialSystems.Definitions.BlockMachineDefinition.MachineInventoryDef;
using static IndustrialSystems.Definitions.DrillDefinition;
using static IndustrialSystems.Definitions.DrillDefinition.DrillBatchDef;
using static IndustrialSystems.Definitions.DrillDefinition.VoxelCheckDef;
using static IndustrialSystems.Definitions.GasRefinerDefinition;
using static IndustrialSystems.Definitions.MaterialDefinition;
using static IndustrialSystems.Definitions.MaterialDefinition.MaterialDef;
using static IndustrialSystems.Definitions.OutputCargoDefinition;
using static IndustrialSystems.Definitions.ResourceModifierDefinition;
using static IndustrialSystems.Definitions.ResourceModifierDefinition.ItemModifierDef;
using static IndustrialSystems.Definitions.SmelterDefinition;
using static IndustrialSystems.Definitions.SmelterDefinition.SmelterOreMultDef;
using BatchSettings = System.Collections.Generic.Dictionary<string, int>;
using OreToGas = System.Collections.Generic.Dictionary<string, VRage.MyTuple<string, int>[]>;
using MaterialProperties = System.Collections.Generic.Dictionary<string, float>;
using Fluid = VRage.MyTuple<string, int>;

namespace ISDefinitions
{
    public partial class Definitions
    {
        SmelterDefinition Template_Smelter => new SmelterDefinition
        {
            Base = new NameDef
            {
                SubtypeId = "is_smelter",
                DefinitionPriority = 0,
            },
            MachineInventory = new MachineInventoryDef
            {
                PowerRequirementOverride = 1,
                MaxItemsInInventory = 150,
            },
            GasRequirements = new GasReqDef
            {
                RequiredFluidsPerBatch = new Fluid[]
                {
                    new Fluid{ Item1 = "Gas Subtype Here", Item2 = 0 },
                },
                SpeedMultiplierWithOptionalFluids = 2,
                OptionalFluidsPerBatch = new Fluid[]
                {
                    new Fluid{ Item1 = "Gas Subtype Here", Item2 = 0 },
                }
            },
            BatchJob = new BatchJobDef
            {
                BatchAmount = 100,
                BatchTimeTicks = 60,
            },
            SmelterStats = new SmelterOreMultDef
            {
                DefaultOreMultiplier = 0,
                SmelterOreMultipliers = new MaterialProperties()
                {
                    ["Stone"] = 1,
                    ["Iron"] = 1,
                    ["Nickel"] = 1,
                    ["Cobalt"] = 0.5f,
                }
            }
        };
    }
}
