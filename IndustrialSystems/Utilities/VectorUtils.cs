using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Utilities
{
    internal static class VectorUtils
    {
        public static Vector3I TransformVector(this IMyCubeBlock block, Vector3I toTransform)
        {
            var left = block.Orientation.TransformDirection(Base6Directions.Direction.Left);
            var up = block.Orientation.TransformDirection(Base6Directions.Direction.Up);
            var back = block.Orientation.TransformDirection(Base6Directions.Direction.Backward);

            return Base6Directions.GetIntVector(left) * toTransform.X
                + Base6Directions.GetIntVector(up) * toTransform.Y
                + Base6Directions.GetIntVector(back) * toTransform.Z + (block.Max + block.Min) / 2;
        }

        public static Vector3I InverseTransformVector(this IMyCubeBlock block, Vector3I toTransform)
        {
            var left = block.Orientation.TransformDirectionInverse(Base6Directions.Direction.Left);
            var up = block.Orientation.TransformDirectionInverse(Base6Directions.Direction.Up);
            var back = block.Orientation.TransformDirectionInverse(Base6Directions.Direction.Backward);

            return Base6Directions.GetIntVector(left) * toTransform.X
                + Base6Directions.GetIntVector(up) * toTransform.Y
                + Base6Directions.GetIntVector(back) * toTransform.Z - (block.Max + block.Min) / 2;
        }
    }
}
