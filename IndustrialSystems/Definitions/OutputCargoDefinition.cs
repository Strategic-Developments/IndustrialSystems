using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class OutputCargoDefinition : Definition
    {
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Output,
                Base.ConvertToObjectArray(),
            };
        }

        public static OutputCargoDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Output)
                return null;

            return new OutputCargoDefinition()
            {
                Base = NameDef.ConvertFromObjectArray((object[])data[1]),
            };
        }
    }
}
