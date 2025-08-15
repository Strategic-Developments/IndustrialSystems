using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.Utils;

namespace IndustrialSystems.Definitions
{
    public enum ItemType : byte
    {
        None = 0,
        Ore = 1,
        Ingot = 3,
    }
    public abstract class Definition
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
        /// <summary>
        /// Need to use object[] because I don't want to make an API just to transfer delegates
        /// </summary>
        public abstract object[] ConvertToObjectArray();
    }

    public abstract class PowerOverrideDefinition : Definition
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
    }
    public static class DefinitionConstants
    {
        public const long MessageHandlerId = 10481;

        /// <summary>
        /// Internal use enum, don't worry about it
        /// </summary>
        public enum ISTypes
        {
            Drill,
            ResourceModifier,
            Smelter,
            GasRefiner,
            Output,
            Material,
        }
        /// <summary>
        /// Normalizes the array's sum to 1
        /// </summary>
        /// <param name="arr"></param>
        /// <returns>1/sum</returns>
        public static float SumNormalize(this float[] arr)
        {
            float sum = 0;
            foreach (var f in arr)
            {
                sum += f;
            }

            float invSum = 1 / sum;

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] *= invSum;
            }
            return invSum;
        }
        /// <summary>
        /// Treats the float[] as a vector, returns its Euclidian length squared.
        /// </summary>
        /// <param name="vec">Vector</param>
        /// <returns>Vector's length squared</returns>
        public static float VecLengthSq(this float[] vec)
        {
            float lenSq = 0;

            foreach (var f in vec)
            {
                lenSq += f * f;
            }

            return lenSq;
        }
        /// <summary>
        /// Treats the float[] as a vector, returns its Euclidian length.
        /// </summary>
        /// <param name="vec">Vector</param>
        /// <returns>Vector's length</returns>
        public static float VecLength(this float[] vec)
        {
            return (float)Math.Sqrt(vec.VecLengthSq());
        }
        /// <summary>
        /// Treats the float[] as a vector, and normalizes its Euclidian length to 1.
        /// </summary>
        /// <param name="vec">Vector</param>
        /// <returns>1/Length</returns>
        public static float VecNormalize(this float[] vec)
        {
            float len = 1 / vec.VecLength();

            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] *= len;
            }
            return len;
        }
        /// <summary>
        /// Multiplies all array elements by mult
        /// </summary>
        /// <param name="arr">Array to multiply</param>
        /// <param name="mult">Value to multiply by</param>
        public static void IndividualMultiply(this float[] arr, float mult)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] *= mult;
            }
        }
        /// <summary>
        /// Multiplies all array elements by mult
        /// </summary>
        /// <param name="arr">Array to multiply</param>
        /// <param name="mult">Value to multiply by</param>
        public static void IndividualMultiply(this float[] arr, int mult)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] *= mult;
            }
        }
        /// <summary>
        /// Doesn't error if vectors are of different sizes, just multiplies the smallest vector with the first n elements of the larger one, where n is the smaller vector's length.
        /// </summary>
        /// <param name="vec1">Vector 1</param>
        /// <param name="vec2">Vector 2</param>
        /// <returns>Dot product of the two vectors.</returns>
        public static float VecDot(this float[] vec1, float[] vec2)
        {
            int max = Math.Min(vec1.Length, vec2.Length);

            float ret = 0;
            for (int i = 0; i < max; i++)
            {
                ret += vec1[i] * vec2[i];
            }

            return ret;
        }
        /// <summary>
        /// Show given ores in the list menu dropdown
        /// </summary>
        /// <param name="maxSelections">Maximum amount of selections users can make</param>
        /// <returns></returns>
        public static Func<Dictionary<string, byte>, float[], List<MyTerminalControlListBoxItem>, int> ShowOresGiven(int maxSelections)
        {
            return (Dictionary<string, byte> keys, float[] parts, List<MyTerminalControlListBoxItem> outUserSelections) =>
            {
                foreach (var str in keys)
                {
                    outUserSelections.Add(
                        new MyTerminalControlListBoxItem(
                            text: MyStringId.GetOrCompute(str.Key), 
                            tooltip: MyStringId.GetOrCompute($"{parts[str.Value]*100:##.####}%"), 
                            userData: str.Key));
                }

                return maxSelections;
            };
        }
        /// <summary>
        /// Show nothing in the list menu dropdown
        /// </summary>
        /// <returns></returns>
        public static Func<Dictionary<string, byte>, float[], List<MyTerminalControlListBoxItem>, int> ShowNone()
        {
            return (Dictionary<string, byte> keys, float[] parts, List<MyTerminalControlListBoxItem> outUserSelections) =>
            {
                return 0;
            };
        }
        /// <summary>
        /// Have the ResourceModifier reduce "None" components of ores/ingots, with a general reduction in item ocunt based on eficiency. 1 = 100% efficient - Usable ore in = Usable ore out (ish, rounds)
        /// </summary>
        /// <param name="efficiency"></param>
        /// <param name="noneAdditive"></param>
        /// <param name="noneMultiplicative"></param>
        /// <returns></returns>
        public static Func<Dictionary<string, byte>, float[], uint, List<MyTerminalControlListBoxItem>, uint> Crusher(float efficiency, float noneAdditive, float noneMultiplicative)
        {
            return (Dictionary<string, byte> keys, float[] parts, uint initialAmount, List<MyTerminalControlListBoxItem> userSelections) =>
            {
                byte index = keys["None"];
                parts[index] = Math.Max(0, parts[index] * noneMultiplicative + noneAdditive);
                float reduction = parts.SumNormalize() * efficiency;
                return (uint)(Math.Floor(initialAmount * reduction));
            };
        }
        /// <summary>
        /// Have the ResourceModifier reduce non selected components of ores/ingots, with a general reduction in item ocunt based on eficiency. 1 = 100% efficient - Usable ore in = Usable ore out (ish, rounds)
        /// </summary>
        /// <param name="efficiency"></param>
        /// <param name="nonSelectedAdditive"></param>
        /// <param name="nonSelectedMultiplicative"></param>
        /// <returns></returns>
        public static Func<Dictionary<string, byte>, float[], uint, List<MyTerminalControlListBoxItem>, uint> Purifier(float efficiency, float nonSelectedAdditive, float nonSelectedMultiplicative)
        {
            return (Dictionary<string, byte> keys, float[] parts, uint initialAmount, List<MyTerminalControlListBoxItem> userSelections) =>
            {
                foreach (var kvp in keys)
                {
                    foreach (var item in userSelections)
                    {
                        if (((string)item.UserData) == kvp.Key)
                        {
                            byte index = keys["None"];
                            parts[index] = Math.Max(0, parts[index] * nonSelectedMultiplicative + nonSelectedAdditive);
                            break;
                        }
                    }
                }
                float reduction = parts.SumNormalize() * efficiency;

                return (uint)Math.Floor(initialAmount * reduction);
            };
        }
    }
}
