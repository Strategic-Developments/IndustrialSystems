using IndustrialSystems.Definitions;
using Sandbox.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluidDefinition = VRage.MyTuple<string, float>;

namespace IndustrialSystems.Utilities
{
    public class Config
    {
        public static Config I;

        public Dictionary<byte, MaterialDefinition> MaterialVoxelDefinitions;
        public Dictionary<string, MaterialDefinition> MaterialOreDefinitions;
        public Dictionary<string, Definition> BlockDefinitions;

        static Config()
        {
            I = new Config();
        }

        public Config()
        {
            MaterialVoxelDefinitions = new Dictionary<byte, MaterialDefinition>();

            //MyDefinitionManager.Static.GetVoxelMaterialDefinitions().Index;

            MaterialOreDefinitions = new Dictionary<string, MaterialDefinition>()
            {
                ["Stone"] = new MaterialDefinition
                {
                    SubtypeId = "Stone",
                    DisplayName = "Stone",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.5f,
                        ["Stone"] = 0.25f,
                        ["Iron"] = 0.15f,
                        ["Nickel"] = 0.06f,
                        ["Silicon"] = 0.04f,
                    }
                },
                ["Iron"] = new MaterialDefinition
                {
                    SubtypeId = "Iron",
                    DisplayName = "Iron Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.7f,
                        ["Iron"] = 0.27f,
                        ["Nickel"] = 0.03f,
                    }
                },
                ["Nickel"] = new MaterialDefinition
                {
                    SubtypeId = "Nickel",
                    DisplayName = "Nickel Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.85f,
                        ["Iron"] = 0.01f,
                        ["Nickel"] = 0.14f,
                    }
                },
                ["Silicon"] = new MaterialDefinition
                {
                    SubtypeId = "Silicon",
                    DisplayName = "Silicon Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.80f,
                        ["Stone"] = 0.0499f,
                        ["Iron"] = 0.01f,
                        ["Silicon"] = 0.15f,
                        ["Magnesium"] = 0.001f,
                    }
                },
                ["Cobalt"] = new MaterialDefinition
                {
                    SubtypeId = "Cobalt",
                    DisplayName = "Cobalt Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.90f,
                        ["Iron"] = 0.0025f,
                        ["Nickel"] = 0.0025f,
                        ["Cobalt"] = 0.095f,
                    }
                },
                ["Magnesium"] = new MaterialDefinition
                {
                    SubtypeId = "Magnesium",
                    DisplayName = "Magnesium Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.99f,
                        ["Silicon"] = 0.005f,
                        ["Magnesium"] = 0.005f,
                    }
                },
                ["Silver"] = new MaterialDefinition
                {
                    SubtypeId = "Silver",
                    DisplayName = "Silver Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.9945f,
                        ["Silver"] = 0.005f,
                        ["Gold"] = 0.0005f,
                    }
                },
                ["Gold"] = new MaterialDefinition
                {
                    SubtypeId = "Gold",
                    DisplayName = "Gold Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.9945f,
                        ["Silver"] = 0.0005f,
                        ["Gold"] = 0.005f,
                    }
                },
                ["Platinum"] = new MaterialDefinition
                {
                    SubtypeId = "Platinum",
                    DisplayName = "Platinum Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.99989f,
                        ["Gold"] = 0.00001f,
                        ["Platinum"] = 0.0001f,
                    }
                },
                ["Uranium"] = new MaterialDefinition
                {
                    SubtypeId = "Uranium",
                    DisplayName = "Uranium Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["None"] = 0.9999f,
                        ["Uranium"] = 0.0001f,
                    }
                },
                ["Ice"] = new MaterialDefinition
                {
                    SubtypeId = "Ice",
                    DisplayName = "Ice Ore",
                    DefinitionPriority = 0,
                    IsMinedOre = true,
                    MaterialProperties = new Dictionary<string, float>()
                    {
                        ["Ice"] = 1f,
                    }
                },
            };

            BlockDefinitions = new Dictionary<string, Definition>()
            {
                ["is_drill"] = new DrillDefinition
                {
                    SubtypeId = "is_drill",
                    DefinitionPriority = 0,
                    PowerRequirementOverride = 1,
                    DefaultDrillSpeed = 0,
                    MaterialDrillSpeed = new Dictionary<string, uint>()
                    {
                        ["Stone"] = 2,
                        ["Iron"] = 2,
                        ["Nickel"] = 2,
                        ["Cobalt"] = 1,
                    },
                    InitialVoxelCheckSize = 25,

                    DownwardsVoxelCheckSize = 250,
                    DownwardVoxelCheckSizeAmount = 1,
                    DownwardVoxelCheckSizeInterval = 60,
                    
                },
                ["is_smelter"] = new SmelterDefinition
                {
                    SubtypeId = "is_smelter",
                    DefinitionPriority = 0,
                    PowerRequirementOverride = 1,
                    DefaultOreMultiplier = 0,
                    MaxOresSmelted = 1,
                    SmelterOreMultipliers = new Dictionary<string, float>()
                    {
                        ["Stone"] = 1,
                        ["Iron"] = 1,
                        ["Nickel"] = 1,
                        ["Cobalt"] = 0.5f,
                    }
                },
                ["is_oxygenrefiner"] = new GasRefinerDefinition
                {
                    SubtypeId = "is_oxygenrefiner",
                    DefinitionPriority = 0,
                    PowerRequirementOverride = 1,
                    GasRefineSpeed = 1,
                    RefineOresToGas = new Dictionary<string, FluidDefinition[]>
                    {
                        ["Ice"] = new[]
                        {
                            new FluidDefinition { Item1 = "Hydrogen", Item2 = 1 },
                            new FluidDefinition { Item1 = "Oxygen", Item2 = 0.5f },
                        }
                    }
                },
                ["is_crusher"] = new ResourceModifierDefinition
                {
                    SubtypeId = "is_crusher",
                    DefinitionPriority = 0,
                    PowerRequirementOverride = 1,
                    MaxSpeed = 1,
                    TypeToModify = ItemType.Ore,
                    UserOptionsFunc = DefinitionConstants.ShowNone(),
                    ModifierFunc = DefinitionConstants.Crusher(efficiency: 0.95f, noneAdditive: -0.2f, noneMultiplicative: 0),
                },
                ["is_purifier"] = new ResourceModifierDefinition
                {
                    SubtypeId = "is_purifier",
                    DefinitionPriority = 0,
                    PowerRequirementOverride = 1,
                    MaxSpeed = 1,
                    TypeToModify = ItemType.Ore,
                    UserOptionsFunc = DefinitionConstants.ShowOresGiven(maxSelections: 1),
                    ModifierFunc = DefinitionConstants.Purifier(efficiency: 0.95f, nonSelectedAdditive: 0, nonSelectedMultiplicative: 0),
                }
            };
        }
    }
}
