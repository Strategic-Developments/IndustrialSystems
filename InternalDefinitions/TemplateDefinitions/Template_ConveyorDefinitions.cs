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
        ConveyorDefinitions Template_ConveyorDefinitions => new ConveyorDefinitions
        {
            // As Conveyor defs are small (and are generally made in a set), they can all go here if you want
            new ConveyorDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "is_conveyor",
                    DefinitionPriority = 0,
                },
                Conveyors = new ConveyorDef
                {
                    Connections = new[]
                    {
                        VectorI(x: 0, y: 0, z: -1), // Forward
                        VectorI(x: 0, y: 0, z: 1), // Backward
                    },
                    ConvertToSplitter = false,
                    ItemTravelFrequency = 16,
                }
            },
            new ConveyorDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "is_conveyor_curved",
                    DefinitionPriority = 0,
                },
                Conveyors = new ConveyorDef
                {
                    Connections = new[]
                    {
                        Vector3I.Backward,
                        Vector3I.Right,
                    },
                    ConvertToSplitter = false,
                    ItemTravelFrequency = 16,
                }
            },
            new ConveyorDefinition
            {
                Base = new NameDef
                {
                    SubtypeId = "is_conveyor_merge",
                    DefinitionPriority = 0,
                },
                Conveyors = new ConveyorDef
                {
                    Connections = new[]
                    {
                        Vector3I.Forward,
                        Vector3I.Up,
                        Vector3I.Down,
                        Vector3I.Left,
                        Vector3I.Right,
                        Vector3I.Backward,
                    },
                    ConvertToSplitter = false,
                    ItemTravelFrequency = 0, // doesn't matter for mergers/splitters
                }
            },
        };
    }
}
