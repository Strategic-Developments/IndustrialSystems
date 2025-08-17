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
        public struct VoxelCheckDef : IPackagable
        {
            /// <summary>
            /// set to zero to ignore
            /// otherwise, actual drill speed is the floor of OresPerBatchPerMaterial (or DefaultOresPerBatch if the material doesn't exist) * VoxelAmountMultiplier * (Voxel count)
            /// </summary>
            public float VoxelAmountMultiplier;

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
                    VoxelAmountMultiplier,
                    InitialVoxelCheckSize,
                    DownwardsVoxelCheckSize,
                    DownwardVoxelCheckSizeInterval,
                    DownwardVoxelCheckSizeAmount,
                };
            }
        }

        /// <summary>
        /// in ticks
        /// </summary>
        public int TimeBetweenBatches;

        /// <summary>
        /// Ores per batch
        /// </summary>
        public int DefaultOresPerBatch;
        public Dictionary<string, int> OresPerBatchPerMaterial;

        public VoxelCheckDef DrillVoxelChecks;
        public GasReqDef GasRequirements;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Drill,
                SubtypeId,
                DefinitionPriority,
                DefaultOresPerBatch,
                OresPerBatchPerMaterial,
                DrillVoxelChecks.ConvertToObjectArray(),
                TimeBetweenBatches,
                MaxItemsInInventory,
                GasRequirements.ConvertToObjectArray(),
            };
        }

        public static DrillDefinition ConvertFromObjectArray(object[] data)
        {
            if ((ISTypes)data[0] != ISTypes.Drill)
                return null;

            return new DrillDefinition()
            {
                SubtypeId = (string)data[1],
                DefinitionPriority = (int)data[2],
                DefaultOresPerBatch = (int)data[3],
                OresPerBatchPerMaterial = (Dictionary<string, int>)data[4],
                PowerRequirementOverride = (float)data[5],
                TimeBetweenBatches = (int)data[6],
                MaxItemsInInventory = (int)data[7],
                GasRequirements = GasReqDef.ConvertFromObjectArray((object[])data[8]),
            };
        }
    }
}
