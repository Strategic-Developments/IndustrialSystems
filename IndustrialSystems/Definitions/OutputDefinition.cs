using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class OutputDefinition : Definition
    {
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Output,
                SubtypeId,
                DefinitionPriority,
            };
        }

        public static OutputDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Output)
                return null;

            return new OutputDefinition()
            {
                SubtypeId = (string)data[1],
                DefinitionPriority = (int)data[2],
            };
        }
    }
}
