using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.ModAPI;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    using TerminalControlText = MyTuple<string, string, string>;
    public class ResourceModifierDefinition : BlockMachineDefinition
    {
        /// <summary>
        /// Ore, or Ingot, will only accept the given type
        /// </summary>
        public ItemType TypeToModify;
        /// <summary>
        /// Minimum input number of ores required to process, and will consume that amount.
        /// </summary>
        public int BatchAmount;

        /// <summary>
        /// Time it takes in ticks for one batch to be processed and output
        /// </summary>
        public int BatchSpeedTicks;

        /// <summary>
        /// <para>Dictionary&lt;string, byte&gt; - map from ore names to indexes in the float[].</para>
        /// <para>float[] - resource vector; basically the item's composition. Recommended to not modify.</para>
        /// <para>List&lt;string&gt; - list of options to present the user - FILL THIS OUT</para>
        /// <para>return value - maximum number of user selections</para>
        /// </summary>
        public Func<IReadOnlyDictionary<string, byte>, float[], List<TerminalControlText>, int> UserOptionsFunc;

        /// <summary>
        /// <para>Dictionary&lt;string, byte&gt; - map from ore names to indexes in the float[].</para>
        /// <para>float[] - resource vector; basically the item's composition. Modify this to change item comp</para>
        /// <para>uint - number of items</para>
        /// <para>List&lt;string&gt; - list of actively user selected options</para>
        /// <para>return value - number of items post modification</para>
        /// </summary>
        public Func<IReadOnlyDictionary<string, byte>, float[], int, List<string>, int> ModifierFunc;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.ResourceModifier,
                SubtypeId,
                DefinitionPriority,
                UserOptionsFunc,
                ModifierFunc,
                BatchAmount,
                BatchSpeedTicks,
                MaxItemsInInventory,
            };
        }

        

        public static ResourceModifierDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.ResourceModifier)
                return null;

            return new ResourceModifierDefinition()
            {
                SubtypeId = (string)data[1],
                DefinitionPriority = (int)data[2],
                PowerRequirementOverride = (float)data[3],
                UserOptionsFunc = (Func<IReadOnlyDictionary<string, byte>, float[], List<TerminalControlText>, int>)data[4],
                ModifierFunc = (Func<IReadOnlyDictionary<string, byte>, float[], int, List<string>, int>)data[5],
                BatchAmount = (int)data[6],
                BatchSpeedTicks = (int)data[7],
                MaxItemsInInventory = (int)data[8],
            };
        }
    }
}
