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
        ResourceModifierDefinition Template_Purifier => new ResourceModifierDefinition
        {
            Base = new NameDef
            {
                SubtypeId = "is_purifier",
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
                },
                NumberOfBatchesToStore = 5,
            },
            BatchJob = new BatchJobDef
            {
                BatchAmount = 100,
                BatchTimeTicks = 60,
            },
            Modifier = new ItemModifierDef
            {
                TypeToModify = Ore,
                UserOptionsFunc = ShowOresGiven(maxSelections: 1),
                ModifierFunc = Purifier(efficiency: 0.95f, nonSelectedAdditive: 0, nonSelectedMultiplicative: 0.25f),
            },
        };
    }
}
