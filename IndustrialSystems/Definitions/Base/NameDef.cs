using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public struct NameDef : IPackagable
    {
        /// <summary>
        /// Subtype ID of the block or material, or mined ore.
        /// </summary>
        public string SubtypeId;
        /// <summary>
        /// If there are multiple definitions with the same subtype ID (like say someone is adjusting stats of another's mod), then the definition with the highest priority will be loaded.
        /// <para>For people making their own mod, its recommended to leave this at zero.</para>
        /// <para>For people MODIFYING other people's mod, its recommended to set this at anything greater than zero.</para>
        /// <para>This effectively allows mod adjuster-like behavior without relying on mod load order, although the entire definition must be copied for it to work properly. Those modifying shield stats can just have the shield definitions in their place w/o copying any models, sbc files, or sounds to the modified mod.</para>
        /// <para>
        /// Units: Unitless
        /// </para>
        /// <para>
        /// Requirements: <c>Value is an integer</c>
        /// </para>
        /// </summary>
        public int DefinitionPriority;
        public object[] ConvertToObjectArray()
        {
            return new object[]
            {
                SubtypeId,
                DefinitionPriority,
            };
        }

        public static NameDef ConvertFromObjectArray(object[] data)
        {
            return new NameDef
            {
                SubtypeId = (string)data[0],
                DefinitionPriority = (int)data[1],
            };
        }
    }
}
