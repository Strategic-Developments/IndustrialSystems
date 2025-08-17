using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public struct BatchJobDef : IPackagable
    {
        /// <summary>
        /// Minimum input number of items required to process, and will consume that amount.
        /// </summary>
        public int BatchAmount;

        /// <summary>
        /// Time it takes in ticks for one batch to be processed and output
        /// </summary>
        public int BatchSpeedTicks;
        public object[] ConvertToObjectArray()
        {
            return new object[]
            {
                BatchAmount,
                BatchSpeedTicks,
            };
        }

        public static BatchJobDef ConvertFromObjectArray(object[] data)
        {
            return new BatchJobDef
            {
                BatchAmount = (int)data[0],
                BatchSpeedTicks = (int)data[1],
            };
        }
    }
}
