using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class SmelterDefinition : Definition
    {
        public uint MaxOresSmelted;
        public float DefaultOreMultiplier;
        public Dictionary<string, float> SmelterOreMultipliers;
        public float PowerOverride;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Smelter,
                SubtypeId,
                DefinitionPriority,
                DefaultOreMultiplier,
                SmelterOreMultipliers,
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
                PowerOverride = (float)data[5],
                MaxOresSmelted = (uint)data[6],
            };
        }
    }
}
