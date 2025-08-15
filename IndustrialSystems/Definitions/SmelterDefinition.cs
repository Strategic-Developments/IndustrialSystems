using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class SmelterDefinition : PowerOverrideDefinition
    {
        /// <summary>
        /// Maximum ores to smelt per second.
        /// </summary>
        public uint MaxOresSmelted;
        /// <summary>
        /// Default ore to ingot multiplier (Floor(MaxOresSmelted * OreMultiplier) = resulting ingot count)
        /// </summary>
        public float DefaultOreMultiplier;
        /// <summary>
        /// Key: Ore name (keen Ore subtype like Iron, Nickel, etc)
        /// Value: Alternate multiplier for the given ore
        /// </summary>
        public Dictionary<string, float> SmelterOreMultipliers;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Smelter,
                SubtypeId,
                DefinitionPriority,
                DefaultOreMultiplier,
                SmelterOreMultipliers,
                PowerRequirementOverride,
                MaxOresSmelted,
            };
        }

        public static SmelterDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Smelter)
                return null;

            return new SmelterDefinition()
            {
                SubtypeId = (string)data[1],
                DefinitionPriority = (int)data[2],
                DefaultOreMultiplier = (float)data[3],
                SmelterOreMultipliers = (Dictionary<string, float>)data[4],
                PowerRequirementOverride = (float)data[5],
                MaxOresSmelted = (uint)data[6],
            };
        }
    }
}
