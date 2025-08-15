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
        public int InventoryToDepositIn;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Drill,
                SubtypeId,
                DefinitionPriority,
                InventoryToDepositIn,
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
                InventoryToDepositIn = (int)data[3],

            };
        }
    }
}
