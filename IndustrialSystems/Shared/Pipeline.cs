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
        public IndustrialSystem parentSystem;
        public List<IMyCubeBlock> Path;
        public List<IFluidInteractable> ConnectedMachines;
        public FluidContainer StoredFluid;

        public ushort DistanceToInsertAtStart;

        public Pipeline(List<IMyCubeBlock> blocks, IndustrialSystem parentSystem)
        {
            Path = blocks;
            StoredFluid = FluidContainer.CreateInvalid();
            DistanceToInsertAtStart = 0;
            this.parentSystem = parentSystem;
        }

        public void Close()
        {
            Path = null;
            ConnectedMachines = null;
        }

        public Vector3D GetPosition()
        {
            if (Path.Count == 0)
                return Vector3D.Zero;

            return Path[0].GetPosition();
        }
    }
}
