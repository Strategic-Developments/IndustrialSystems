using IndustrialSystems.Utilities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class IndustrialSystemManager
    {
        public static IndustrialSystemManager I;

        public Dictionary<int, IndustrialSystem> Systems;
        public static void Load()
        {
            I = new IndustrialSystemManager
            {
                Systems = new Dictionary<int, IndustrialSystem>(),
            };
        }
        public static void Unload()
        {
            I = null;
        }
        public void OnPartAdd(int assemblyId, IMyCubeBlock block, bool isBasePart)
        {
            if (!Systems.ContainsKey(assemblyId))
                Systems.Add(assemblyId, new IndustrialSystem(assemblyId));

            Systems[assemblyId].AddPart(block);
        }
        public void OnPartRemove(int assemblyId, IMyCubeBlock block, bool isBasePart)
        {
            if (!Systems.ContainsKey(assemblyId))
                return;

            Systems[assemblyId].RemovePart(block);
        }

        public void OnAssemblyDestroy(int assemblyId)
        {
            Systems.Remove(assemblyId);
        }

        public void Update()
        {
            
        }
    }
}
