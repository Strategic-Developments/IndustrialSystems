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
        MaterialDefinitions Template_MaterialDefinitions => new MaterialDefinitions
        {
            // As Material defs are small, they can all go here if you want
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Stone",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Stone",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.5f,
                        ["Stone"] = 0.25f,
                        ["Iron"] = 0.15f,
                        ["Nickel"] = 0.06f,
                        ["Silicon"] = 0.04f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Iron",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Iron Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.7f,
                        ["Iron"] = 0.27f,
                        ["Nickel"] = 0.03f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Nickel",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Nickel Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.85f,
                        ["Iron"] = 0.01f,
                        ["Nickel"] = 0.14f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Silicon",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Siligone Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.80f,
                        ["Stone"] = 0.0499f,
                        ["Iron"] = 0.01f,
                        ["Silicon"] = 0.15f,
                        ["Magnesium"] = 0.001f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Cobalt",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Cobalt Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.90f,
                        ["Iron"] = 0.0025f,
                        ["Nickel"] = 0.0025f,
                        ["Cobalt"] = 0.095f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Magnesium",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Magnesium Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.99f,
                        ["Silicon"] = 0.005f,
                        ["Magnesium"] = 0.005f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Silver",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Silver Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.9945f,
                        ["Silver"] = 0.005f,
                        ["Gold"] = 0.0005f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Gold",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Gold Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.9945f,
                        ["Silver"] = 0.0005f,
                        ["Gold"] = 0.005f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Platinum",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Platinum Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.99989f,
                        ["Gold"] = 0.00001f,
                        ["Platinum"] = 0.0001f,
                    }
                }
            },
            new MaterialDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "Uranium",
                    DefinitionPriority = 0,
                },
                Material = new MaterialDef
                {
                    DisplayName = "Uranium Ore",
                    IsMinedOre = true,
                    MaterialProperties = new MaterialProperties()
                    {
                        ["None"] = 0.9999f,
                        ["Uranium"] = 0.0001f,
                    }
                }
            },
        };
    }
}
