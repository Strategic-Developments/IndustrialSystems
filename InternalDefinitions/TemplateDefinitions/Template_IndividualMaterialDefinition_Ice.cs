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
        // This option is available to those of you who want one file per Material Definition.
        // Template_MaterialDefinitions.cs (where multiple can be placed in one file) is usually cleaner
        MaterialDefinition Template_Ice => new MaterialDefinition
        {
            Base = new NameDef
            {
                SubtypeId = "Ice",
                DefinitionPriority = 0,
            },
            Material = new MaterialDef
            {
                DisplayName = "Ice",
                IsMinedOre = true,
                MaterialProperties = new MaterialProperties()
                {
                    ["Ice"] = 1f,
                }
            }
        };
    }
}
