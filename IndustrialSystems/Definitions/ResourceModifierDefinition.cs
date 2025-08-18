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
        public BatchJobDef BatchJob;

        public ItemModifierDef Modifier;
        public struct ItemModifierDef : IPackagable
        {
            /// <summary>
            /// Ore, or Ingot, will only accept the given type
            /// </summary>
            public ItemType TypeToModify;

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

            public object[] ConvertToObjectArray()
            {
                return new object[]
                {
                    TypeToModify,
                    UserOptionsFunc,
                    ModifierFunc,
                };
            }

            public static ItemModifierDef ConvertFromObjectArray(object[] data)
            {
                return new ItemModifierDef
                {
                    TypeToModify = (ItemType)data[0],
                    UserOptionsFunc = (Func<IReadOnlyDictionary<string, byte>, float[], List<TerminalControlText>, int>)data[1],
                    ModifierFunc = (Func<IReadOnlyDictionary<string, byte>, float[], int, List<string>, int>)data[2],
                };
            }
        }
        

        public GasReqDef GasRequirements;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.ResourceModifier,
                Base,
                MachineInventory,
                BatchJob,
                Modifier,
                GasRequirements,
            };
        }

        

        public static ResourceModifierDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.ResourceModifier)
                return null;

            return new ResourceModifierDefinition()
            {
                Base = NameDef.ConvertFromObjectArray((object[])data[1]),
                MachineInventory = MachineInventoryDef.ConvertFromObjectArray((object[])data[2]),
                BatchJob = BatchJobDef.ConvertFromObjectArray((object[])data[3]),
                Modifier = ItemModifierDef.ConvertFromObjectArray((object[])data[4]),
                GasRequirements = GasReqDef.ConvertFromObjectArray((object[])data[5]),
            };
        }
    }
}
