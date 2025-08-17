using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    using Fluid = VRage.MyTuple<string, int>;
    public struct GasReqDef : IPackagable
    {
        public Fluid[] RequiredFluidsPerBatch;

        public float SpeedIncreaseWithOptionalFluids;
        public Fluid[] OptionalFluidsPerBatch;

        public object[] ConvertToObjectArray()
        {
            return new object[]
            {
                RequiredFluidsPerBatch,
                SpeedIncreaseWithOptionalFluids,
                OptionalFluidsPerBatch,
            };
        }

        public static GasReqDef ConvertFromObjectArray(object[] data)
        {
            return new GasReqDef
            {
                RequiredFluidsPerBatch = (Fluid[])data[1],
                SpeedIncreaseWithOptionalFluids = (float)data[2],
                OptionalFluidsPerBatch = (Fluid[])data[3],
            };
        }
    }
}
