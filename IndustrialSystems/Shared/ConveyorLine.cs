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
    public class ConveyorLine : IIndustrialSystemMachine
    {
        public IIndustrialSystemMachine Destination;
        public IIndustrialSystemMachine Start;

        public List<IMyCubeBlock> Path;
        public List<ConveyorItem> Items;

        public ushort DistanceToInsertAtStart;
    }
}
