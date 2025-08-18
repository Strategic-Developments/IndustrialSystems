using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using static IndustrialSystems.Definitions.DefinitionConstants;
using Fluid = VRage.MyTuple<string, int>;

namespace IndustrialSystems.Definitions
{
    public class GasRefinerDefinition : Definition
    {
        public NameDef Base;
        public MachineInventoryDef MachineInventory;
        public BatchJobDef BatchJob;
        /// <summary>
        /// Key: ore name
        /// Value: Gas resource name & amount
        /// </summary>
        public Dictionary<string, Fluid[]> RefineOresToGas;
        public Dictionary<string, Vector3I[]> GasExports;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.GasRefiner,
                Base.ConvertToObjectArray(),
                MachineInventory.ConvertToObjectArray(),
                BatchJob.ConvertToObjectArray(),
                RefineOresToGas,
                GasExports,
            };
        }

        public static GasRefinerDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.GasRefiner)
                return null;

            return new GasRefinerDefinition()
            {
                Base = NameDef.ConvertFromObjectArray((object[])data[1]),
                MachineInventory = MachineInventoryDef.ConvertFromObjectArray((object[])data[2]),
                BatchJob = BatchJobDef.ConvertFromObjectArray((object[])data[3]),
                RefineOresToGas = (Dictionary<string, Fluid[]>)data[4],
                GasExports = (Dictionary<string, Vector3I[]>)data[5],
            };
        }
    }
}
