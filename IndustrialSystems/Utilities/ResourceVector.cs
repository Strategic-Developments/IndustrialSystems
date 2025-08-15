using IndustrialSystems.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace IndustrialSystems.Utilities
{
    /// <summary>
    /// As this is designed to work with modded ores, we can't use a fixed vector size
    /// Map maps Ore names to indexes in the vector
    /// Dictionary for every vector is wasteful as once the session is loaded we know how many ores there will be
    /// If MyStringId is safe to use then all strings should probably be replaced with it
    /// </summary>
    public class ResourceVector : IEnumerable
    {
        public static Dictionary<string, byte> Map;
        public readonly float[] Vector;

        public ResourceVector()
        {
            Vector = new float[Map.Count];
        }
        private ResourceVector(float[] vector)
        {
            this.Vector = new float[Map.Count];
            vector.CopyTo(this.Vector, 0);
        }
        public ResourceVector(Dictionary<string, float> kvps)
        {
            Vector = new float[Map.Count];

            RemapVector(kvps);
        }
        public ResourceVector(Dictionary<string, float> kvps, float defaultValue)
        {
            Vector = new float[Map.Count];
            for (int i = 0; i < Map.Count; i++)
            {
                Vector[i] = defaultValue;
            }

            RemapVector(kvps);
        }

        public void RemapVector(Dictionary<string, float> kvps)
        {
            foreach (var kvp in kvps)
            {
                Vector[Map[kvp.Key]] = kvp.Value;
            }
        }

        public ResourceVector Copy()
        {
            return new ResourceVector(Vector);
        }

        public static void Initialize(string s)
        {
            if (Map == null)
            {
                Map = new Dictionary<string, byte>();
            }

            Map[s] = (byte)(Map.Count + 1);
        }
        public float Dot(ResourceVector other)
        {
            float ret = 0;
            for (int i = 0; i < Map.Count; i++)
            {
                ret += other.Vector[i] * Vector[i];
            }

            return ret;
        }
        public void Multiply(float mult)
        {
            Vector.IndividualMultiply(mult);
        }
        public void Multiply(int mult)
        {
            Vector.IndividualMultiply(mult);
        }
        public void ElementMultiply(ResourceVector other)
        {
            for (int i = 0; i < Map.Count; i++)
            {
                Vector[i] = Vector[i] * other.Vector[i];
            }
        }
        public float Sum()
        {
            float sum = 0;

            foreach (var f in Vector)
            {
                sum += f;
            }

            return sum;
        }
        public float LengthSq()
        {
            return Vector.VecLengthSq();
        }

        public float Length()
        {
            return Vector.VecLengthSq();
        }
        public float Normalize()
        {
            return Vector.VecNormalize();
        }

        public float SumNormalize()
        {
            return Vector.SumNormalize();
        }

        public IEnumerator<float> GetEnumerator()
        {
            return ((IEnumerable<float>)Vector).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Vector.GetEnumerator();
        }
        public float this[string key]
        {
            get
            {
                return Vector[Map[key]];
            }
            set
            {
                Vector[Map[key]] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Map.Count * 24);
            foreach (var str in Map)
            {
                if (Vector[str.Value] != 0)
                    sb.Append($"{str.Key}: {Vector[str.Value]*100:##.####}%\n");
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceVector && Vector.SequenceEqual(((ResourceVector)obj).Vector);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            for (int i = 0; i < Vector.Length; i++)
            {
                hash *= 31;
                hash ^= Vector[i].GetHashCode();
            }
            return hash;
        }
    }
}