using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class SmelterDefinition : Definition
    {
        public MachineInventoryDef MachineInventory;
        public BatchJobDef BatchJob;
        public SmelterOreMultDef SmelterStats;
        public GasReqDef GasRequirements;
        public struct SmelterOreMultDef : IPackagable
        {
            /// <summary>
            /// Default ore to ingot resource multiplier
            /// </summary>
            public float DefaultOreMultiplier;
            /// <summary>
            /// Key: Ore name (keen Ore subtype like Iron, Nickel, etc)
            /// Value: Alternate multiplier for the given ore
            /// </summary>
            public Dictionary<string, float> SmelterOreMultipliers;
            /// <summary>
            /// Minimum ore percentage needed to smelt.
            /// </summary>
            public float MinimumOreRequired;

            public object[] ConvertToObjectArray()
            {
                return new object[]
                {
                    DefaultOreMultiplier,
                    SmelterOreMultipliers,
                    MinimumOreRequired,
                };
            }

            public static SmelterOreMultDef ConvertFromObjectArray(object[] data)
            {
                return new SmelterOreMultDef
                {
                    DefaultOreMultiplier = (float)data[0],
                    SmelterOreMultipliers = (Dictionary<string, float>)data[1],
                    MinimumOreRequired = (float)data[2],
                };
            }
        }
        
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Smelter,
                Base.ConvertToObjectArray(),
                MachineInventory.ConvertToObjectArray(),
                BatchJob.ConvertToObjectArray(),
                SmelterStats.ConvertToObjectArray(),
                GasRequirements.ConvertToObjectArray(),
            };
        }

        public static SmelterDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Smelter)
                return null;

            return new SmelterDefinition()
            {
                Base = NameDef.ConvertFromObjectArray((object[])data[1]),
                MachineInventory = MachineInventoryDef.ConvertFromObjectArray((object[])data[2]),
                BatchJob = BatchJobDef.ConvertFromObjectArray((object[])data[3]),
                SmelterStats = SmelterOreMultDef.ConvertFromObjectArray((object[])data[4]),
                GasRequirements = GasReqDef.ConvertFromObjectArray((object[])data[5]),
            };
        }
    }
}
