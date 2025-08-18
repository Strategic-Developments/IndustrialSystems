using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public struct MachineInventoryDef : IPackagable
    {
        /// <summary>
        /// If not zero, overrides the block's power requirement to this value in MW.
        /// <para>
        /// Units: MW
        /// </para>
        /// <para>
        /// Requirements: <c>Value is greater than or equal to 0</c>
        /// </para>
        /// </summary>
        public float PowerRequirementOverride;

        /// <summary>
        /// Max number of items before machine stops accepting/producing more.
        /// </summary>
        public int MaxItemsInInventory;

        public object[] ConvertToObjectArray()
        {
            return new object[]
            {
                PowerRequirementOverride,
                MaxItemsInInventory,
            };
        }

        public static MachineInventoryDef ConvertFromObjectArray(object[] data)
        {
            return new MachineInventoryDef
            {
                PowerRequirementOverride = (float)data[0],
                MaxItemsInInventory = (int)data[1],
            };
        }
    }
}
