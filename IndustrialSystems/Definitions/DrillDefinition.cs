using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class DrillDefinition : BlockMachineDefinition
    {
        public DrillBatchDef DrillBatches;
        public struct DrillBatchDef : IPackagable
        {
            /// <summary>
            /// in ticks
            /// </summary>
            public int TimeBetweenBatches;

            /// <summary>
            /// Ores per batch
            /// </summary>
            public int DefaultOresPerBatch;
            public Dictionary<string, int> OresPerBatchPerMaterial;

            /// <summary>
            /// set to zero to ignore
            /// otherwise, actual drill speed is the floor of OresPerBatchPerMaterial (or DefaultOresPerBatch if the material doesn't exist) * VoxelAmountMultiplier * (Voxel count)
            /// </summary>
            public float VoxelAmountMultiplier;

            public object[] ConvertToObjectArray()
            {
                return new object[]
                    {
                        TimeBetweenBatches,
                        DefaultOresPerBatch,
                        OresPerBatchPerMaterial,
                        VoxelAmountMultiplier,
                    };
            }

            public static DrillBatchDef ConvertFromObjectArray(object[] data)
            {
                return new DrillBatchDef
                {
                    TimeBetweenBatches = (int)data[0],
                    DefaultOresPerBatch = (int)data[1],
                    OresPerBatchPerMaterial = (Dictionary<string, int>)data[2],
                    VoxelAmountMultiplier = (float)data[3],
                };
            }
        }

        public VoxelCheckDef DrillVoxelChecks;
        public struct VoxelCheckDef : IPackagable
        {
            /// <summary>
            /// Cube side length, centered on block origin to check for voxels. If the check passes then it will do downwards cube checks based on other variables. All voxels seen are compiled into a list for users to choose from.
            /// </summary>
            public float InitialVoxelCheckSize;
            /// <summary>
            ///  Cube side length, centered on downard check origin
            /// </summary>
            public float DownwardsVoxelCheckSize;
            /// <summary>
            /// Distance between cube voxel checks
            /// </summary>
            public float DownwardVoxelCheckSizeInterval;
            /// <summary>
            /// Number of downward cube voxel checks
            /// </summary>
            public int DownwardVoxelCheckSizeAmount;

            public object[] ConvertToObjectArray()
            {
                return new object[]
                {
                    InitialVoxelCheckSize,
                    DownwardsVoxelCheckSize,
                    DownwardVoxelCheckSizeInterval,
                    DownwardVoxelCheckSizeAmount,
                };
            }

            public static VoxelCheckDef ConvertFromObjectArray(object[] data)
            {
                return new VoxelCheckDef
                {
                    InitialVoxelCheckSize = (float)data[0],
                    DownwardsVoxelCheckSize = (float)data[1],
                    DownwardVoxelCheckSizeInterval = (float)data[2],
                    DownwardVoxelCheckSizeAmount = (int)data[3],
                };
            }
        }
        public GasReqDef GasRequirements;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Drill,
                Base.ConvertToObjectArray(),
                DrillBatches.ConvertToObjectArray(),
                MachineInventory.ConvertToObjectArray(),
                DrillVoxelChecks.ConvertToObjectArray(),
                GasRequirements.ConvertToObjectArray(),
            };
        }

        public static DrillDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Drill)
                return null;

            return new DrillDefinition()
            {
                Base = NameDef.ConvertFromObjectArray((object[])data[1]),
                DrillBatches = DrillBatchDef.ConvertFromObjectArray((object[])data[2]),
                MachineInventory = MachineInventoryDef.ConvertFromObjectArray((object[])data[3]),
                DrillVoxelChecks = VoxelCheckDef.ConvertFromObjectArray((object[])data[4]),
                GasRequirements = GasReqDef.ConvertFromObjectArray((object[])data[5]),
            };
        }
    }
}
