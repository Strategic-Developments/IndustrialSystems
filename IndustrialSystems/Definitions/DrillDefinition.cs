using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialSystems.Definitions.DefinitionConstants;

namespace IndustrialSystems.Definitions
{
    public class DrillDefinition : PowerOverrideDefinition
    {
        /// <summary>
        /// Ores per second
        /// </summary>
        public uint DefaultDrillSpeed;
        public Dictionary<string, uint> MaterialDrillSpeed;

        /// <summary>
        /// set to zero to ignore
        /// otherwise, actual drill speed is MaterialDrillSpeed (or DefaultDrillSpeed if the material doesn't exist) * VoxelAmountMultiplier * (Voxel count)
        /// </summary>
        public float VoxelAmountMultiplier;

        public float InitialVoxelCheckSize;
        public float DownwardsVoxelCheckSize;
        public float DownwardVoxelCheckSizeInterval;
        public int DownwardVoxelCheckSizeAmount;
        public override object[] ConvertToObjectArray()
        {
            return new object[] {
                ISTypes.Drill,
                SubtypeId,
                DefinitionPriority,
                DefaultDrillSpeed,
                MaterialDrillSpeed,
                InitialVoxelCheckSize,
                DownwardsVoxelCheckSize,
                DownwardVoxelCheckSizeInterval,
                DownwardVoxelCheckSizeAmount,
                VoxelAmountMultiplier,
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
                DefaultDrillSpeed = (uint)data[3],
                MaterialDrillSpeed = (Dictionary<string, uint>)data[4],
                PowerRequirementOverride = (float)data[5],
                InitialVoxelCheckSize = (float)data[6],
                DownwardsVoxelCheckSize = (float)data[7],
                DownwardVoxelCheckSizeInterval = (float)data[8],
                DownwardVoxelCheckSizeAmount = (int)data[9],
                VoxelAmountMultiplier = (float)data[10],
            };
        }
    }
}
