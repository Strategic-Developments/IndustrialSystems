using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class Pipeline : IIndustrialSystemMachine
    {
        public List<IMyCubeBlock> Path;
        public List<IFluidInteractable> ConnectedMachines;
        public FluidContainer StoredFluid;

        public ushort DistanceToInsertAtStart;

        public Pipeline(List<IMyCubeBlock> blocks)
        {
            Path = blocks;
            StoredFluid = FluidContainer.CreateInvalid();
            DistanceToInsertAtStart = 0;
        }

        public void Close()
        {
            
        }

        public bool IsBlockAPartOf(IMyCubeBlock block)
        {
            return Path.Contains(block);
        }
    }
}
