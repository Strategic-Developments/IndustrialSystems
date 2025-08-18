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
using MaterialProperties = System.Collections.Generic.Dictionary<string, float>;
using Fluid = VRage.MyTuple<string, int>;
using VRageMath;

namespace ISDefinitions
{
    public partial class Definitions
    {
        DrillDefinition Template_Drill => new DrillDefinition
        {
            Base = new NameDef
            {
                SubtypeId = "is_drill",
                DefinitionPriority = 0,

            },
            MachineInventory = new MachineInventoryDef
            {
                MaxItemsInInventory = 10,
                PowerRequirementOverride = 1,
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
                },
                NumberOfBatchesToStore = 5,
            },
            DrillBatches = new DrillBatchDef
            {
                TimeBetweenBatches = 10,
                DefaultOresPerBatch = 10,
                VoxelAmountMultiplier = 1,
                OresPerBatchPerMaterial = new BatchSettings()
                {
                    ["Stone"] = 2,
                    ["Iron"] = 2,
                    ["Nickel"] = 2,
                    ["Cobalt"] = 1,
                },
            },
            DrillVoxelChecks = new VoxelCheckDef
            {
                InitialVoxelCheckSize = 25,

                DownwardsVoxelCheckSize = 250,
                DownwardVoxelCheckSizeAmount = 1,
                DownwardVoxelCheckSizeInterval = 60,
            },
        };
    }
}
