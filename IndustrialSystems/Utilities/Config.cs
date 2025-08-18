using IndustrialSystems.Definitions;
using Sandbox.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluidDefinition = VRage.MyTuple<string, float>;
using static IndustrialSystems.Definitions.DefinitionConstants;

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
            MaterialOreDefinitions = new Dictionary<string, MaterialDefinition>();
            BlockDefinitions = new Dictionary<string, Definition>();
            InitializeResourceVector();
        }

        public void InitializeResourceVector()
        {
            foreach (var voxeldef in MaterialVoxelDefinitions.Values.Union(MaterialOreDefinitions.Values))
            {
                foreach (var ore in voxeldef.MaterialProperties.Keys)
                {
                    ResourceVector.Initialize(ore);
                }
            }
        }
    }
}
