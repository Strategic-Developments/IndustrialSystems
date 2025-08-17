using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;
using FluidDefinition = VRage.MyTuple<string, float>;

namespace IndustrialSystems.Definitions
{
    public class GasRefinerDefinition : BlockMachineDefinition
    {
        /// <summary>
        /// Minimum input number of ores required to process, and will consume that amount.
        /// </summary>
        public int BatchAmount;

        /// <summary>
        /// Time it takes in ticks for one batch to be processed and output
        /// </summary>
        public int BatchSpeedTicks;
        /// <summary>
        /// Key: ore name
        /// Value: Gas resource name & amount
        /// </summary>
        public Dictionary<string, FluidDefinition[]> RefineOresToGas;
        
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.GasRefiner,
                SubtypeId,
                DefinitionPriority,
                RefineOresToGas,
                PowerRequirementOverride,
                BatchAmount,
                BatchSpeedTicks,
                MaxItemsInInventory,
            };
        }

        public static GasRefinerDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.GasRefiner)
                return null;

            return new GasRefinerDefinition()
            {
                SubtypeId = (string)data[1],
                DefinitionPriority = (int)data[2],
                RefineOresToGas = (Dictionary<string, FluidDefinition[]>)data[3],
                PowerRequirementOverride = (float)data[4],
                BatchAmount = (int)data[5],
                BatchSpeedTicks = (int)data[6],
                MaxItemsInInventory = (int)data[7],
            };
        }
    }
}
