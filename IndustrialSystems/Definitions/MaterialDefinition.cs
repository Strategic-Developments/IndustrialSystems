using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class MaterialDefinition : Definition
    {
        /// <summary>
        /// Name players will see for materials shown in terminals
        /// </summary>
        public string DisplayName;
        /// <summary>
        /// If true, has this definition point to the voxel's MinedOre instead of its subtype.
        /// </summary>
        public bool IsMinedOre;

        /// <summary>
        /// Key: Ore name (keen Ore subtype like Iron, Nickel, etc)
        /// Value: Percentage per ore
        /// "None" for empty space
        /// Values must sum to 1, if they aren't then they will be scaled to have the sum = 1
        /// </summary>
        public Dictionary<string, float> MaterialProperties;

        public void AppendMaterialProperties(StringBuilder sb)
        {
            foreach (var kvp in MaterialProperties)
            {
                sb.Append($"{kvp.Key}: {kvp.Value*100:##.####}%\n");
            }
        }

        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Material,
                SubtypeId,
                DefinitionPriority,
                IsMinedOre,
                MaterialProperties,
            };
        }

        public static MaterialDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Material)
                return null;

            return new MaterialDefinition()
            {
                SubtypeId = (string)data[1],
                DefinitionPriority = (int)data[2],
                IsMinedOre = (bool)data[3],
                MaterialProperties = (Dictionary<string, float>)data[4],
            };
        }
    }
}
