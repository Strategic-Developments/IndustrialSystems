using IndustrialSystems.Definitions;
using Sandbox.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialSystems.Utilities
{
    public class Config
    {
        public static Config I;

        public Dictionary<byte, MaterialDefinition> MaterialVoxelDefinitions;
        public Dictionary<string, MaterialDefinition> MaterialOreDefinitions;
        public Dictionary<string, Definition> BlockDefinitions;
        public Dictionary<string, ConveyorDefinition> ConveyorLineDefinitions;
        public static void Load()
        {
            I = new Config
            {
                MaterialVoxelDefinitions = new Dictionary<byte, MaterialDefinition>(),
                MaterialOreDefinitions = new Dictionary<string, MaterialDefinition>(),
                BlockDefinitions = new Dictionary<string, Definition>(),
                ConveyorLineDefinitions = new Dictionary<string, ConveyorDefinition>()
            };
        }

        public void InitializeResourceVector()
        {
            foreach (var voxeldef in MaterialVoxelDefinitions.Values.Union(MaterialOreDefinitions.Values))
            {
                foreach (var ore in voxeldef.Material.MaterialProperties.Keys)
                {
                    ResourceVector.Initialize(ore);
                }
            }
        }
    }
}
